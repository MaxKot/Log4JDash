#include <stdlib.h>
#include <memory.h>
#include <stdbool.h>
#include "Log4JIterator.h"

typedef void Log4JIteratorDestroyCb (void *context);

typedef bool Log4JIteratorMoveNextCb (void *context);

typedef const Log4JEvent Log4JIteratorCurrentCb (const void *context);

struct Log4JIterator_
{
    void *Context;

    Log4JIteratorDestroyCb *Destroy;
    Log4JIteratorMoveNextCb *MoveNext;
    Log4JIteratorCurrentCb *Current;
};

void Log4JIteratorDestroy (Log4JIterator *self)
{
    self->Destroy (self->Context);
    free (self);
}

bool Log4JIteratorMoveNext (Log4JIterator *self)
{
    return self->MoveNext (self->Context);
}

const Log4JEvent Log4JIteratorCurrent (const Log4JIterator *self)
{
    return self->Current (self->Context);
}

#pragma region XML string log4j_iterator

typedef struct
{
    const Log4JEventSource *Source;
    bool Start;
    Log4JEvent Current;
} Log4JIteratorEventSourceContext_;

static void Log4JIteratorEventSourceDestroy (void *context);
static bool Log4JIteratorEventSourceMoveNext (void *context);
static const Log4JEvent Log4JIteratorEventSourceCurrent (const void *context);

void Log4JIteratorInitEventSource (Log4JIterator **self, const Log4JEventSource *source)
{
    Log4JIteratorEventSourceContext_ *context = (Log4JIteratorEventSourceContext_ *) malloc (sizeof (Log4JIteratorEventSourceContext_));
    *context = (Log4JIteratorEventSourceContext_ ) { .Source = source, .Start = true, .Current = NULL };

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

    *contextD = (Log4JIteratorEventSourceContext_) { .Source = NULL, .Start = false, .Current = NULL };
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

    if (contextD->Start)
    {
        nextEvent = Log4JEventSourceFirst (contextD->Source);
        contextD->Start = false;
    }
    else if (contextD->Current)
    {
        nextEvent = Log4JEventSourceNext (contextD->Source, contextD->Current);
    }
    else
    {
        nextEvent = NULL;
    }

    contextD->Current = nextEvent;
    return nextEvent != NULL;
}

const Log4JEvent Log4JIteratorEventSourceCurrent (const void *context)
{
    Log4JIteratorEventSourceContext_ *contextD = (const Log4JIteratorEventSourceContext_ *) context;
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
static const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context);

void log4j_iterator_init_filter (Log4JIterator **self, Log4JIterator *inner, const Log4JFilter *filter)
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
        Log4JEvent event = Log4JIteratorCurrent (contextF->Inner);
        if (FilterApply (contextF->Filter, &event))
        {
            return true;
        }
    }

    return false;
}

const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context)
{
    Log4JIteratorFilterContext_ *contextF = (Log4JIteratorFilterContext_ *) context;

    return Log4JIteratorCurrent (contextF->Inner);
}

#pragma endregion
