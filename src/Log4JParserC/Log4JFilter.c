#include <stdlib.h>
#include <memory.h>
#include <stdbool.h>
#include <string.h>
#include "Log4JParserC.h"
#include "LevelParser.h"

typedef bool Log4JFilterApplyCb_ (void *context, const Log4JEvent event);

typedef void Log4JFilterDestroyCb_ (void *context);

struct Log4JFilter_
{
    void *Context;

    Log4JFilterDestroyCb_ *Destroy;
    Log4JFilterApplyCb_ *Apply;
};

static void InitLog4JFilter_ (Log4JFilter *self, void *context, Log4JFilterDestroyCb_ *destroy, Log4JFilterApplyCb_ *apply)
{
    self->Context = context;
    self->Destroy = destroy;
    self->Apply = apply;
}

LOG4JPARSERC_API void Log4JFilterDestroy (Log4JFilter *self)
{
    self->Destroy (self->Context);
    *self = (Log4JFilter)
    {
        .Context = NULL,
        .Destroy = NULL,
        .Apply = NULL
    };
    free (self);
}

LOG4JPARSERC_API bool Log4JFilterApply (const Log4JFilter *self, const Log4JEvent event)
{
    return self->Apply (self->Context, event);
}

static bool Log4JFilterEquals_ (const Log4JFilter *x, const Log4JFilter *y)
{
    return x->Context == y->Context &&
           x->Apply == y->Apply &&
           x->Destroy == y->Destroy;
}

#pragma region Level lilter

typedef struct
{
    int32_t Min;
    int32_t Max;
} Log4JFilterLevelContext_;

static void Log4JFilterLevelDestroy_ (void *context);
static bool Log4JFilterLevelApply_ (void *context, const Log4JEvent event);

static void Log4JFilterInitLevelI (Log4JFilter **self, int32_t min, int32_t max)
{
    Log4JFilterLevelContext_ *context = (Log4JFilterLevelContext_ *) malloc (sizeof *context);
    *context = (Log4JFilterLevelContext_ ) { .Min = min, .Max = max };

    Log4JFilter *result = (Log4JFilter *) malloc (sizeof *result);
    InitLog4JFilter_ (result, context, &Log4JFilterLevelDestroy_, &Log4JFilterLevelApply_);

    *self = result;
}

LOG4JPARSERC_API void Log4JFilterInitLevelC (Log4JFilter **self, const char *min, const char *max)
{
    int minI = GetLevelValue (min, INT_MAX);
    int maxI = GetLevelValue (max, INT_MAX);

    Log4JFilterInitLevelI (self, minI, maxI);
}

static void Log4JFilterLevelDestroy_ (void *context)
{
    Log4JFilterLevelContext_ *contextL = (Log4JFilterLevelContext_ *) context;

    *contextL = (Log4JFilterLevelContext_ ) { .Min = 0, .Max = 0 };
    free (contextL);
}

static bool Log4JFilterLevelApply_ (void *context, const Log4JEvent event)
{
    Log4JFilterLevelContext_ *contextL = (Log4JFilterLevelContext_ *) context;

    const char *level;
    size_t levelSize;
    Log4JEventLevel (event, &level, &levelSize);
    int32_t value = GetLevelValue (level, levelSize);

    return contextL->Min <= value && value <= contextL->Max;
}

#pragma endregion

#pragma region Logger filter

typedef struct
{
    char *Logger;
    size_t LoggerSize;
} Log4JFilterLoggerContext_;

static void Log4JFilterLoggerDestroy_ (void *context);
static bool Log4JFilterLoggerApply_ (void *context, const Log4JEvent event);

LOG4JPARSERC_API void Log4JFilterInitLoggerFs (Log4JFilter **self, const char *logger, const size_t loggerSize)
{
    Log4JFilterLoggerContext_ *context = (Log4JFilterLoggerContext_ *) malloc (sizeof *context);
    char *contextLogger = (char *) malloc (loggerSize * sizeof logger[0]);
    memcpy (contextLogger, logger, loggerSize);
    *context = (Log4JFilterLoggerContext_ ) { .Logger = contextLogger, .LoggerSize = loggerSize };

    Log4JFilter *result = (Log4JFilter *) malloc (sizeof *result);
    InitLog4JFilter_ (result, context, &Log4JFilterLoggerDestroy_, &Log4JFilterLoggerApply_);

    *self = result;
}

