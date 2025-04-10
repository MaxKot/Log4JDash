﻿#pragma once

#include <stdbool.h>
#include <stdint.h>

#ifdef LOG4JPARSERC_EXPORTS
#define LOG4JPARSERC_API __declspec(dllexport)
#else
#define LOG4JPARSERC_API __declspec(dllimport)
#endif

// Allocator

typedef void *(__cdecl Log4JAlloc) (size_t);

typedef void __cdecl Log4JFree (void *);

LOG4JPARSERC_API void Log4JSetAllocator (Log4JAlloc *alloc, Log4JFree *free);

LOG4JPARSERC_API void Log4JSetDefaultAllocator ();

// Event

typedef void *Log4JEvent;

LOG4JPARSERC_API void Log4JEventLevel (const Log4JEvent log4JEvent, const char **value, size_t *size);

LOG4JPARSERC_API void Log4JEventLogger (const Log4JEvent log4JEvent, const char **value, size_t *size);

LOG4JPARSERC_API void Log4JEventThread (const Log4JEvent log4JEvent, const char **value, size_t *size);

LOG4JPARSERC_API int64_t Log4JEventTimestamp (const Log4JEvent log4JEvent);

LOG4JPARSERC_API void Log4JEventMessage (const Log4JEvent log4JEvent, const char **value, size_t *size);

LOG4JPARSERC_API void Log4JEventThrowable (const Log4JEvent log4JEvent, const char **value, size_t *size);

#pragma pack (push, 4)

typedef struct
{
    const char *name;
    size_t nameSize;
    const char *value;
    size_t valueSize;
} Log4JEventProperty;

#pragma pack (pop)

LOG4JPARSERC_API size_t Log4JEventProperties (const Log4JEvent log4JEvent, size_t skip, Log4JEventProperty *properties, size_t propertiesSize);

// Event Source

typedef struct Log4JEventSource_ Log4JEventSource;

typedef enum
{
    E_SUCCESS = 0,
    E_MEMORY_ERROR = 10,
    E_DOCUMENT_ERRORS = 20,
} Log4JStatus;

LOG4JPARSERC_API Log4JStatus Log4JEventSourceInitXmlString (Log4JEventSource **self, char *xmlString);

LOG4JPARSERC_API void Log4JEventSourceDestroy (Log4JEventSource *self);

LOG4JPARSERC_API Log4JEvent Log4JEventSourceFirst (const Log4JEventSource *self);

LOG4JPARSERC_API Log4JEvent Log4JEventSourceNext (const Log4JEventSource *self, Log4JEvent event);

LOG4JPARSERC_API Log4JEvent Log4JEventSourceLast (const Log4JEventSource *self);

LOG4JPARSERC_API Log4JEvent Log4JEventSourcePrev (const Log4JEventSource *self, const Log4JEvent event);

// Filter base

typedef struct Log4JFilter_ Log4JFilter;

LOG4JPARSERC_API void Log4JFilterDestroy (Log4JFilter *self);

LOG4JPARSERC_API bool Log4JFilterApply (const Log4JFilter *self, const Log4JEvent event);

// Level filter

LOG4JPARSERC_API Log4JStatus Log4JFilterInitLevelC (Log4JFilter **self, const char *min, const char *max);

// Logger filter

LOG4JPARSERC_API Log4JStatus Log4JFilterInitLoggerFs (Log4JFilter **self, const char *logger, const size_t loggerSize);
LOG4JPARSERC_API Log4JStatus Log4JFilterInitLoggerNt (Log4JFilter **self, const char *logger);

// Message filter

LOG4JPARSERC_API Log4JStatus Log4JFilterInitMessageFs (Log4JFilter **self, const char *message, const size_t messageSize);
LOG4JPARSERC_API Log4JStatus Log4JFilterInitMessageNt (Log4JFilter **self, const char *message);

// Timestamp filter

LOG4JPARSERC_API Log4JStatus Log4JFilterInitTimestamp (Log4JFilter **self, int64_t min, int64_t max);

// All filter

LOG4JPARSERC_API Log4JStatus Log4JFilterInitAll (Log4JFilter **self);

LOG4JPARSERC_API Log4JStatus Log4JFilterAllAdd (Log4JFilter *self, const Log4JFilter *childFilter);
LOG4JPARSERC_API void Log4JFilterAllRemove (Log4JFilter *self, const Log4JFilter *childFilter);

// Any filter

LOG4JPARSERC_API Log4JStatus Log4JFilterInitAny (Log4JFilter **self);

LOG4JPARSERC_API Log4JStatus Log4JFilterAnyAdd (Log4JFilter *self, const Log4JFilter *childFilter);
LOG4JPARSERC_API void Log4JFilterAnyRemove (Log4JFilter *self, const Log4JFilter *childFilter);

// Not filter

LOG4JPARSERC_API Log4JStatus Log4JFilterInitNot (Log4JFilter **self, const Log4JFilter *childFilter);

// Iterators

typedef struct Log4JIterator_ Log4JIterator;

LOG4JPARSERC_API void Log4JIteratorDestroy (Log4JIterator *self);

LOG4JPARSERC_API bool Log4JIteratorMoveNext (Log4JIterator *self);

LOG4JPARSERC_API const Log4JEvent Log4JIteratorCurrent (const Log4JIterator *self, size_t *id);

LOG4JPARSERC_API void Log4JIteratorInitEventSource (Log4JIterator **self, const Log4JEventSource *source);

LOG4JPARSERC_API void Log4JIteratorInitEventSourceReverse (Log4JIterator **self, const Log4JEventSource *source);

LOG4JPARSERC_API void Log4JIteratorInitFilter (Log4JIterator **self, Log4JIterator *inner, const Log4JFilter *filter);

// Utilities

LOG4JPARSERC_API int32_t Log4JGetLevelValueFs (const char *level, size_t levelSize);
LOG4JPARSERC_API int32_t Log4JGetLevelValueNt (const char *level);

// Constants

LOG4JPARSERC_API void Log4JLevelAlert (const char **value);
LOG4JPARSERC_API void Log4JLevelAll (const char **value);
LOG4JPARSERC_API void Log4JLevelCritical (const char **value);
LOG4JPARSERC_API void Log4JLevelDebug (const char **value);
LOG4JPARSERC_API void Log4JLevelEmergency (const char **value);
LOG4JPARSERC_API void Log4JLevelError (const char **value);
LOG4JPARSERC_API void Log4JLevelFatal (const char **value);
LOG4JPARSERC_API void Log4JLevelFine (const char **value);
LOG4JPARSERC_API void Log4JLevelFiner (const char **value);
LOG4JPARSERC_API void Log4JLevelFinest (const char **value);
LOG4JPARSERC_API void Log4JLevelInfo (const char **value);
LOG4JPARSERC_API void Log4JLevelNotice (const char **value);
LOG4JPARSERC_API void Log4JLevelOff (const char **value);
LOG4JPARSERC_API void Log4JLevelSevere (const char **value);
LOG4JPARSERC_API void Log4JLevelTrace (const char **value);
LOG4JPARSERC_API void Log4JLevelVerbose (const char **value);
LOG4JPARSERC_API void Log4JLevelWarn (const char **value);
