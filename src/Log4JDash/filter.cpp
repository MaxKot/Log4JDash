#include "stdafx.h"
#include "filter.h"
#include "level_parser.h"

typedef bool filter_apply_cb (const filter *self, const log4j_event<> *event);

typedef void filter_destroy_cb (filter *self);

struct _filter {
    filter_apply_cb *apply;
    filter_destroy_cb *destroy;
};

void filter_destroy (filter *self) {
    self->destroy (self);
    *self = {nullptr, nullptr};
    free (self);
}

bool filter_apply (const filter *self, const log4j_event<> *event) {
    return self->apply (self, event);
}

static inline bool _filter_equals (const filter *x, const filter *y) {
    return x == y;
}

#pragma region Level lilter

struct _filter_level {
    struct _filter base;
    int32_t min;
    int32_t max;
};

static void _filter_level_destroy (filter *context);
static bool _filter_level_apply (const filter *context, const log4j_event<> *event);

void filter_init_level_i (filter **self, int32_t min, int32_t max) {
    auto result = (_filter_level *) malloc (sizeof (_filter_level));
    *result = {{&_filter_level_apply, &_filter_level_destroy}, min, max};

    *self = (_filter *) result;
}

void filter_init_level_c (filter **self, const char *min, const char *max) {
    auto min_i = level_parser<char>::instance.get_value (min);
    auto max_i = level_parser<char>::instance.get_value (max);

    filter_init_level_i (self, min_i, max_i);
}

static void _filter_level_destroy (filter *self) {
    auto self_l = (_filter_level *) self;
    self_l->min = 0;
    self_l->max = 0;
}

static bool _filter_level_apply (const filter *self, const log4j_event<> *event) {
    auto self_l = (const _filter_level *) self;

    auto val_string = event->get_level ();
    int32_t value = level_parser<char>::instance.get_value (val_string.value (), val_string.size ());

    return self_l->min <= value && value <= self_l->max;
}

#pragma endregion

#pragma region Logger filter

struct _filter_logger {
    struct _filter base;
    char *logger;
    size_t logger_size;
};

static void _filter_logger_destroy (filter *context);
static bool _filter_logger_apply (const filter *context, const log4j_event<> *event);

void filter_init_logger_fs (filter **self, const char *logger, const size_t logger_size) {
    // Reminder: logger_size must be multiplied by sizeof (Ch) if this code is reused for non-char
    // strings (e.g. wchar strings).
    auto context_logger = (char *) malloc (logger_size);
    memcpy (context_logger, logger, logger_size);

    auto result = (_filter_logger *) malloc (sizeof (_filter_logger));
    *result = {{&_filter_logger_apply, &_filter_logger_destroy}, context_logger, logger_size};

    *self = (filter *) result;
}

void filter_init_logger_nt (filter **self, const char *logger) {
    auto logger_size = strlen (logger);
    filter_init_logger_fs (self, logger, logger_size);
}

static void _filter_logger_destroy (filter *self) {
    auto self_l = (_filter_logger *) self;

    free (self_l->logger);
    self_l->logger = nullptr;
    self_l->logger_size = 0;
}

static bool _filter_logger_apply (const filter *self, const log4j_event<> *event) {
    auto self_l = (const _filter_logger *) self;

    auto value = event->get_logger ();

    auto logger = self_l->logger;
    auto logger_size = self_l->logger_size;

    return value.size () >= logger_size &&
           _strnicmp (value.value (), logger, logger_size) == 0;
}

#pragma endregion

#pragma region Message filter

struct _filter_message {
    struct _filter base;
    char *message;
    size_t message_size;
};

static void _filter_message_destroy (filter *self);
static bool _filter_message_apply (const filter *self, const log4j_event<> *event);

void filter_init_message_fs (filter **self, const char *message, const size_t message_size) {
    // Reminder: message_size must be multiplied by sizeof (Ch) if this code is reused for non-char
    // strings (e.g. wchar strings).
    auto context_message = (char *) malloc (message_size);
    memcpy (context_message, message, message_size);

    auto result = (_filter_message *) malloc (sizeof (_filter_message));
    *result = {{&_filter_message_apply, &_filter_message_destroy}, context_message, message_size};

    *self = (filter *) result;
}

void filter_init_message_nt (filter **self, const char *message) {
    auto message_size = strlen (message);
    filter_init_message_fs (self, message, message_size);
}

static void _filter_message_destroy (filter *self) {
    auto self_m = (_filter_message *) self;

    free (self_m->message);
    self_m->message = nullptr;
    self_m->message_size = 0;
}