LOG4JPARSERC_API void Log4JFilterInitLoggerNt (Log4JFilter **self, const char *logger)
{
    size_t loggerSize = strlen (logger);
    Log4JFilterInitLoggerFs (self, logger, loggerSize);
}

static void Log4JFilterLoggerDestroy_ (void *context)
{
    Log4JFilterLoggerContext_ *contextL = (Log4JFilterLoggerContext_ *) context;

    free (contextL->Logger);
    *contextL = (Log4JFilterLoggerContext_) { .Logger = NULL, .LoggerSize = 0 };

    free (context);
}

static bool Log4JFilterLoggerApply_ (void *context, const Log4JEvent event)
{
    Log4JFilterLoggerContext_ *contextL = (Log4JFilterLoggerContext_ *) context;

    const char *logger;
    size_t loggerSize;
    Log4JEventLogger (event, &logger, &loggerSize);

    return loggerSize >= contextL->LoggerSize &&
           _strnicmp (logger, contextL->Logger, contextL->LoggerSize) == 0;
}

#pragma endregion

#pragma region Message filter

typedef struct 
{
    char *Message;
    size_t MessageSize;
} Log4JFilterMessageContext_;

static void Log4JFilterMessageDestroy_ (void *context);
static bool Log4JFilterMessageApply_ (void *context, const Log4JEvent event);

LOG4JPARSERC_API void Log4JFilterInitMessageFs (Log4JFilter **self, const char *message, const size_t messageSize)
{
    Log4JFilterMessageContext_ *context = (Log4JFilterMessageContext_ *) malloc (sizeof *context);
    char *contextMessage = (char *) malloc (messageSize * sizeof message[0]);
    memcpy (contextMessage, message, messageSize);
    *context = (Log4JFilterMessageContext_) { .Message = contextMessage, .MessageSize = messageSize };

    Log4JFilter *result = (Log4JFilter *) malloc (sizeof *result);
    InitLog4JFilter_ (result, context, &Log4JFilterMessageDestroy_, &Log4JFilterMessageApply_);

    *self = result;
}

LOG4JPARSERC_API void Log4JFilterInitMessageNt (Log4JFilter **self, const char *message)
{
    size_t messageSize = strlen (message);
    Log4JFilterInitMessageFs (self, message, messageSize);
}

static void Log4JFilterMessageDestroy_ (void *context)
{
    Log4JFilterMessageContext_ *contextM = (Log4JFilterMessageContext_ *) context;

    free (contextM->Message);
    *contextM = (Log4JFilterMessageContext_) { .Message = NULL, .MessageSize = 0 };

    free (context);
}

static bool Log4JFilterMessageApply_ (void *context, const Log4JEvent event)
{
    Log4JFilterMessageContext_ *contextM = (Log4JFilterMessageContext_ *) context;

    if (contextM->MessageSize == 0)
    {
        return true;
    }

    const char *value;
    size_t valueSize;
    Log4JEventMessage (event, &value, &valueSize);

    if (valueSize < contextM->MessageSize)
    {
        return false;
    }

    const char messageHead = *contextM->Message;
    const char *messageTail = contextM->Message + 1;
    const size_t messageTailSize = contextM->MessageSize - 1;

    do
    {
        char sc;

        do {
            sc = *value;
            ++value;
            --valueSize;

            if (valueSize < messageTailSize)
            {
                return false;
            }
        } while (sc != messageHead);
    } while (strncmp (value, messageTail, messageTailSize) != 0);

    return true;
}

#pragma endregion

#pragma region Timestamp filter

typedef struct
{
    int64_t Min;
    int64_t Max;
} Log4JFilterTimestampContext_;

static void Log4JFilterTimestampDestroy_ (void *context);
static bool Log4JFilterTimestampApply_ (void *context, const Log4JEvent event);

