#pragma once

#include <stdint.h>

typedef void *Log4JEvent;

void Log4JEventLevel (const Log4JEvent log4JEvent, const char **value, size_t *size);

void Log4JEventLogger (const Log4JEvent log4JEvent, const char **value, size_t *size);

void Log4JEventThread (const Log4JEvent log4JEvent, const char **value, size_t *size);

void Log4JEventTimestamp (const Log4JEvent log4JEvent, const char **value, size_t *size);
int64_t Log4JEventTime (const Log4JEvent log4JEvent);

void Log4JEventMessage (const Log4JEvent log4JEvent, const char **value, size_t *size);

void Log4JEventThrowable (const Log4JEvent log4JEvent, const char **value, size_t *size);

typedef struct Log4JEventSource_ Log4JEventSource;

void Log4JEventSourceInitXmlString (Log4JEventSource **self, char *xmlString);

void Log4JEventSourceDestroy (Log4JEventSource *self);

Log4JEvent Log4JEventSourceFirst (const Log4JEventSource *self);

Log4JEvent Log4JEventSourceNext (const Log4JEventSource *self, Log4JEvent event);
