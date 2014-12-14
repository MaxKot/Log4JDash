#include <stdlib.h>
#include <memory.h>
#include <string.h>
#include "filter.h"
#include "LevelParser.h"

typedef bool FilterApplyCb_ (void *context, const Log4JEvent event);

typedef void FilterDestroyCb_ (void *context);

struct Filter_
{
    void *Context;

    FilterApplyCb_ *Apply;
    FilterDestroyCb_ *Destroy;
};

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

struct FilterLevelContext_
{
    int32_t Min;
    int32_t Max;
};

static void FilterLevelDestroy_ (void *context);
static bool FilterLevelApply_ (void *context, const Log4JEvent event);

void FilterInitLevelI (Filter **self, int32_t min, int32_t max)
{
    auto context = (FilterLevelContext_ *) malloc (sizeof (FilterLevelContext_));
    *context = { min, max };

    auto result = (Filter *) malloc (sizeof (Filter));
    *result = { context, &FilterLevelApply_, &FilterLevelDestroy_ };

    *self = result;
}

void FilterInitLevelC (Filter **self, const char *min, const char *max)
{
    auto minI = GetLevelValue (min, INT_MAX);
    auto maxI = GetLevelValue (max, INT_MAX);

    FilterInitLevelI (self, minI, maxI);
}

static void FilterLevelDestroy_ (void *context)
{
    auto contextL = (FilterLevelContext_ *) context;

    *contextL = { 0, 0 };
    free (contextL);
}

static bool FilterLevelApply_ (void *context, const Log4JEvent event)
{
    auto contextL = (FilterLevelContext_ *) context;

    auto valString = Log4JEventLevel (event);
    int32_t value = GetLevelValue (valString.Value, valString.Size);

    return contextL->Min <= value && value <= contextL->Max;
}

#pragma endregion

#pragma region Logger filter

struct FilterLoggerContext_
{
    char *Logger;
    size_t LoggerSize;
};

static void FilterLoggerDestroy_ (void *context);
static bool FilterLoggerApply_ (void *context, const Log4JEvent event);

void FilterInitLoggerFs (Filter **self, const char *logger, const size_t loggerSize)
{
    auto context = (FilterLoggerContext_ *) malloc (sizeof (FilterLoggerContext_));
    // Reminder: logger_size must be multiplied by sizeof (Ch) if this code is reused for non-char
    // strings (e.g. wchar strings).
    auto contextLogger = (char *) malloc (loggerSize);
    memcpy (contextLogger, logger, loggerSize);
    *context = { contextLogger, loggerSize };

    auto result = (Filter *) malloc (sizeof (Filter));
    *result = { context, &FilterLoggerApply_, &FilterLoggerDestroy_ };

    *self = result;
}

void filterInitLoggerNt (Filter **self, const char *logger)
{
    auto loggerSize = strlen (logger);
    FilterInitLoggerFs (self, logger, loggerSize);
}

static void FilterLoggerDestroy_ (void *context)
{
    auto contextL = (FilterLoggerContext_ *) context;

    free (contextL->Logger);
    contextL->Logger = nullptr;
    contextL->LoggerSize = 0;

    free (context);
}

static bool FilterLoggerApply_ (void *context, const Log4JEvent event)
{
    auto contextL = (FilterLoggerContext_ *) context;

    auto value = Log4JEventLogger (event);

    auto logger = contextL->Logger;
    auto loggerSize = contextL->LoggerSize;

    return value.Size >= loggerSize &&
           _strnicmp (value.Value, logger, loggerSize) == 0;
}

#pragma endregion

#pragma region Message filter

struct FilterMessageContext_
{
    char *Message;
    size_t MessageSize;
};

static void FilterMessageDestroy_ (void *context);
static bool FilterMessageApply_ (void *context, const Log4JEvent event);

void FilterInitMessageFs (Filter **self, const char *message, const size_t messageSize)
{
    auto context = (FilterMessageContext_ *) malloc (sizeof (FilterMessageContext_));
    // Reminder: message_size must be multiplied by sizeof (Ch) if this code is reused for non-char
    // strings (e.g. wchar strings).
    auto contextMessage = (char *) malloc (messageSize);
    memcpy (contextMessage, message, messageSize);
    *context = { contextMessage, messageSize };

    auto result = (Filter *) malloc (sizeof (Filter));
    *result = { context, &FilterMessageApply_, &FilterMessageDestroy_ };

    *self = result;
}

void FilterInitMessageNt (Filter **self, const char *message)
{
    auto messageSize = strlen (message);
    FilterInitMessageFs (self, message, messageSize);
}

static void FilterMessageDestroy_ (void *context)
{
    auto contextM = (FilterMessageContext_ *) context;

    free (contextM->Message);
    contextM->Message = nullptr;
    contextM->MessageSize = 0;

    free (context);
}

