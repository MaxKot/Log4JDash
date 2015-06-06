#pragma once

#include <stdint.h>

#ifdef LOG4JPARSERC_EXPORTS
#define LOG4JPARSERC_API __declspec(dllexport)
#else
#define LOG4JPARSERC_API __declspec(dllimport)
#endif

// Event

typedef void *Log4JEvent;

LOG4JPARSERC_API void Log4JEventLevel (const Log4JEvent log4JEvent, const char **value, size_t *size);

LOG4JPARSERC_API void Log4JEventLogger (const Log4JEvent log4JEvent, const char **value, size_t *size);

LOG4JPARSERC_API void Log4JEventThread (const Log4JEvent log4JEvent, const char **value, size_t *size);

LOG4JPARSERC_API int64_t Log4JEventTimestamp (const Log4JEvent log4JEvent);

LOG4JPARSERC_API void Log4JEventMessage (const Log4JEvent log4JEvent, const char **value, size_t *size);

LOG4JPARSERC_API void Log4JEventThrowable (const Log4JEvent log4JEvent, const char **value, size_t *size);

// Event Source

typedef struct Log4JEventSource_ Log4JEventSource;

LOG4JPARSERC_API void Log4JEventSourceInitXmlString (Log4JEventSource **self, char *xmlString);

LOG4JPARSERC_API void Log4JEventSourceDestroy (Log4JEventSource *self);

LOG4JPARSERC_API Log4JEvent Log4JEventSourceFirst (const Log4JEventSource *self);

LOG4JPARSERC_API Log4JEvent Log4JEventSourceNext (const Log4JEventSource *self, Log4JEvent event);

// Filter base

typedef struct Log4JFilter_ Log4JFilter;

LOG4JPARSERC_API void Log4JFilterDestroy (Log4JFilter *self);

LOG4JPARSERC_API bool Log4JFilterApply (const Log4JFilter *self, const Log4JEvent event);

// Level filter

LOG4JPARSERC_API void Log4JFilterInitLevelI (Log4JFilter **self, int32_t min, int32_t max);
LOG4JPARSERC_API void Log4JFilterInitLevelC (Log4JFilter **self, const char *min, const char *max);

// Logger filter

LOG4JPARSERC_API void Log4JFilterInitLoggerFs (Log4JFilter **self, const char *logger, const size_t loggerSize);
LOG4JPARSERC_API void Log4JFilterInitLoggerNt (Log4JFilter **self, const char *logger);

// Message filter

LOG4JPARSERC_API void Log4JFilterInitMessageFs (Log4JFilter **self, const char *message, const size_t messageSize);
LOG4JPARSERC_API void Log4JFilterInitMessageNt (Log4JFilter **self, const char *message);

// Timestamp filter

LOG4JPARSERC_API void Log4JFilterInitTimestamp (Log4JFilter **self, int64_t min, int64_t max);

// All filter

LOG4JPARSERC_API void Log4JFilterInitAll (Log4JFilter **self);

LOG4JPARSERC_API void Log4JFilterAllAdd (Log4JFilter *self, const Log4JFilter *childFilter);
LOG4JPARSERC_API void Log4JFilterAllRemove (Log4JFilter *self, const Log4JFilter *childFilter);

// Any filter

LOG4JPARSERC_API void Log4JFilterInitAny (Log4JFilter **self);

LOG4JPARSERC_API void Log4JFilterAnyAdd (Log4JFilter *self, const Log4JFilter *childFilter);
LOG4JPARSERC_API void Log4JFilterAnyRemove (Log4JFilter *self, const Log4JFilter *childFilter);

// Not filter

LOG4JPARSERC_API void Log4JFilterInitNot (Log4JFilter **self, const Log4JFilter *childFilter);

// Iterators

typedef struct Log4JIterator_ Log4JIterator;

LOG4JPARSERC_API void Log4JIteratorDestroy (Log4JIterator *self);

LOG4JPARSERC_API bool Log4JIteratorMoveNext (Log4JIterator *self);

LOG4JPARSERC_API const Log4JEvent Log4JIteratorCurrent (const Log4JIterator *self, int32_t *id);

LOG4JPARSERC_API void Log4JIteratorInitEventSource (Log4JIterator **self, const Log4JEventSource *source);

LOG4JPARSERC_API void Log4JIteratorInitFilter (Log4JIterator **self, Log4JIterator *inner, const Log4JFilter *filter);

// Constants

LOG4JPARSERC_API int GetLevelAlert (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelAll (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelCritical (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelDebug (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelEmergency (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelError (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelFatal (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelFine (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelFiner (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelFinest (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelInfo (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelNotice (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelOff (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelSevere (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelTrace (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelVerbose (char *buffer, size_t bufferSize);
LOG4JPARSERC_API int GetLevelWarn (char *buffer, size_t bufferSize);
