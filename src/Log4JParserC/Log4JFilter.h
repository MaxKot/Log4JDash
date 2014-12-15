#pragma once

#include "Log4JEvent.h"

typedef struct Log4JFilter_ Log4JFilter;

void Log4JFilterDestroy (Log4JFilter *self);

bool Log4JFilterApply (const Log4JFilter *self, const Log4JEvent event);

// Level filter

void Log4JFilterInitLevelI (Log4JFilter **self, int32_t min, int32_t max);
void Log4JFilterInitLevelC (Log4JFilter **self, const char *min, const char *max);

// Logger filter

void Log4JFilterInitLoggerFs (Log4JFilter **self, const char *logger, const size_t loggerSize);
void Log4JFilterInitLoggerNt (Log4JFilter **self, const char *logger);

// Message filter

void Log4JFilterInitMessageFs (Log4JFilter **self, const char *message, const size_t messageSize);
void Log4JFilterInitMessageNt (Log4JFilter **self, const char *message);

// Timestamp filter

void Log4JFilterInitTimestamp (Log4JFilter **self, int64_t min, int64_t max);

// All filter

void Log4JFilterInitAll (Log4JFilter **self);

void Log4JFilterAllAdd (Log4JFilter *self, const Log4JFilter *childFilter);
void Log4JFilterAllRemove (Log4JFilter *self, const Log4JFilter *childFilter);

// Any filter

void Log4JFilterInitAny (Log4JFilter **self);

void Log4JFilterAnyAdd (Log4JFilter *self, const Log4JFilter *childFilter);
void Log4JFilterAnyRemove (Log4JFilter *self, const Log4JFilter *childFilter);

// Not filter

void Log4JFilterInitNot (Log4JFilter **self, const Log4JFilter *childFilter);