LOG4JPARSERC_API void Log4JFilterInitTimestamp (Log4JFilter **self, int64_t min, int64_t max)
{
    Log4JFilterTimestampContext_ *context = (Log4JFilterTimestampContext_ *) malloc (sizeof *context);
    *context = (Log4JFilterTimestampContext_) { .Min = min, .Max = max };

    Log4JFilter *result = (Log4JFilter *) malloc (sizeof *result);
    InitLog4JFilter_ (result, context, &Log4JFilterTimestampDestroy_, &Log4JFilterTimestampApply_);

    *self = result;
}

static void Log4JFilterTimestampDestroy_ (void *context)
{
    Log4JFilterTimestampContext_ *contextT = (Log4JFilterTimestampContext_ *) context;

    *contextT = (Log4JFilterTimestampContext_) { .Min = 0, .Max = 0 };
    free (contextT);
}

static bool Log4JFilterTimestampApply_ (void *context, const Log4JEvent event)
{
    Log4JFilterTimestampContext_ *contextT = (Log4JFilterTimestampContext_ *) context;

    int64_t value = Log4JEventTimestamp (event);

    return contextT->Min <= value && value <= contextT->Max;
}

#pragma endregion

// Composite filters

#pragma region Log4JFilterList

typedef struct Log4JFilterEntry_
{
    const Log4JFilter *Filter;
    struct Log4JFilterEntry_ *Next;
} Log4JFilterEntry_;

static void Log4JFilterListDestroy_ (Log4JFilterEntry_ *head)
{
    if (head->Next)
    {
        Log4JFilterListDestroy_ (head->Next);
    }
    *head = (Log4JFilterEntry_ ) { .Filter = NULL, .Next = NULL };

    free (head);
}

static Log4JFilterEntry_ *Log4JFilterListAdd_ (Log4JFilterEntry_ *head, const Log4JFilter *filter)
{
    Log4JFilterEntry_ *result = (Log4JFilterEntry_ *) malloc (sizeof *result);
    *result = (Log4JFilterEntry_ ) { .Filter = filter, .Next = head };
    return result;
}

static Log4JFilterEntry_ *Log4JFilterListRemove_ (Log4JFilterEntry_ *head, const Log4JFilter *filter)
{
    Log4JFilterEntry_ *current = head;
    Log4JFilterEntry_ **prevPtr = &head;

    while (current)
    {
        if (Log4JFilterEquals_ (current->Filter, filter))
        {
            *prevPtr = current->Next;
            *current = (Log4JFilterEntry_) { .Filter = NULL, .Next = NULL };
            free (current);

            break;
        }

        prevPtr = &(current->Next);
        current = current->Next;
    }

    return head;
}

#pragma endregion

#pragma region All filter

typedef struct
{
    Log4JFilterEntry_ *ChildrenHead;
} Log4JFilterAllContext_;

static void Log4JFilterAllDestroy_ (void *context);
static bool Log4JFilterAllApply (void *context, const Log4JEvent event);

LOG4JPARSERC_API void Log4JFilterInitAll (Log4JFilter **self)
{
    Log4JFilterAllContext_ *context = (Log4JFilterAllContext_ *) malloc (sizeof *context);
    *context = (Log4JFilterAllContext_ ) { .ChildrenHead = NULL };

    Log4JFilter *result = (Log4JFilter *) malloc (sizeof *result);
    InitLog4JFilter_ (result, context, &Log4JFilterAllDestroy_, &Log4JFilterAllApply);

    *self = result;
}

static void Log4JFilterAllDestroy_ (void *context)
{
    Log4JFilterAllContext_ *contextA = (Log4JFilterAllContext_ *) context;

    if (contextA->ChildrenHead)
    {
        Log4JFilterListDestroy_ (contextA->ChildrenHead);
    }
    *contextA = (Log4JFilterAllContext_) { .ChildrenHead = NULL };

    free (context);
}

LOG4JPARSERC_API void Log4JFilterAllAdd (Log4JFilter *self, const Log4JFilter *child)
{
    Log4JFilterAllContext_ *context = (Log4JFilterAllContext_ *) self->Context;

    context->ChildrenHead = Log4JFilterListAdd_ (context->ChildrenHead, child);
}

LOG4JPARSERC_API void Log4JFilterAllRemove (Log4JFilter *self, const Log4JFilter *child)
{
    Log4JFilterAllContext_ *context = (Log4JFilterAllContext_ *) self->Context;

    context->ChildrenHead = Log4JFilterListRemove_ (context->ChildrenHead, child);
}

