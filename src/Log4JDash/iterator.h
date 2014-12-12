#pragma once

#include <rapidxml\rapidxml.hpp>
#include "log4j_event.h"
#include "filter.h"

typedef struct _iterator iterator;

void iterator_destroy (iterator *self);

bool iterator_move_next (iterator *self);

const log4j_event<> iterator_current (const iterator *self);

void iterator_init_document (iterator **self, const rapidxml::xml_document<> *doc);

void iterator_init_filter (iterator **self, iterator *inner, const filter *filter);
