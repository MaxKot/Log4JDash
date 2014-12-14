#include <stdlib.h>
#include <memory.h>
#include <stdbool.h>
#include <string.h>
#include "filter.h"
#include "LevelParser.h"

typedef bool FilterApplyCb_ (void *context, const Log4JEvent event);

typedef void FilterDestroyCb_ (void *context);

struct Filter_
{
    void *Context;

    FilterDestroyCb_ *Destroy;
    FilterApplyCb_ *Apply;
};

static void InitFilter_ (Filter *self, void *context, FilterDestroyCb_ *destroy, FilterApplyCb_ *apply)
{
    self->Context = context;
    self->Destroy = destroy;
    self->Apply = apply;
}

void FilterDestroy (Filter *self)
{
    self->Destroy (self->Context);
    free (self);
}

bool FilterApply (const Filter *self, const Log4JEvent event)
{
    return self->Apply (self->Context, event);
}

static bool FilterEquals_ (const Filter *x, const Filter *y)
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
} FilterLevelContext_;

static void FilterLevelDestroy_ (void *context);
static bool FilterLevelApply_ (void *context, const Log4JEvent event);

void FilterInitLevelI (Filter **self, int32_t min, int32_t max)
{
    FilterLevelContext_ *context = (FilterLevelContext_ *) malloc (sizeof (FilterLevelContext_));
    *context = (FilterLevelContext_ ) { .Min = min, .Max = max };

    Filter *result = (Filter *) malloc (sizeof (Filter));
    InitFilter_ (result, context, &FilterLevelDestroy_, &FilterLevelApply_);

    *self = result;
}

void FilterInitLevelC (Filter **self, const char *min, const char *max)
{
    int minI = GetLevelValue (min, INT_MAX);
    int maxI = GetLevelValue (max, INT_MAX);

    FilterInitLevelI (self, minI, maxI);
}

static void FilterLevelDestroy_ (void *context)
{
    FilterLevelContext_ *contextL = (FilterLevelContext_ *) context;

    *contextL = (FilterLevelContext_ ) { .Min = 0, .Max = 0 };
    free (contextL);
}

static bool FilterLevelApply_ (void *context, const Log4JEvent event)
{
    FilterLevelContext_ *contextL = (FilterLevelContext_ *) context;

    FixedString valString = Log4JEventLevel (event);
    int32_t value = GetLevelValue (valString.Value, valString.Size);

    return contextL->Min <= value && value <= contextL->Max;
}

#pragma endregion

#pragma region Logger filter

typedef struct
{
    char *Logger;
    size_t LoggerSize;
} FilterLoggerContext_;

static void FilterLoggerDestroy_ (void *context);
static bool FilterLoggerApply_ (void *context, const Log4JEvent event);

void FilterInitLoggerFs (Filter **self, const char *logger, const size_t loggerSize)
{
    FilterLoggerContext_ *context = (FilterLoggerContext_ *) malloc (sizeof (FilterLoggerContext_));
    // Reminder: logger_size must be multiplied by sizeof (Ch) if this code is reused for non-char
    // strings (e.g. wchar strings).
    char *contextLogger = (char *) malloc (loggerSize);
    memcpy (contextLogger, logger, loggerSize);
    *context = (FilterLoggerContext_ ) { .Logger = contextLogger, .LoggerSize = loggerSize };

    Filter *result = (Filter *) malloc (sizeof (Filter));
    InitFilter_ (result, context, &FilterLoggerDestroy_, &FilterLoggerApply_);

    *self = result;
}

void filterInitLoggerNt (Filter **self, const char *logger)
{
    size_t loggerSize = strlen (logger);
    FilterInitLoggerFs (self, logger, loggerSize);
}

static void FilterLoggerDestroy_ (void *context)
{
    FilterLoggerContext_ *contextL = (FilterLoggerContext_ *) context;

    free (contextL->Logger);
    *contextL = (FilterLoggerContext_) { .Logger = NULL, .LoggerSize = 0 };

    free (context);
}

static bool FilterLoggerApply_ (void *context, const Log4JEvent event)
{
    FilterLoggerContext_ *contextL = (FilterLoggerContext_ *) context;

    FixedString value = Log4JEventLogger (event);

    char *logger = contextL->Logger;
    size_t loggerSize = contextL->LoggerSize;

    return value.Size >= loggerSize &&
           _strnicmp (value.Value, logger, loggerSize) == 0;
}

#pragma endregion

#pragma region Message filter

typedef struct 
{
    char *Message;
    size_t MessageSize;
} FilterMessageContext_;