static bool FilterMessageApply_ (void *context, const Log4JEvent event)
{
    auto contextM = (FilterMessageContext_ *) context;

    if (contextM->MessageSize == 0)
    {
        return true;
    }

    auto message = Log4JEventMessage (event);
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

struct FilterTimestampContext_
{
    int64_t Min;
    int64_t Max;
};

static void FilterTimestampDestroy_ (void *context);
static bool FilterTimestampApply_ (void *context, const Log4JEvent event);

void FilterInitTimestamp (Filter **self, int64_t min, int64_t max)
{
    auto context = (FilterTimestampContext_ *) malloc (sizeof (FilterTimestampContext_));
    *context = { min, max };

    auto result = (Filter *) malloc (sizeof (Filter));
    *result = { context, &FilterTimestampApply_, &FilterTimestampDestroy_ };

    *self = result;
}

static void FilterTimestampDestroy_ (void *context)
{
    auto contextT = (FilterTimestampContext_ *) context;

    *contextT = { 0, 0 };
    free (contextT);
}

static bool FilterTimestampApply_ (void *context, const Log4JEvent event)
{
    auto contextT = (FilterTimestampContext_ *) context;

    auto value = Log4JEventTime (event);

    return contextT->Min <= value && value <= contextT->Max;
}

#pragma endregion

// Composite filters

struct FilterEntry_
{
    Filter *Filter;
    FilterEntry_ *Next;
};

static void FilterListDestroy_ (FilterEntry_ *head)
{
    if (head->Next)
    {
        FilterListDestroy_ (head->Next);
    }
    *head = { nullptr, nullptr };

    free (head);
}

static FilterEntry_ *FilterListAdd_ (FilterEntry_ *head, Filter *filter)
{
    auto result = (FilterEntry_ *) malloc (sizeof (FilterEntry_));
    *result = { filter, head };
    return result;
}

static FilterEntry_ *FilterListRemove_ (FilterEntry_ *head, Filter *filter)
{
    auto current = head;
    auto prevPtr = &head;

    while (current)
    {
        if (FilterEquals_ (current->Filter, filter))
        {
            *prevPtr = current->Next;
            *current = { nullptr, nullptr };
            free (current);

            break;
        }

        prevPtr = &(current->Next);
        current = current->Next;
    }

    return head;
}

#pragma region All filter

struct FilterAllContext_
{
    FilterEntry_ *ChildrenHead;
};

static void FilterAllDestroy_ (void *context);
static bool FilterAllApply (void *context, const Log4JEvent event);

void filter_init_all (Filter **self)
{
    auto context = (FilterAllContext_ *) malloc (sizeof (FilterAllContext_));
    *context = { nullptr };

    auto result = (Filter *) malloc (sizeof (Filter));
    *result = { context, &FilterAllApply, &FilterAllDestroy_ };

    *self = result;
}

static void FilterAllDestroy_ (void *context)
{
    auto contextA = (FilterAllContext_ *) context;

    if (contextA->ChildrenHead)
    {
        FilterListDestroy_ (contextA->ChildrenHead);
    }
    contextA = { nullptr };

    free (context);
}

void FilterAllAdd (Filter *self, Filter *child)
{
    auto context = (FilterAllContext_ *) self->Context;

    context->ChildrenHead = FilterListAdd_ (context->ChildrenHead, child);
}

void filterAllRemove (Filter *self, Filter *child)
{
    auto context = (FilterAllContext_ *) self->Context;

    context->ChildrenHead = FilterListRemove_ (context->ChildrenHead, child);
}

static bool FilterAllApply (void *context, const Log4JEvent event)
{
    auto contextA = (FilterAllContext_ *) context;

    auto current = contextA->ChildrenHead;

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

struct FilterAnyContext_
{
    FilterEntry_ *ChildrenHead;
};

static void FilterAnyDestroy_ (void *context);
static bool FilterAnyApply_ (void *context, const Log4JEvent event);

void FilterInitAny (Filter **self)
{
    auto context = (FilterAnyContext_ *) malloc (sizeof (FilterAnyContext_));
    *context = { nullptr };

    auto result = (Filter *) malloc (sizeof (Filter));
    *result = { context, &FilterAnyApply_, &FilterAnyDestroy_ };

    *self = result;
}

static void FilterAnyDestroy_ (void *context)
{
    auto contextA = (FilterAnyContext_ *) context;

    if (contextA->ChildrenHead)
    {
        FilterListDestroy_ (contextA->ChildrenHead);
    }
    contextA = { nullptr };

    free (context);
}

void FilterAnyAdd (Filter *self, Filter *child)
{
    auto context = (FilterAnyContext_ *) self->Context;

    context->ChildrenHead = FilterListAdd_ (context->ChildrenHead, child);
}

void FilterAnyRemove (Filter *self, Filter *child)
{
    auto context = (FilterAnyContext_ *) self->Context;

    context->ChildrenHead = FilterListRemove_ (context->ChildrenHead, child);
}

static bool FilterAnyApply_ (void *context, const Log4JEvent event)
{
    auto contextA = (FilterAnyContext_ *) context;

    auto current = contextA->ChildrenHead;

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

struct FilterNotContext_
{
    Filter *ChildFilter;
};

static void FilterNotDestroy_ (void *context);
static bool FilterNotApply_ (void *context, const Log4JEvent event);

void FilterInitNot (Filter **self, Filter *child_filter)
{
    auto context = (FilterNotContext_ *) malloc (sizeof (FilterNotContext_));
    *context = { child_filter };

    auto result = (Filter *) malloc (sizeof (Filter));
    *result = { context, &FilterNotApply_, &FilterNotDestroy_ };

    *self = result;
}

static void FilterNotDestroy_ (void *context)
{
    auto contextN = (FilterNotContext_ *) context;

    *contextN = { nullptr };

    free (context);
}

static bool FilterNotApply_ (void *context, const Log4JEvent event)
{
    auto contextN = (FilterNotContext_ *) context;

    return !FilterApply (contextN->ChildFilter, event);
}

#pragma endregion
