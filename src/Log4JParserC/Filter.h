#pragma once

#include "Log4JEvent.h"

typedef struct Filter_ Filter;

void FilterDestroy (Filter *self);

bool FilterApply (const Filter *self, const Log4JEvent event);

// Level filter

void FilterInitLevelI (Filter **self, int32_t min, int32_t max);
void FilterInitLevelC (Filter **self, const char *min, const char *max);

// Logger filter

void FilterInitLoggerFs (Filter **self, const char *logger, const size_t loggerSize);
void FilterInitLoggerNt (Filter **self, const char *logger);

// Message filter

void FilterInitMessageFs (Filter **self, const char *message, const size_t messageSize);
void FilterInitMessageNt (Filter **self, const char *message);

// Timestamp filter

void FilterInitTimestamp (Filter **self, int64_t min, int64_t max);

// All filter

void FilterInitAll (Filter **self);

void FilterAllAdd (Filter *self, Filter *childFilter);
void FilterAllRemove (Filter *self, Filter *childFilter);

// Any filter

void FilterInitAny (Filter **self);

void FilterAnyAdd (Filter *self, Filter *childFilter);
void FilterAnyRemove (Filter *self, Filter *childFilter);

// Not filter

void FilterInitNot (Filter **self, Filter *childFilter);
