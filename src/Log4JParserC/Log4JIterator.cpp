#include <stdlib.h>
#include <memory.h>
#include "Log4JIterator.h"

typedef bool log4j_iterator_move_next_cb (void *context);

typedef const Log4JEvent log4j_iterator_current_cb (const void *context);

typedef void log4j_iterator_destroy_cb (void *context);

struct Log4JIterator_ {
    void *context;

    log4j_iterator_destroy_cb *destroy;
    log4j_iterator_move_next_cb *move_next;
    log4j_iterator_current_cb *current;
};

void log4j_iterator_destroy (Log4JIterator *self) {
    self->destroy (self->context);
    free (self);
}

bool log4j_iterator_move_next (Log4JIterator *self) {
    return self->move_next (self->context);
}

const Log4JEvent log4j_iterator_current (const Log4JIterator *self) {
    return self->current (self->context);
}

#pragma region Document log4j_iterator

struct _log4j_iterator_document_context {
    const rapidxml::xml_document<> *doc;
    bool start;
    rapidxml::xml_node<> *node;
};

static void _log4j_iterator_document_destroy (void *context);
static bool _log4j_iterator_document_move_next (void *context);
static const Log4JEvent _log4j_iterator_document_current (const void *context);

void log4j_iterator_init_document (Log4JIterator **self, const rapidxml::xml_document<> *doc) {
    auto context = (_log4j_iterator_document_context *) malloc (sizeof (_log4j_iterator_document_context));
    *context = { doc, true, nullptr };

    auto result = (Log4JIterator *) malloc (sizeof (Log4JIterator));
    *result = {
        context,
        &_log4j_iterator_document_destroy,
        &_log4j_iterator_document_move_next,
        &_log4j_iterator_document_current
    };

    *self = result;
}

void _log4j_iterator_document_destroy (void *context) {
    auto context_d = (_log4j_iterator_document_context *) context;

    *context_d = { nullptr, false, nullptr };
    free (context_d);
}

bool _log4j_iterator_document_move_next (void *context) {
    auto context_d = (_log4j_iterator_document_context *) context;

    if (!context_d->doc) {
        return false;
    }

    rapidxml::xml_node<> *next_node;

    if (context_d->start) {
        next_node = log4j_event<>::first_node (context_d->doc);
        context_d->start = false;
    } else if (context_d->node) {
        next_node = log4j_event<>::next_sibling (context_d->node);
    } else {
        next_node = nullptr;
    }

    context_d->node = next_node;
    return next_node != nullptr;
}

const log4j_event<> _log4j_iterator_document_current (const void *context) {
    auto context_d = (const _log4j_iterator_document_context *) context;
    log4j_event<> event (context_d->node);
    return event;
}

#pragma endregion

#pragma region Filter log4j_iterator

struct _log4j_iterator_filter_context {
    Log4JIterator *inner;
    const Filter *filter;
};

static void _log4j_iterator_filter_destroy (void *context);
static bool _log4j_iterator_filter_move_next (void *context);
static const Log4JEvent _log4j_iterator_filter_current (const void *context);

void log4j_iterator_init_filter (Log4JIterator **self, Log4JIterator *inner, const Filter *filter) {
    auto context = (_log4j_iterator_filter_context *) malloc (sizeof (_log4j_iterator_filter_context));
    *context = { inner, filter };

    auto result = (Log4JIterator *) malloc (sizeof (Log4JIterator));
    *result = {
        context,
        &_log4j_iterator_filter_destroy,
        &_log4j_iterator_filter_move_next,
        &_log4j_iterator_filter_current
    };

    *self = result;
}

void _log4j_iterator_filter_destroy (void *context) {
    auto context_f = (_log4j_iterator_filter_context *) context;

    *context_f = { nullptr, nullptr };
    free (context_f);
}

bool _log4j_iterator_filter_move_next (void *context) {
    auto context_f = (_log4j_iterator_filter_context *) context;

    while (log4j_iterator_move_next (context_f->inner)) {
        auto event = log4j_iterator_current (context_f->inner);
        if (filter_apply (context_f->filter, &event)) {
            return true;
        }
    }

    return false;
}

const log4j_event<> _log4j_iterator_filter_current (const void *context) {
    auto context_f = (_log4j_iterator_filter_context *) context;

    return log4j_iterator_current (context_f->inner);
}

#pragma endregion