static bool _filter_message_apply (const filter *self, const log4j_event<> *event) {
    auto self_m = (const _filter_message *) self;

    if (self_m->message_size == 0) {
        return true;
    }

    auto message = event->get_message ();
    size_t value_size = message.size ();

    if (value_size < self_m->message_size) {
        return false;
    }

    const char *value = message.value ();
    const char message_head = *self_m->message;
    const char *message_tail = self_m->message + 1;
    const size_t message_tail_size = self_m->message_size - 1;

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

struct _filter_timestamp {
    struct _filter base;
    int64_t min;
    int64_t max;
};

static void _filter_timestamp_destroy (filter *self);
static bool _filter_timestamp_apply (const filter *self, const log4j_event<> *event);

void filter_init_timestamp (filter **self, int64_t min, int64_t max) {
    auto result = (_filter_timestamp *) malloc (sizeof (_filter_timestamp));
    *result = {{&_filter_timestamp_apply, &_filter_timestamp_destroy}, min, max};

    *self = (filter *) result;
}

static void _filter_timestamp_destroy (filter *self) {
    auto self_t = (_filter_timestamp *) self;

    self_t->min = 0;
    self_t->max = 0;
}

static bool _filter_timestamp_apply (const filter *self, const log4j_event<> *event) {
    auto self_t = (const _filter_timestamp *) self;

    auto value = event->get_time ();

    return self_t->min <= value && value <= self_t->max;
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

struct _filter_all {
    struct _filter base;
    _filter_entry *children_head;
};

static void _filter_all_destroy (filter *self);
static bool _filter_all_apply (const filter *self, const log4j_event<> *event);

void filter_init_all (filter **self) {
    auto result = (_filter_all *) malloc (sizeof (_filter_all));
    *result = {{&_filter_all_apply, &_filter_all_destroy}, nullptr};

    *self = (filter *) result;
}

static void _filter_all_destroy (filter *self) {
    auto self_a = (_filter_all *) self;

    if (self_a->children_head) {
        _filter_list_destroy (self_a->children_head);
    }
    self_a->children_head = nullptr;
}

void filter_all_add (filter *self, filter *child) {
    auto self_a = (_filter_all *) self;

    self_a->children_head = _filter_list_add (self_a->children_head, child);
}

void filter_all_remove (filter *self, filter *child) {
    auto self_a = (_filter_all *) self;

    self_a->children_head = _filter_list_remove (self_a->children_head, child);
}

static bool _filter_all_apply (const filter *self, const log4j_event<> *event) {
    auto self_a = (const _filter_all *) self;

    auto current = self_a->children_head;

    bool result = true;
    while (current && result) {
        result = result && filter_apply (current->filter, event);
        current = current->next;
    }

    return result;
}

#pragma endregion

#pragma region Any filter

struct _filter_any {
    struct _filter base;
    _filter_entry *children_head;
};

static void _filter_any_destroy (filter *self);
static bool _filter_any_apply (const filter *self, const log4j_event<> *event);

void filter_init_any (filter **self) {
    auto result = (_filter_any *) malloc (sizeof (_filter_any));
    *result = {{&_filter_any_apply, &_filter_any_destroy}, nullptr};

    *self = (filter *) result;
}

static void _filter_any_destroy (filter *self) {
    auto self_a = (_filter_any *) self;

    if (self_a->children_head) {
        _filter_list_destroy (self_a->children_head);
    }
    self_a->children_head = nullptr;
}

void filter_any_add (filter *self, filter *child) {
    auto self_a = (_filter_any *) self;

    self_a->children_head = _filter_list_add (self_a->children_head, child);
}

void filter_any_remove (filter *self, filter *child) {
    auto self_a = (_filter_any *) self;

    self_a->children_head = _filter_list_remove (self_a->children_head, child);
}

static bool _filter_any_apply (const filter *self, const log4j_event<> *event) {
    auto self_a = (const _filter_any *) self;

    auto current = self_a->children_head;

    bool result = false;
    while (current && !result) {
        result = result || filter_apply (current->filter, event);
        current = current->next;
    }

    return result;
}

#pragma endregion

#pragma region Not filter

struct _filter_not {
    struct _filter base;
    filter *child_filter;
};

static void _filter_not_destroy (filter *context);
static bool _filter_not_apply (const filter *context, const log4j_event<> *event);

void filter_init_not (filter **self, filter *child_filter) {
    auto result = (_filter_not *) malloc (sizeof (_filter_not));
    *result = {{&_filter_not_apply, &_filter_not_destroy}, child_filter};

    *self = (filter *) result;
}

static void _filter_not_destroy (filter *self) {
    auto self_n = (_filter_not *) self;

    self_n->child_filter = nullptr;
}

static bool _filter_not_apply (const filter *self, const log4j_event<> *event) {
    auto self_n = (const _filter_not *) self;

    return !filter_apply (self_n->child_filter, event);
}

#pragma endregion