static void FilterMessageDestroy_ (void *context);
static bool FilterMessageApply_ (void *context, const Log4JEvent event);

void FilterInitMessageFs (Filter **self, const char *message, const size_t messageSize)
{
    FilterMessageContext_ *context = (FilterMessageContext_ *) malloc (sizeof (FilterMessageContext_));
    // Reminder: message_size must be multiplied by sizeof (Ch) if this code is reused for non-char
    // strings (e.g. wchar strings).
    char *contextMessage = (char *) malloc (messageSize);
    memcpy (contextMessage, message, messageSize);
    *context = (FilterMessageContext_) { .Message = contextMessage, .MessageSize = messageSize };

    Filter *result = (Filter *) malloc (sizeof (Filter));
    InitFilter_ (result, context, &FilterMessageDestroy_, &FilterMessageApply_);

    *self = result;
}

void FilterInitMessageNt (Filter **self, const char *message)
{
    size_t messageSize = strlen (message);
    FilterInitMessageFs (self, message, messageSize);
}

static void FilterMessageDestroy_ (void *context)
{
    FilterMessageContext_ *contextM = (FilterMessageContext_ *) context;

    free (contextM->Message);
    *contextM = (FilterMessageContext_) { .Message = NULL, .MessageSize = 0 };

    free (context);
}

static bool FilterMessageApply_ (void *context, const Log4JEvent event)
{
    FilterMessageContext_ *contextM = (FilterMessageContext_ *) context;

    if (contextM->MessageSize == 0)
    {
        return true;
    }

    FixedString message = Log4JEventMessage (event);
    size_t valueSize = message.Size;

    if (valueSize < contextM->MessageSize)
    {
        return false;
    }

    const char *value = message.Value;
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
} FilterTimestampContext_;

static void FilterTimestampDestroy_ (void *context);
static bool FilterTimestampApply_ (void *context, const Log4JEvent event);

void FilterInitTimestamp (Filter **self, int64_t min, int64_t max)
{
    FilterTimestampContext_ *context = (FilterTimestampContext_ *) malloc (sizeof (FilterTimestampContext_));
    *context = (FilterTimestampContext_) { .Min = min, .Max = max };

    Filter *result = (Filter *) malloc (sizeof (Filter));
    InitFilter_ (result, context, &FilterTimestampDestroy_, &FilterTimestampApply_);

    *self = result;
}

static void FilterTimestampDestroy_ (void *context)
{
    FilterTimestampContext_ *contextT = (FilterTimestampContext_ *) context;

    *contextT = (FilterTimestampContext_) { .Min = 0, .Max = 0 };
    free (contextT);
}

static bool FilterTimestampApply_ (void *context, const Log4JEvent event)
{
    FilterTimestampContext_ *contextT = (FilterTimestampContext_ *) context;

    int64_t value = Log4JEventTime (event);

    return contextT->Min <= value && value <= contextT->Max;
}

#pragma endregion

// Composite filters

typedef struct FilterEntry_
{
    Filter *Filter;
    struct FilterEntry_ *Next;
} FilterEntry_;

static void FilterListDestroy_ (FilterEntry_ *head)
{
    if (head->Next)
    {
        FilterListDestroy_ (head->Next);
    }
    *head = (FilterEntry_ ) { .Filter = NULL, .Next = NULL };

    free (head);
}

static FilterEntry_ *FilterListAdd_ (FilterEntry_ *head, Filter *filter)
{
    FilterEntry_ *result = (FilterEntry_ *) malloc (sizeof (FilterEntry_));
    *result = (FilterEntry_ ) { .Filter = filter, .Next = head };
    return result;
}

static FilterEntry_ *FilterListRemove_ (FilterEntry_ *head, Filter *filter)
{
    FilterEntry_ *current = head;
    FilterEntry_ **prevPtr = &head;

    while (current)
    {
        if (FilterEquals_ (current->Filter, filter))
        {
            *prevPtr = current->Next;
            *current = (FilterEntry_) { .Filter = NULL, .Next = NULL };
            free (current);

            break;
        }

        prevPtr = &(current->Next);
        current = current->Next;
    }

    return head;
}

#pragma region All filter

typedef struct
{
    FilterEntry_ *ChildrenHead;
} FilterAllContext_;

static void FilterAllDestroy_ (void *context);
static bool FilterAllApply (void *context, const Log4JEvent event);

