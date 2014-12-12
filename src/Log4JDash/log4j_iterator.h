#pragma once

#include <rapidxml\rapidxml.hpp>
#include "log4j_event.h"
#include "filter.h"

typedef struct _log4j_iterator log4j_iterator;

void log4j_iterator_destroy (log4j_iterator *self);

bool log4j_iterator_move_next (log4j_iterator *self);

const log4j_event<> log4j_iterator_current (const log4j_iterator *self);

void log4j_iterator_init_document (log4j_iterator **self, const rapidxml::xml_document<> *doc);

void log4j_iterator_init_filter (log4j_iterator **self, log4j_iterator *inner, const filter *filter);
