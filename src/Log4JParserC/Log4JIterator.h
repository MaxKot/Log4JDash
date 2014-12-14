#pragma once

#include "Log4JEvent.h"
#include "Filter.h"

typedef struct Log4JIterator_ Log4JIterator;

void Log4JIteratorDestroy (Log4JIterator *self);

bool Log4JIteratorMoveNext (Log4JIterator *self);

const Log4JEvent Log4JIteratorCurrent (const Log4JIterator *self);

void Log4JIteratorInitStringNt (Log4JIterator **self, const char *doc);

void Log4JIteratorInitFilter (Log4JIterator **self, Log4JIterator *inner, const Filter *filter);