static bool Log4JFilterAllApply (void *context, const Log4JEvent event)
{
    Log4JFilterAllContext_ *contextA = (Log4JFilterAllContext_ *) context;

    Log4JFilterEntry_ *current = contextA->ChildrenHead;

    bool result = true;
    while (current && result)
    {
        result = result && Log4JFilterApply (current->Filter, event);
        current = current->Next;
    }

    return result;
}

#pragma endregion

#pragma region Any filter

typedef struct
{
    Log4JFilterEntry_ *ChildrenHead;
} Log4JFilterAnyContext_;

static void Log4JFilterAnyDestroy_ (void *context);
static bool Log4JFilterAnyApply_ (void *context, const Log4JEvent event);

LOG4JPARSERC_API void Log4JFilterInitAny (Log4JFilter **self)
{
    Log4JFilterAnyContext_ *context = (Log4JFilterAnyContext_ *) malloc (sizeof *context);
    *context = (Log4JFilterAnyContext_) { .ChildrenHead = NULL };

    Log4JFilter *result = (Log4JFilter *) malloc (sizeof *result);
    InitLog4JFilter_ (result, context, &Log4JFilterAnyDestroy_, &Log4JFilterAnyApply_);

    *self = result;
}

static void Log4JFilterAnyDestroy_ (void *context)
{
    Log4JFilterAnyContext_ *contextA = (Log4JFilterAnyContext_ *) context;

    if (contextA->ChildrenHead)
    {
        Log4JFilterListDestroy_ (contextA->ChildrenHead);
    }
    *contextA = (Log4JFilterAnyContext_) { .ChildrenHead = NULL };

    free (context);
}

LOG4JPARSERC_API void Log4JFilterAnyAdd (Log4JFilter *self, const Log4JFilter *child)
{
    Log4JFilterAnyContext_ *context = (Log4JFilterAnyContext_ *) self->Context;

    context->ChildrenHead = Log4JFilterListAdd_ (context->ChildrenHead, child);
}

LOG4JPARSERC_API void Log4JFilterAnyRemove (Log4JFilter *self, const Log4JFilter *child)
{
    Log4JFilterAnyContext_ *context = (Log4JFilterAnyContext_ *) self->Context;

    context->ChildrenHead = Log4JFilterListRemove_ (context->ChildrenHead, child);
}

static bool Log4JFilterAnyApply_ (void *context, const Log4JEvent event)
{
    Log4JFilterAnyContext_ *contextA = (Log4JFilterAnyContext_ *) context;

    Log4JFilterEntry_ *current = contextA->ChildrenHead;

    bool result = false;
    while (current && !result)
    {
        result = result || Log4JFilterApply (current->Filter, event);
        current = current->Next;
    }

    return result;
}

#pragma endregion

#pragma region Not filter

typedef struct
{
    const Log4JFilter *ChildFilter;
} Log4JFilterNotContext_;

static void Log4JFilterNotDestroy_ (void *context);
static bool Log4JFilterNotApply_ (void *context, const Log4JEvent event);

LOG4JPARSERC_API void Log4JFilterInitNot (Log4JFilter **self, const Log4JFilter *child_filter)
{
    Log4JFilterNotContext_ *context = (Log4JFilterNotContext_ *) malloc (sizeof *context);
    *context = (Log4JFilterNotContext_ ) { .ChildFilter = child_filter };

    Log4JFilter *result = (Log4JFilter *) malloc (sizeof *result);
    InitLog4JFilter_ (result, context, &Log4JFilterNotDestroy_, &Log4JFilterNotApply_);

    *self = result;
}

static void Log4JFilterNotDestroy_ (void *context)
{
    Log4JFilterNotContext_ *contextN = (Log4JFilterNotContext_ *) context;

    *contextN = (Log4JFilterNotContext_ ) { .ChildFilter = NULL };

    free (context);
}

static bool Log4JFilterNotApply_ (void *context, const Log4JEvent event)
{
    Log4JFilterNotContext_ *contextN = (Log4JFilterNotContext_ *) context;

    return !Log4JFilterApply (contextN->ChildFilter, event);
}

#pragma endregion
