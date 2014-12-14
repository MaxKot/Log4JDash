#include <stdlib.h>
#include <memory.h>
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

struct Log4JIteratorEventSourceContext_
{
    const Log4JEventSource *Source;
    bool Start;
    Log4JEvent Current;
};

static void Log4JIteratorEventSourceDestroy (void *context);
static bool Log4JIteratorEventSourceMoveNext (void *context);
static const Log4JEvent Log4JIteratorEventSourceCurrent (const void *context);

void Log4JIteratorInitEventSource (Log4JIterator **self, const Log4JEventSource *source)
{
    auto context = (Log4JIteratorEventSourceContext_ *) malloc (sizeof (Log4JIteratorEventSourceContext_));
    *context = { source, true, nullptr };

    auto result = (Log4JIterator *) malloc (sizeof (Log4JIterator));
    *result =
    {
        context,
        &Log4JIteratorEventSourceDestroy,
        &Log4JIteratorEventSourceMoveNext,
        &Log4JIteratorEventSourceCurrent
    };

    *self = result;
}

void Log4JIteratorEventSourceDestroy (void *context)
{
    auto contextD = (Log4JIteratorEventSourceContext_ *) context;

    *contextD = { nullptr, false, nullptr };
    free (contextD);
}

bool Log4JIteratorEventSourceMoveNext (void *context)
{
    auto contextD = (Log4JIteratorEventSourceContext_ *) context;

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
        nextEvent = nullptr;
    }

    contextD->Current = nextEvent;
    return nextEvent != nullptr;
}

const Log4JEvent Log4JIteratorEventSourceCurrent (const void *context)
{
    auto contextD = (const Log4JIteratorEventSourceContext_ *) context;
    return contextD->Current;
}

#pragma endregion

#pragma region Filtering iterator

struct Log4JIteratorFilterContext
{
    Log4JIterator *Inner;
    const Filter *Filter;
};

static void Log4JIteratorFilterDestroy_ (void *context);
static bool Log4JIteratorFilterMoveNext_ (void *context);
static const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context);

void log4j_iterator_init_filter (Log4JIterator **self, Log4JIterator *inner, const Filter *filter)
{
    auto context = (Log4JIteratorFilterContext *) malloc (sizeof (Log4JIteratorFilterContext));
    *context = { inner, filter };

    auto result = (Log4JIterator *) malloc (sizeof (Log4JIterator));
    *result =
    {
        context,
        &Log4JIteratorFilterDestroy_,
        &Log4JIteratorFilterMoveNext_,
        &Log4JIteratorFilterCurrent_
    };

    *self = result;
}

void Log4JIteratorFilterDestroy_ (void *context)
{
    auto contextF = (Log4JIteratorFilterContext *) context;

    *contextF = { nullptr, nullptr };
    free (contextF);
}

bool Log4JIteratorFilterMoveNext_ (void *context)
{
    auto contextF = (Log4JIteratorFilterContext *) context;

    while (Log4JIteratorMoveNext (contextF->Inner))
    {
        auto event = Log4JIteratorCurrent (contextF->Inner);
        if (FilterApply (contextF->Filter, &event))
        {
            return true;
        }
    }

    return false;
}

const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context)
{
    auto contextF = (Log4JIteratorFilterContext *) context;

    return Log4JIteratorCurrent (contextF->Inner);
}

#pragma endregion
