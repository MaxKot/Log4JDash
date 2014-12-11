#include "stdafx.h"
#include "filter.h"
#include "level_parser.h"

typedef bool filter_apply_cb (void *context, const log4j_event<> *event);

typedef void filter_destroy_cb (void *context);

struct _filter {
    void *context;

    filter_apply_cb *apply;
    filter_destroy_cb *destroy;
};

void filter_destroy (filter *self) {
    self->destroy (self->context);
    free (self);
}

bool filter_apply (const filter *self, const log4j_event<> *event) {
    return self->apply (self->context, event);
}

static bool _filter_equals (const filter *x, const filter *y) {
    return x->context == y->context &&
           x->apply == y->apply &&
           x->destroy == y->destroy;
}

#pragma region Level lilter

struct _filter_level_context {
    int32_t min;
    int32_t max;
};

static void _filter_level_destroy (void *context);
static bool _filter_level_apply (void *context, const log4j_event<> *event);

void filter_init_level_i (filter **self, int32_t min, int32_t max) {
    auto context = (_filter_level_context *) malloc (sizeof (_filter_level_context));
    *context = {min, max};

    auto result = (filter *) malloc (sizeof (filter));
    *result = {context, &_filter_level_apply, &_filter_level_destroy};

    *self = result;
}

void filter_init_level_c (filter **self, const char *min, const char *max) {
    auto min_i = level_parser<char>::instance.get_value (min);
    auto max_i = level_parser<char>::instance.get_value (max);

    filter_init_level_i (self, min_i, max_i);
}

static void _filter_level_destroy (void *context) {
    auto context_l = (_filter_level_context *) context;

    *context_l = {0, 0};
    free (context_l);
}

static bool _filter_level_apply (void *context, const log4j_event<> *event) {
    auto context_l = (_filter_level_context *) context;

    auto val_string = event->get_level ();
    int32_t value = level_parser<char>::instance.get_value (val_string.value (), val_string.size ());

    return context_l->min <= value && value <= context_l->max;
}

#pragma endregion

#pragma region Logger filter

struct _filter_logger_context {
    char *logger;
    size_t logger_size;
};

static void _filter_logger_destroy (void *context);
static bool _filter_logger_apply (void *context, const log4j_event<> *event);

void filter_init_logger_fs (filter **self, const char *logger, const size_t logger_size) {
    auto context = (_filter_logger_context *) malloc (sizeof (_filter_logger_context));
    // Reminder: logger_size must be multiplied by sizeof (Ch) if this code is reused for non-char
    // strings (e.g. wchar strings).
    auto context_logger = (char *) malloc (logger_size);
    memcpy (context_logger, logger, logger_size);
    *context = {context_logger, logger_size};

    auto result = (filter *) malloc (sizeof (filter));
    *result = {context, &_filter_logger_apply, &_filter_logger_destroy};

    *self = result;
}

void filter_init_logger_nt (filter **self, const char *logger) {
    auto logger_size = strlen (logger);
    filter_init_logger_fs (self, logger, logger_size);
}

static void _filter_logger_destroy (void *context) {
    auto context_l = (_filter_logger_context *) context;

    free (context_l->logger);
    context_l->logger = nullptr;
    context_l->logger_size = 0;

    free (context);
}

static bool _filter_logger_apply (void *context, const log4j_event<> *event) {
    auto context_l = (_filter_logger_context *) context;

    auto value = event->get_logger ();

    auto logger = context_l->logger;
    auto logger_size = context_l->logger_size;

    return value.size () >= logger_size &&
           _strnicmp (value.value (), logger, logger_size) == 0;
}

#pragma endregion

#pragma region Message filter

struct _filter_message_context {
    char *message;
    size_t message_size;
};

static void _filter_message_destroy (void *context);
static bool _filter_message_apply (void *context, const log4j_event<> *event);

void filter_init_message_fs (filter **self, const char *message, const size_t message_size) {
    auto context = (_filter_message_context *) malloc (sizeof (_filter_message_context));
    // Reminder: message_size must be multiplied by sizeof (Ch) if this code is reused for non-char
    // strings (e.g. wchar strings).
    auto context_message = (char *) malloc (message_size);
    memcpy (context_message, message, message_size);
    *context = {context_message, message_size};

    auto result = (filter *) malloc (sizeof (filter));
    *result = {context, &_filter_message_apply, &_filter_message_destroy};

    *self = result;
}

void filter_init_message_nt (filter **self, const char *message) {
    auto message_size = strlen (message);
    filter_init_message_fs (self, message, message_size);
}

static void _filter_message_destroy (void *context) {
    auto context_m = (_filter_message_context *) context;

    free (context_m->message);
    context_m->message = nullptr;
    context_m->message_size = 0;

    free (context);
}

