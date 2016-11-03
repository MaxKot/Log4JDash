#include <assert.h>
#include <memory.h>
#include <stdbool.h>
#include <stdlib.h>
#include "Log4JParserC.h"
#include "Private.h"

typedef void Log4JIteratorDestroyCb (void *context);

typedef bool Log4JIteratorMoveNextCb (void *context);

typedef const Log4JEvent Log4JIteratorCurrentCb (const void *context, size_t *id);

#ifdef _DEBUG
    typedef enum { Undefined = 0, EventSourceBase, EventSource, EventSourceReverse, Filter } IteratorType;
#endif

struct Log4JIterator_
{
#ifdef _DEBUG
    IteratorType Type;
#endif
    void *Context;

    Log4JIteratorDestroyCb *Destroy;
    Log4JIteratorMoveNextCb *MoveNext;
    Log4JIteratorCurrentCb *Current;
};

static void Log4JIteratorInitEventSourceBase_ (Log4JIterator **self, const Log4JEventSource *source);
static void Log4JIteratorEventSourceBaseDestroy_ (void *context);
static const Log4JEvent Log4JIteratorEventSourceBaseCurrent_ (const void *context, size_t *id);

static bool Log4JIteratorEventSourceMoveNext_ (void *context);
static bool Log4JIteratorEventSourceReverseMoveNext_ (void *context);

static void Log4JIteratorFilterDestroy_ (void *context);
static bool Log4JIteratorFilterMoveNext_ (void *context);
static const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context, size_t *id);

#ifdef _DEBUG
    static void AssertLog4JIteratorSanityImpl_ (const Log4JIterator *self)
    {
        assert (self != NULL);
        assert (self->Context != NULL);
        assert (self->Type == EventSource || self->Type == EventSourceReverse || self->Type == Filter);
        assert (self->Type != EventSource || self->Current == Log4JIteratorEventSourceBaseCurrent_);
        assert (self->Type != EventSourceReverse || self->Current == Log4JIteratorEventSourceBaseCurrent_);
        assert (self->Type != Filter || self->Current == Log4JIteratorFilterCurrent_);
        assert (self->Type != EventSource || self->MoveNext == Log4JIteratorEventSourceMoveNext_);
        assert (self->Type != EventSourceReverse || self->MoveNext == Log4JIteratorEventSourceReverseMoveNext_);
        assert (self->Type != Filter || self->MoveNext == Log4JIteratorFilterMoveNext_);
        assert (self->Type != EventSource || self->Destroy == Log4JIteratorEventSourceBaseDestroy_);
        assert (self->Type != EventSourceReverse || self->Destroy == Log4JIteratorEventSourceBaseDestroy_);
        assert (self->Type != Filter || self->Destroy == Log4JIteratorFilterDestroy_);
    }

    #define AssertLog4JIteratorSanity_(v) AssertLog4JIteratorSanityImpl_ (v)
#else
    #define AssertLog4JIteratorSanity_(v)
#endif

LOG4JPARSERC_API void Log4JIteratorDestroy (Log4JIterator *self)
{
    AssertLog4JIteratorSanity_ (self);

    self->Destroy (self->Context);

    *self = (Log4JIterator)
    {
        #ifdef _DEBUG
            .Type = -1,
        #endif
        .Context = NULL,
        .Destroy = NULL,
        .MoveNext = NULL,
        .Current = NULL
    };
    free (self);
}

LOG4JPARSERC_API bool Log4JIteratorMoveNext (Log4JIterator *self)
{
    AssertLog4JIteratorSanity_ (self);

    bool result = self->MoveNext(self->Context);

    AssertLog4JIteratorSanity_ (self);
    return result;
}

LOG4JPARSERC_API const Log4JEvent Log4JIteratorCurrent (const Log4JIterator *self, size_t *id)
{
    AssertLog4JIteratorSanity_ (self);

    Log4JEvent result = self->Current (self->Context, id);

    AssertLog4JIteratorSanity_ (self);
    return result;
}

#pragma region Event source log4j_iterator base

typedef struct
{
    const Log4JEventSource *Source;
    Log4JEvent Current;
    Log4JEvent First;
} Log4JIteratorEventSourceBaseContext_;

