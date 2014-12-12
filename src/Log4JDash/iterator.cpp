#include "stdafx.h"
#include "iterator.h"

typedef bool iterator_move_next_cb (void *context);

typedef const log4j_event<> iterator_current_cb (const void *context);

typedef void iterator_destroy_cb (void *context);

struct _iterator {
    void *context;

    iterator_destroy_cb *destroy;
    iterator_move_next_cb *move_next;
    iterator_current_cb *current;
};

void iterator_destroy (iterator *self) {
    self->destroy (self->context);
    free (self);
}

bool iterator_move_next (iterator *self) {
    return self->move_next (self->context);
}

const log4j_event<> iterator_current (const iterator *self) {
    return self->current (self->context);
}

#pragma region Document iterator

struct _iterator_document_context {
    const rapidxml::xml_document<> *doc;
    bool start;
    rapidxml::xml_node<> *node;
};

static void _iterator_document_destroy (void *context);
static bool _iterator_document_move_next (void *context);
static const log4j_event<> _iterator_document_current (const void *context);

void iterator_init_document (iterator **self, const rapidxml::xml_document<> *doc) {
    auto context = (_iterator_document_context *) malloc (sizeof (_iterator_document_context));
    *context = { doc, true, nullptr };

    auto result = (iterator *) malloc (sizeof (iterator));
    *result = {
        context,
        &_iterator_document_destroy,
        &_iterator_document_move_next,
        &_iterator_document_current
    };

    *self = result;
}

void _iterator_document_destroy (void *context) {
    auto context_d = (_iterator_document_context *) context;

    *context_d = { nullptr, false, nullptr };
    free (context_d);
}

bool _iterator_document_move_next (void *context) {
    auto context_d = (_iterator_document_context *) context;

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

const log4j_event<> _iterator_document_current (const void *context) {
    auto context_d = (const _iterator_document_context *) context;
    log4j_event<> event (context_d->node);
    return event;
}

#pragma endregion

#pragma region Filter iterator

struct _iterator_filter_context {
    iterator *inner;
    const filter *filter;
};

static void _iterator_filter_destroy (void *context);
static bool _iterator_filter_move_next (void *context);
static const log4j_event<> _iterator_filter_current (const void *context);

void iterator_init_filter (iterator **self, iterator *inner, const filter *filter) {
    auto context = (_iterator_filter_context *) malloc (sizeof (_iterator_filter_context));
    *context = { inner, filter };

    auto result = (iterator *) malloc (sizeof (iterator));
    *result = {
        context,
        &_iterator_filter_destroy,
        &_iterator_filter_move_next,
        &_iterator_filter_current
    };

    *self = result;
}

void _iterator_filter_destroy (void *context) {
    auto context_f = (_iterator_filter_context *) context;

    *context_f = { nullptr, nullptr };
    free (context_f);
}

bool _iterator_filter_move_next (void *context) {
    auto context_f = (_iterator_filter_context *) context;

    while (iterator_move_next (context_f->inner)) {
        auto event = iterator_current (context_f->inner);
        if (filter_apply (context_f->filter, &event)) {
            return true;
        }
    }

    return false;
}

const log4j_event<> _iterator_filter_current (const void *context) {
    auto context_f = (_iterator_filter_context *) context;

    return iterator_current (context_f->inner);
}

#pragma endregion
