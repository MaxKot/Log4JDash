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
};

struct _filter_array {
    struct _filter_entry *elements;
    size_t count;
};

static void _filter_array_destroy (_filter_array *array) {
    if (array->elements) {
        free (array->elements);
    }
    array->elements = nullptr;
    array->count = 0UL;
}

static void _filter_array_add (_filter_array *array, filter *filter, void *context) {
    auto new_count = array->count + 1UL;
    auto new_elements = (_filter_entry *) realloc (array->elements, sizeof (_filter_entry)*new_count);
    if (!new_elements) {
        return;
    }

    new_elements[new_count - 1] = _filter_entry { filter, context };

    array->elements = new_elements;
    array->count = new_count;
}

static void _filter_array_remove (_filter_array *array, filter *filter, void *context) {
    auto elements = array->elements;
    auto count = array->count;

    if (elements) {
        bool result = false;
        for (size_t i = 0; i < count && !result; ++i) {
            auto element = array->elements[count - 1 - i];
            if (element.context == context && element.filter == filter) {
                result = true;

                auto new_count = count - 1UL;
                if (new_count > 0) {
                    auto new_elements = (_filter_entry *) malloc (sizeof (_filter_entry)*new_count);
                    if (!new_elements) {
                        return;
                    }

                    for (size_t j = 0; j < count - 1 - i; ++j) {
                        new_elements[j] = elements[j];
                    }
                    for (size_t j = count - i; j < count; ++j) {
                        new_elements[j - 1] = elements[j];
                    }

                    free (elements);
                    array->elements = new_elements;
                    array->count = new_count;
                } else {
                    free (elements);
                    array->elements = nullptr;
                    array->count = 0UL;
                }
            }
        }
    }
}

// And filter

struct _filter_and_context {
    _filter_array children;
};

void filter_and_init (filter_and_context **context) {
    auto result = (filter_and_context *) malloc (sizeof (filter_and_context));

    result->children = _filter_array { nullptr, 0UL };

    *context = result;
}

void filter_and_destroy (filter_and_context *context) {
    _filter_array_destroy (&context->children);

    free (context);
}

void filter_and_add (filter_and_context *context, filter *child, void *child_context) {
    _filter_array_add (&context->children, child, child_context);
}

void filter_and_remove (filter_and_context *context, filter *child, void *child_context) {
    _filter_array_remove (&context->children, child, child_context);
}

bool filter_and (void *context, const log4j_event<> *event) {
    auto context_a = (filter_and_context *) context;
    auto children = context_a->children.elements;
    auto child_count = context_a->children.count;

    bool result;
    if (children && child_count > 0) {
        result = true;
        for (size_t i = 0UL; i < child_count && result; ++i) {
            auto child = children[i];
            result = result && child.filter (child.context, event);
        }
    } else {
        result = true;
    }

    return result;
}
