#include "stdafx.h"
#include "filter.h"
#include "level_parser.h"

// Level filter

struct _filter_level_context {
    int32_t min;
    int32_t max;
};

void filter_level_init_i (filter_level_context **context, int32_t min, int32_t max) {
    auto result = (filter_level_context *) malloc (sizeof (filter_level_context));

    result->min = min;
    result->max = max;

    *context = result;
}

void filter_level_init_c (filter_level_context **context, const char *min, const char *max) {
    auto min_i = level_parser<char>::instance.get_value (min);
    auto max_i = level_parser<char>::instance.get_value (max);

    filter_level_init_i (context, min_i, max_i);
}

void filter_level_destroy (filter_level_context *context) {
    free (context);
}

bool filter_level (void *context, const log4j_event<> *event) {
    auto context_l = (filter_level_context *) context;

    auto val_string = event->get_level ();
    int32_t value = level_parser<char>::instance.get_value (val_string.value (), val_string.size ());

    return context_l->min <= value && value <= context_l->max;
}

// Logger filter

struct _filter_logger_context {
    char *logger;
    size_t logger_size;
};

void filter_logger_init_fs (filter_logger_context **context, const char *logger, const size_t logger_size) {
    auto result = (filter_logger_context *) malloc (sizeof (filter_logger_context));

    // Reminder: logger_size must be multiplied by sizeof (Ch) if this code is reused for non-char
    // strings (e.g. wchar strings).
    result->logger = (char *) malloc (logger_size);
    memcpy (result->logger, logger, logger_size);
    result->logger_size = logger_size;

    *context = result;
}

void filter_logger_init_nt (filter_logger_context **context, const char *logger) {
    auto logger_size = strlen (logger);
    filter_logger_init_fs (context, logger, logger_size);
}

void filter_logger_destroy (filter_logger_context *context) {
    free (context->logger);
    context->logger = nullptr;
    context->logger_size = 0;

    free (context);
}

bool filter_logger (void *context, const log4j_event<> *event) {
    auto context_l = (filter_logger_context *) context;

    auto value = event->get_logger ();

    auto logger = context_l->logger;
    auto logger_size = context_l->logger_size;

    return value.size () >= logger_size &&
           _strnicmp (value.value (), logger, logger_size) == 0;
}

// Message filter

struct _filter_message_context {
    char *message;
    size_t message_size;
};

void filter_message_init_fs (filter_message_context **context, const char *message, const size_t message_size) {
    auto result = (filter_message_context *) malloc (sizeof (filter_message_context));

    // Reminder: message_size must be multiplied by sizeof (Ch) if this code is reused for non-char
    // strings (e.g. wchar strings).
    result->message = (char *) malloc (message_size);
    memcpy (result->message, message, message_size);
    result->message_size = message_size;

    *context = result;
}

void filter_message_init_nt (filter_message_context **context, const char *message) {
    auto message_size = strlen (message);
    filter_message_init_fs (context, message, message_size);
}

void filter_message_destroy (filter_message_context *context) {
    free (context->message);
    context->message = nullptr;
    context->message_size = 0;

    free (context);
}

bool filter_message (void *context, const log4j_event<> *event) {
    auto context_m = (filter_message_context *) context;

    if (context_m->message_size == 0) {
        return true;
    }

    auto message = event->get_message ();
    size_t value_size = message.size ();

    if (value_size < context_m->message_size) {
        return false;
    }

    const char *value = message.value ();
    const char message_head = *context_m->message;
    const char *message_tail = context_m->message + 1;
    const size_t message_tail_size = context_m->message_size - 1;

    do {
        char sc;

        do {
            sc = *value;
            ++value;
            --value_size;

            if (value_size < message_tail_size) {
                return false;
            }
        } while (sc != message_head);
    } while (strncmp (value, message_tail, message_tail_size) != 0);

    return true;
}

// Timestamp filter

struct _filter_timestamp_context {
    int64_t min;
    int64_t max;
};

void filter_timestamp_init (filter_timestamp_context **context, int64_t min, int64_t max) {
    auto result = (filter_timestamp_context *) malloc (sizeof (filter_timestamp_context));

    result->min = min;
    result->max = max;

    *context = result;
}