void Log4JIteratorInitEventSourceBase_ (Log4JIterator **self, const Log4JEventSource *source)
{
    Log4JIteratorEventSourceBaseContext_ *context = (Log4JIteratorEventSourceBaseContext_ *) malloc (sizeof (*context));
    *context = (Log4JIteratorEventSourceBaseContext_)
    {
        .Source = source,
        .Current = NULL,
        .First = NULL
    };

    Log4JIterator *result = (Log4JIterator *) malloc (sizeof *result);
    *result = (Log4JIterator)
    {
        #ifdef _DEBUG
            .Type = EventSourceBase,
        #endif
        .Context = context,
        .Destroy = &Log4JIteratorEventSourceBaseDestroy_,
        .MoveNext = NULL,
        .Current = &Log4JIteratorEventSourceBaseCurrent_
    };

    *self = result;
}

void Log4JIteratorEventSourceBaseDestroy_ (void *context)
{
    Log4JIteratorEventSourceBaseContext_ *contextD = (Log4JIteratorEventSourceBaseContext_ *) context;

    *contextD = (Log4JIteratorEventSourceBaseContext_)
    {
        .Source = NULL,
        .Current = NULL,
        .First = NULL
    };
    free (contextD);
}

const Log4JEvent Log4JIteratorEventSourceBaseCurrent_ (const void *context, size_t *id)
{
    const Log4JIteratorEventSourceBaseContext_ *contextD = (const Log4JIteratorEventSourceBaseContext_ *) context;
    if (id != NULL && contextD->Current && contextD->First)
    {
        size_t sizeDump;

        const char *currentLogger;
        Log4JEventLogger (contextD->Current, &currentLogger, &sizeDump);

        const char *firstLogger;
        Log4JEventLogger (contextD->First, &firstLogger, &sizeDump);

        *id = currentLogger - firstLogger;
    }

    return contextD->Current;
}

#pragma endregion

#pragma region Event source log4j_iterator

LOG4JPARSERC_API void Log4JIteratorInitEventSource (Log4JIterator **self, const Log4JEventSource *source)
{
    Log4JIteratorInitEventSourceBase_ (self, source);
#ifdef _DEBUG
    (*self)->Type = EventSource;
#endif
    (*self)->MoveNext = &Log4JIteratorEventSourceMoveNext_;
}

bool Log4JIteratorEventSourceMoveNext_ (void *context)
{
    Log4JIteratorEventSourceBaseContext_ *contextD = (Log4JIteratorEventSourceBaseContext_ *) context;

    if (!contextD->Source)
    {
        return false;
    }

    Log4JEvent nextEvent;

    if (contextD->First == NULL)
    {
        nextEvent = Log4JEventSourceFirst (contextD->Source);
        contextD->First = nextEvent;
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

#pragma endregion

#pragma region Reverse event source log4j_iterator

LOG4JPARSERC_API void Log4JIteratorInitEventSourceReverse (Log4JIterator **self, const Log4JEventSource *source)
{
    Log4JIteratorInitEventSourceBase_ (self, source);
#ifdef _DEBUG
    (*self)->Type = EventSourceReverse;
#endif
    (*self)->MoveNext = &Log4JIteratorEventSourceReverseMoveNext_;
}

bool Log4JIteratorEventSourceReverseMoveNext_ (void *context)
{
    Log4JIteratorEventSourceBaseContext_ *contextD = (Log4JIteratorEventSourceBaseContext_ *) context;

    if (!contextD->Source)
    {
        return false;
    }

    Log4JEvent nextEvent;

    if (contextD->First == NULL)
    {
        nextEvent = Log4JEventSourceLast (contextD->Source);
        contextD->First = Log4JEventSourceFirst (contextD->Source);
    }
    else if (contextD->Current)
    {
        nextEvent = Log4JEventSourcePrev (contextD->Source, contextD->Current);
    }
    else
    {
        nextEvent = NULL;
    }

    contextD->Current = nextEvent;
    return nextEvent != NULL;
}

#pragma endregion

#pragma region Filtering iterator

typedef struct
{
    Log4JIterator *Inner;
    const Log4JFilter *Filter;
} Log4JIteratorFilterContext_;

LOG4JPARSERC_API void Log4JIteratorInitFilter (Log4JIterator **self, Log4JIterator *inner, const Log4JFilter *filter)
{
    Log4JIteratorFilterContext_ *context = (Log4JIteratorFilterContext_ *) malloc (sizeof *context);
    *context = (Log4JIteratorFilterContext_) { .Inner = inner, .Filter = filter };

    Log4JIterator *result = (Log4JIterator *) malloc (sizeof *result);
    *result = (Log4JIterator)
    {
        #ifdef _DEBUG
            .Type = Filter,
        #endif
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

const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context, size_t *id)
{
    Log4JIteratorFilterContext_ *contextF = (Log4JIteratorFilterContext_ *) context;

    return Log4JIteratorCurrent (contextF->Inner, id);
}

#pragma endregion
