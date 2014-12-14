#pragma once

#include <stdint.h>

typedef struct
{
    const char *Value;
    const size_t Size;
} FixedString;

typedef void *Log4JEvent;

FixedString Log4JEventLevel (const Log4JEvent log4JEvent);

FixedString Log4JEventLogger (const Log4JEvent log4JEvent);

FixedString Log4JEventThread (const Log4JEvent log4JEvent);

FixedString Log4JEventTimestamp (const Log4JEvent log4JEvent);
int64_t Log4JEventTime (const Log4JEvent log4JEvent);

FixedString Log4JEventMessage (const Log4JEvent log4JEvent);

FixedString Log4JEventThrowable (const Log4JEvent log4JEvent);

void Log4JEventInitFirst (Log4JEvent *event, const char *xmlString);

bool Log4JEventNext (Log4JEvent *event);

void Log4JEventDestroy (Log4JEvent *event);