void filter_timestamp_destroy (filter_timestamp_context *context) {
    context->min = 0;
    context->max = 0;

    free (context);
}

bool filter_timestamp (void *context, const log4j_event<> *event) {
    auto context_t = (filter_timestamp_context *) context;

    auto value = event->get_time ();

    return context_t->min <= value && value <= context_t->max;
}

// Composite filters

struct _filter_entry {
    filter *filter;
    void *context;
    _filter_entry *next;
};

static void _filter_list_destroy (_filter_entry *head) {
    if (head->next) {
        _filter_list_destroy (head->next);
    }
    head->filter = nullptr;
    head->context = nullptr;

    free (head);
}

static _filter_entry *_filter_list_add (_filter_entry *head, filter *filter, void *context) {
    auto result = (_filter_entry *) malloc (sizeof (_filter_entry));
    *result = _filter_entry { filter, context, head };
    return result;
}

static _filter_entry *_filter_list_remove (_filter_entry *head, filter *filter, void *context) {
    auto current = head;
    auto prev_ptr = &head;

    while (current) {
        if (current->context == context && current->filter == filter) {
            *prev_ptr = current->next;
            current->context = nullptr;
            current->filter = nullptr;
            current->next = nullptr;
            free (current);

            break;
        }

        prev_ptr = &(current->next);
        current = current->next;
    }

    return head;
}

// All filter

struct _filter_all_context {
    _filter_entry *children_head;
};

void filter_all_init (filter_all_context **context) {
    auto result = (filter_all_context *) malloc (sizeof (filter_all_context));

    result->children_head = nullptr;

    *context = result;
}

void filter_all_destroy (filter_all_context *context) {
    if (context->children_head) {
        _filter_list_destroy (context->children_head);
    }

    free (context);
}

void filter_all_add (filter_all_context *context, filter *child, void *child_context) {
    context->children_head = _filter_list_add (context->children_head, child, child_context);
}

void filter_all_remove (filter_all_context *context, filter *child, void *child_context) {
    context->children_head = _filter_list_remove (context->children_head, child, child_context);
}

bool filter_all (void *context, const log4j_event<> *event) {
    auto context_a = (filter_all_context *) context;

    auto current = context_a->children_head;

    bool result = true;
    while (current && result) {
        result = result && current->filter (current->context, event);
        current = current->next;
    }

    return result;
}

// Any filter

struct _filter_any_context {
    _filter_entry *children_head;
};

void filter_any_init (filter_any_context **context) {
    auto result = (filter_any_context *) malloc (sizeof (filter_any_context));

    result->children_head = nullptr;

    *context = result;
}

void filter_any_destroy (filter_any_context *context) {
    if (context->children_head) {
        _filter_list_destroy (context->children_head);
    }

    free (context);
}

void filter_any_add (filter_any_context *context, filter *child, void *child_context) {
    context->children_head = _filter_list_add (context->children_head, child, child_context);
}

void filter_any_remove (filter_any_context *context, filter *child, void *child_context) {
    context->children_head = _filter_list_remove (context->children_head, child, child_context);
}

bool filter_any (void *context, const log4j_event<> *event) {
    auto context_a = (filter_any_context *) context;

    auto current = context_a->children_head;

    bool result = false;
    while (current && !result) {
        result = result || current->filter (current->context, event);
        current = current->next;
    }

    return result;
}

// Not filter

struct _filter_not_context {
    filter *child_filter;
    void *child_context;
};

void filter_not_init (filter_not_context **context, filter *child_filter, void *child_context) {
    auto result = (filter_not_context *) malloc (sizeof (filter_not_context));

    result->child_filter = child_filter;
    result->child_context = child_context;

    *context = result;
}

void filter_not_destroy (filter_not_context *context) {
    context->child_filter = nullptr;
    context->child_context = nullptr;

    free (context);
}

bool filter_not (void *context, const log4j_event<> *event) {
    auto context_n = (filter_not_context *) context;

    return !context_n->child_filter (context_n->child_context, event);
}
