#include <stdlib.h>
#include <memory.h>
#include <stdbool.h>
#include "Log4JParserC.h"
#include "Private.h"

typedef void Log4JIteratorDestroyCb (void *context);

typedef bool Log4JIteratorMoveNextCb (void *context);

typedef const Log4JEvent Log4JIteratorCurrentCb (const void *context, int32_t *id);

struct Log4JIterator_
{
    void *Context;

    Log4JIteratorDestroyCb *Destroy;
    Log4JIteratorMoveNextCb *MoveNext;
    Log4JIteratorCurrentCb *Current;
};

LOG4JPARSERC_API void Log4JIteratorDestroy (Log4JIterator *self)
{
    self->Destroy (self->Context);
    free (self);
}

LOG4JPARSERC_API bool Log4JIteratorMoveNext (Log4JIterator *self)
{
    bool result = self->MoveNext(self->Context);
    return result;
}

LOG4JPARSERC_API const Log4JEvent Log4JIteratorCurrent (const Log4JIterator *self, int32_t *id)
{
    return self->Current (self->Context, id);
}

#pragma region XML string log4j_iterator

typedef struct
{
    const Log4JEventSource *Source;
    Log4JEvent Current;
    int32_t Count;
} Log4JIteratorEventSourceContext_;

static void Log4JIteratorEventSourceDestroy (void *context);
static bool Log4JIteratorEventSourceMoveNext (void *context);
static const Log4JEvent Log4JIteratorEventSourceCurrent (const void *context, int32_t *id);

LOG4JPARSERC_API void Log4JIteratorInitEventSource (Log4JIterator **self, const Log4JEventSource *source)
{
    Log4JIteratorEventSourceContext_ *context = (Log4JIteratorEventSourceContext_ *) malloc (sizeof (Log4JIteratorEventSourceContext_));
    *context = (Log4JIteratorEventSourceContext_ )
    {
        .Source = source,
        .Current = NULL,
        .Count = 0
    };

    Log4JIterator *result = (Log4JIterator *) malloc (sizeof (Log4JIterator));
    *result = (Log4JIterator)
    {
        .Context = context,
        .Destroy = &Log4JIteratorEventSourceDestroy,
        .MoveNext = &Log4JIteratorEventSourceMoveNext,
        .Current = &Log4JIteratorEventSourceCurrent
    };

    *self = result;
}

void Log4JIteratorEventSourceDestroy (void *context)
{
    Log4JIteratorEventSourceContext_ *contextD = (Log4JIteratorEventSourceContext_ *) context;

    *contextD = (Log4JIteratorEventSourceContext_)
    {
        .Source = NULL,
        .Current = NULL,
        .Count = 0
    };
    free (contextD);
}

bool Log4JIteratorEventSourceMoveNext (void *context)
{
    Log4JIteratorEventSourceContext_ *contextD = (Log4JIteratorEventSourceContext_ *) context;

    if (!contextD->Source)
    {
        return false;
    }

    Log4JEvent nextEvent;

    if (contextD->Count == 0)
    {
        nextEvent = Log4JEventSourceFirst (contextD->Source);
        contextD->Count = 1;
    }
    else if (contextD->Current)
    {
        nextEvent = Log4JEventSourceNext (contextD->Source, contextD->Current);
        ++contextD->Count;
    }
    else
    {
        nextEvent = NULL;
    }

    contextD->Current = nextEvent;
    return nextEvent != NULL;
}

const Log4JEvent Log4JIteratorEventSourceCurrent (const void *context, int32_t *id)
{
    const Log4JIteratorEventSourceContext_ *contextD = (const Log4JIteratorEventSourceContext_ *) context;
    if (id != NULL)
    {
        *id = contextD->Count;
    }

    return contextD->Current;
}

#pragma endregion

#pragma region Filtering iterator

typedef struct
{
    Log4JIterator *Inner;
    const Log4JFilter *Filter;
} Log4JIteratorFilterContext_;

static void Log4JIteratorFilterDestroy_ (void *context);
static bool Log4JIteratorFilterMoveNext_ (void *context);
static const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context, int32_t *id);

LOG4JPARSERC_API void Log4JIteratorInitFilter (Log4JIterator **self, Log4JIterator *inner, const Log4JFilter *filter)
{
    Log4JIteratorFilterContext_ *context = (Log4JIteratorFilterContext_ *) malloc (sizeof (Log4JIteratorFilterContext_));
    *context = (Log4JIteratorFilterContext_) { .Inner = inner, .Filter = filter };

    Log4JIterator *result = (Log4JIterator *) malloc (sizeof (Log4JIterator));
    *result = (Log4JIterator)
    {
        .Context = context,
        .Destroy = &Log4JIteratorFilterDestroy_,
        .MoveNext = &Log4JIteratorFilterMoveNext_,
        .Current = &Log4JIteratorFilterCurrent_
    };

    *self = result;
}

void Log4JIteratorFilterDestroy_ (void *context)
{
    Log4JIteratorFilterContext_ *contextF = (Log4JIteratorFilterContext_ *) context;

    *contextF = (Log4JIteratorFilterContext_ ) { .Inner = NULL, .Filter = NULL };
    free (contextF);
}

bool Log4JIteratorFilterMoveNext_ (void *context)
{
    Log4JIteratorFilterContext_ *contextF = (Log4JIteratorFilterContext_ *) context;

    while (Log4JIteratorMoveNext (contextF->Inner))
    {
        Log4JEvent event = Log4JIteratorCurrent (contextF->Inner, NULL);
        if (Log4JFilterApply (contextF->Filter, event))
        {
            return true;
        }
    }

    return false;
}

const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context, int32_t *id)
{
    Log4JIteratorFilterContext_ *contextF = (Log4JIteratorFilterContext_ *) context;

    return Log4JIteratorCurrent (contextF->Inner, id);
}

#pragma endregion