static bool _filter_message_apply (void *context, const log4j_event<> *event) {
    auto context_m = (_filter_message_context *) context;

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

#pragma endregion

#pragma region Timestamp filter

struct _filter_timestamp_context {
    int64_t min;
    int64_t max;
};

static void _filter_timestamp_destroy (void *context);
static bool _filter_timestamp_apply (void *context, const log4j_event<> *event);

void filter_init_timestamp (filter **self, int64_t min, int64_t max) {
    auto context = (_filter_timestamp_context *) malloc (sizeof (_filter_timestamp_context));
    *context = {min, max};

    auto result = (filter *) malloc (sizeof (filter));
    *result = {context, &_filter_timestamp_apply, &_filter_timestamp_destroy};

    *self = result;
}

static void _filter_timestamp_destroy (void *context) {
    auto context_t = (_filter_timestamp_context *) context;

    *context_t = {0, 0};
    free (context_t);
}

static bool _filter_timestamp_apply (void *context, const log4j_event<> *event) {
    auto context_t = (_filter_timestamp_context *) context;

    auto value = event->get_time ();

    return context_t->min <= value && value <= context_t->max;
}

#pragma endregion

// Composite filters

struct _filter_entry {
    filter *filter;
    _filter_entry *next;
};

static void _filter_list_destroy (_filter_entry *head) {
    if (head->next) {
        _filter_list_destroy (head->next);
    }
    *head = {nullptr, nullptr};

    free (head);
}

static _filter_entry *_filter_list_add (_filter_entry *head, filter *filter) {
    auto result = (_filter_entry *) malloc (sizeof (_filter_entry));
    *result = {filter, head};
    return result;
}

static _filter_entry *_filter_list_remove (_filter_entry *head, filter *filter) {
    auto current = head;
    auto prev_ptr = &head;

    while (current) {
        if (_filter_equals (current->filter, filter)) {
            *prev_ptr = current->next;
            *current = {nullptr, nullptr};
            free (current);

            break;
        }

        prev_ptr = &(current->next);
        current = current->next;
    }

    return head;
}

#pragma region All filter

struct _filter_all_context {
    _filter_entry *children_head;
};

static void _filter_all_destroy (void *context);
static bool _filter_all_apply (void *context, const log4j_event<> *event);

void filter_init_all (filter **self) {
    auto context = (_filter_all_context *) malloc (sizeof (_filter_all_context));
    *context = {nullptr};

    auto result = (filter *) malloc (sizeof (filter));
    *result = {context, &_filter_all_apply, &_filter_all_destroy};

    *self = result;
}

static void _filter_all_destroy (void *context) {
    auto context_a = (_filter_all_context *) context;

    if (context_a->children_head) {
        _filter_list_destroy (context_a->children_head);
    }
    context_a = {nullptr};

    free (context);
}

void filter_all_add (filter *self, filter *child) {
    auto context = (_filter_all_context *) self->context;

    context->children_head = _filter_list_add (context->children_head, child);
}

void filter_all_remove (filter *self, filter *child) {
    auto context = (_filter_all_context *) self->context;

    context->children_head = _filter_list_remove (context->children_head, child);
}

static bool _filter_all_apply (void *context, const log4j_event<> *event) {
    auto context_a = (_filter_all_context *) context;

    auto current = context_a->children_head;

    bool result = true;
    while (current && result) {
        result = result && filter_apply (current->filter, event);
        current = current->next;
    }

    return result;
}

#pragma endregion

#pragma region Any filter

struct _filter_any_context {
    _filter_entry *children_head;
};

static void _filter_any_destroy (void *context);
static bool _filter_any_apply (void *context, const log4j_event<> *event);

void filter_init_any (filter **self) {
    auto context = (_filter_any_context *) malloc (sizeof (_filter_any_context));
    *context = {nullptr};

    auto result = (filter *) malloc (sizeof (filter));
    *result = {context, &_filter_any_apply, &_filter_any_destroy};

    *self = result;
}

static void _filter_any_destroy (void *context) {
    auto context_a = (_filter_any_context *) context;

    if (context_a->children_head) {
        _filter_list_destroy (context_a->children_head);
    }
    context_a = {nullptr};

    free (context);
}

void filter_any_add (filter *self, filter *child) {
    auto context = (_filter_any_context *) self->context;

    context->children_head = _filter_list_add (context->children_head, child);
}

void filter_any_remove (filter *self, filter *child) {
    auto context = (_filter_any_context *) self->context;

    context->children_head = _filter_list_remove (context->children_head, child);
}

static bool _filter_any_apply (void *context, const log4j_event<> *event) {
    auto context_a = (_filter_any_context *) context;

    auto current = context_a->children_head;

    bool result = false;
    while (current && !result) {
        result = result || filter_apply (current->filter, event);
        current = current->next;
    }

    return result;
}

#pragma endregion

#pragma region Not filter

struct _filter_not_context {
    filter *child_filter;
};

static void _filter_not_destroy (void *context);
static bool _filter_not_apply (void *context, const log4j_event<> *event);

void filter_init_not (filter **self, filter *child_filter) {
    auto context = (_filter_not_context *) malloc (sizeof (_filter_not_context));
    *context = {child_filter};

    auto result = (filter *) malloc (sizeof (filter));
    *result = {context, &_filter_not_apply, &_filter_not_destroy};

    *self = result;
}

static void _filter_not_destroy (void *context) {
    auto context_n = (_filter_not_context *) context;

    *context_n = {nullptr};

    free (context);
}

static bool _filter_not_apply (void *context, const log4j_event<> *event) {
    auto context_n = (_filter_not_context *) context;

    return !filter_apply (context_n->child_filter, event);
}

#pragma endregion