void filter_init_all (Filter **self)
{
    FilterAllContext_ *context = (FilterAllContext_ *) malloc (sizeof (FilterAllContext_));
    *context = (FilterAllContext_ ) { .ChildrenHead = NULL };

    Filter *result = (Filter *) malloc (sizeof (Filter));
    InitFilter_ (result, context, &FilterAllDestroy_, &FilterAllApply);

    *self = result;
}

static void FilterAllDestroy_ (void *context)
{
    FilterAllContext_ *contextA = (FilterAllContext_ *) context;

    if (contextA->ChildrenHead)
    {
        FilterListDestroy_ (contextA->ChildrenHead);
    }
    *contextA = (FilterAllContext_) { .ChildrenHead = NULL };

    free (context);
}

void FilterAllAdd (Filter *self, Filter *child)
{
    FilterAllContext_ *context = (FilterAllContext_ *) self->Context;

    context->ChildrenHead = FilterListAdd_ (context->ChildrenHead, child);
}

void filterAllRemove (Filter *self, Filter *child)
{
    FilterAllContext_ *context = (FilterAllContext_ *) self->Context;

    context->ChildrenHead = FilterListRemove_ (context->ChildrenHead, child);
}

static bool FilterAllApply (void *context, const Log4JEvent event)
{
    FilterAllContext_ *contextA = (FilterAllContext_ *) context;

    FilterEntry_ *current = contextA->ChildrenHead;

    bool result = true;
    while (current && result)
    {
        result = result && FilterApply (current->Filter, event);
        current = current->Next;
    }

    return result;
}

#pragma endregion

#pragma region Any filter

typedef struct
{
    FilterEntry_ *ChildrenHead;
} FilterAnyContext_;

static void FilterAnyDestroy_ (void *context);
static bool FilterAnyApply_ (void *context, const Log4JEvent event);

void FilterInitAny (Filter **self)
{
    FilterAnyContext_ *context = (FilterAnyContext_ *) malloc (sizeof (FilterAnyContext_));
    *context = (FilterAnyContext_) { .ChildrenHead = NULL };

    Filter *result = (Filter *) malloc (sizeof (Filter));
    InitFilter_ (result, context, &FilterAnyDestroy_, &FilterAnyApply_);

    *self = result;
}

static void FilterAnyDestroy_ (void *context)
{
    FilterAnyContext_ *contextA = (FilterAnyContext_ *) context;

    if (contextA->ChildrenHead)
    {
        FilterListDestroy_ (contextA->ChildrenHead);
    }
    *contextA = (FilterAnyContext_) { .ChildrenHead = NULL };

    free (context);
}

void FilterAnyAdd (Filter *self, Filter *child)
{
    FilterAnyContext_ *context = (FilterAnyContext_ *) self->Context;

    context->ChildrenHead = FilterListAdd_ (context->ChildrenHead, child);
}

void FilterAnyRemove (Filter *self, Filter *child)
{
    FilterAnyContext_ *context = (FilterAnyContext_ *) self->Context;

    context->ChildrenHead = FilterListRemove_ (context->ChildrenHead, child);
}

static bool FilterAnyApply_ (void *context, const Log4JEvent event)
{
    FilterAnyContext_ *contextA = (FilterAnyContext_ *) context;

    FilterEntry_ *current = contextA->ChildrenHead;

    bool result = false;
    while (current && !result)
    {
        result = result || FilterApply (current->Filter, event);
        current = current->Next;
    }

    return result;
}

#pragma endregion

#pragma region Not filter

typedef struct
{
    Filter *ChildFilter;
} FilterNotContext_;

static void FilterNotDestroy_ (void *context);
static bool FilterNotApply_ (void *context, const Log4JEvent event);

void FilterInitNot (Filter **self, Filter *child_filter)
{
    FilterNotContext_ *context = (FilterNotContext_ *) malloc (sizeof (FilterNotContext_));
    *context = (FilterNotContext_ ) { .ChildFilter = child_filter };

    Filter *result = (Filter *) malloc (sizeof (Filter));
    InitFilter_ (result, context, &FilterNotDestroy_, &FilterNotApply_);

    *self = result;
}

static void FilterNotDestroy_ (void *context)
{
    FilterNotContext_ *contextN = (FilterNotContext_ *) context;

    *contextN = (FilterNotContext_ ) { .ChildFilter = NULL };

    free (context);
}

static bool FilterNotApply_ (void *context, const Log4JEvent event)
{
    FilterNotContext_ *contextN = (FilterNotContext_ *) context;

    return !FilterApply (contextN->ChildFilter, event);
}

#pragma endregion
