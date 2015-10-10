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
    typedef enum { EventSource, EventSourceReverse, Filter } IteratorType;
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

static void Log4JIteratorEventSourceDestroy_ (void *context);
static bool Log4JIteratorEventSourceMoveNext_ (void *context);
static const Log4JEvent Log4JIteratorEventSourceCurrent_ (const void *context, size_t *id);

static void Log4JIteratorEventSourceReverseDestroy_ (void *context);
static bool Log4JIteratorEventSourceReverseMoveNext_ (void *context);
static const Log4JEvent Log4JIteratorEventSourceReverseCurrent_ (const void *context, size_t *id);

static void Log4JIteratorFilterDestroy_ (void *context);
static bool Log4JIteratorFilterMoveNext_ (void *context);
static const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context, size_t *id);

#ifdef _DEBUG
    static void AssertLog4JIteratorSanityImpl_ (const Log4JIterator *self)
    {
        assert (self != NULL);
        assert (self->Context != NULL);
        assert (self->Type == EventSource || self->Type == EventSourceReverse || self->Type == Filter);
        assert (self->Type != EventSource || self->Current == Log4JIteratorEventSourceCurrent_);
        assert (self->Type != EventSourceReverse || self->Current == Log4JIteratorEventSourceReverseCurrent_);
        assert (self->Type != Filter || self->Current == Log4JIteratorFilterCurrent_);
        assert (self->Type != EventSource || self->MoveNext == Log4JIteratorEventSourceMoveNext_);
        assert (self->Type != EventSourceReverse || self->MoveNext == Log4JIteratorEventSourceReverseMoveNext_);
        assert (self->Type != Filter || self->MoveNext == Log4JIteratorFilterMoveNext_);
        assert (self->Type != EventSource || self->Destroy == Log4JIteratorEventSourceDestroy_);
        assert (self->Type != EventSourceReverse || self->Destroy == Log4JIteratorEventSourceReverseDestroy_);
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

#pragma region Event source log4j_iterator

typedef struct
{
    const Log4JEventSource *Source;
    Log4JEvent Current;
    Log4JEvent First;
} Log4JIteratorEventSourceContext_;

LOG4JPARSERC_API void Log4JIteratorInitEventSource (Log4JIterator **self, const Log4JEventSource *source)
{
    Log4JIteratorEventSourceContext_ *context = (Log4JIteratorEventSourceContext_ *) malloc (sizeof (*context));
    *context = (Log4JIteratorEventSourceContext_ )
    {
        .Source = source,
        .Current = NULL,
        .First = NULL
    };

    Log4JIterator *result = (Log4JIterator *) malloc (sizeof (*result));
    *result = (Log4JIterator)
    {
        #ifdef _DEBUG
            .Type = EventSource,
        #endif
        .Context = context,
        .Destroy = &Log4JIteratorEventSourceDestroy_,
        .MoveNext = &Log4JIteratorEventSourceMoveNext_,
        .Current = &Log4JIteratorEventSourceCurrent_
    };

    *self = result;
}

void Log4JIteratorEventSourceDestroy_ (void *context)
{
    Log4JIteratorEventSourceContext_ *contextD = (Log4JIteratorEventSourceContext_ *) context;

    *contextD = (Log4JIteratorEventSourceContext_)
    {
        .Source = NULL,
        .Current = NULL,
        .First = NULL
    };
    free (contextD);
}

bool Log4JIteratorEventSourceMoveNext_ (void *context)
{
    Log4JIteratorEventSourceContext_ *contextD = (Log4JIteratorEventSourceContext_ *) context;

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

const Log4JEvent Log4JIteratorEventSourceCurrent_ (const void *context, size_t *id)
{
    const Log4JIteratorEventSourceContext_ *contextD = (const Log4JIteratorEventSourceContext_ *) context;
    if (id != NULL && contextD->Current && contextD->First)
    {
        size_t sizeDump;

        const char *currentMessage;
        Log4JEventMessage (contextD->Current, &currentMessage, &sizeDump);

        const char *firstMessage;
        Log4JEventMessage(contextD->First, &firstMessage, &sizeDump);

        *id = currentMessage - firstMessage;
    }

    return contextD->Current;
}

#pragma endregion

#pragma region Reverse event source log4j_iterator

typedef struct
{
    const Log4JEventSource *Source;
    Log4JEvent Current;
    Log4JEvent First;
} Log4JIteratorEventSourceReverseContext_;

LOG4JPARSERC_API void Log4JIteratorInitEventSourceReverse (Log4JIterator **self, const Log4JEventSource *source)
{
    Log4JIteratorEventSourceContext_ *context = (Log4JIteratorEventSourceContext_ *) malloc (sizeof (*context));
    *context = (Log4JIteratorEventSourceContext_)
    {
        .Source = source,
        .Current = NULL,
        .First = NULL
    };

    Log4JIterator *result = (Log4JIterator *) malloc (sizeof (*result));
    *result = (Log4JIterator)
    {
    #ifdef _DEBUG
        .Type = EventSourceReverse,
    #endif
        .Context = context,
        .Destroy = &Log4JIteratorEventSourceReverseDestroy_,
        .MoveNext = &Log4JIteratorEventSourceReverseMoveNext_,
        .Current = &Log4JIteratorEventSourceReverseCurrent_
    };

    *self = result;
}

void Log4JIteratorEventSourceReverseDestroy_ (void *context)
{
    Log4JIteratorEventSourceReverseContext_ *contextD = (Log4JIteratorEventSourceReverseContext_ *) context;

    *contextD = (Log4JIteratorEventSourceReverseContext_)
    {
        .Source = NULL,
        .Current = NULL,
        .First = NULL
    };
    free (contextD);
}

bool Log4JIteratorEventSourceReverseMoveNext_ (void *context)
{
    Log4JIteratorEventSourceReverseContext_ *contextD = (Log4JIteratorEventSourceReverseContext_ *) context;

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

const Log4JEvent Log4JIteratorEventSourceReverseCurrent_ (const void *context, size_t *id)
{
    const Log4JIteratorEventSourceReverseContext_ *contextD = (const Log4JIteratorEventSourceReverseContext_ *) context;
    if (id != NULL && contextD->Current && contextD->First)
    {
        size_t sizeDump;

        const char *currentMessage;
        Log4JEventMessage (contextD->Current, &currentMessage, &sizeDump);

        const char *firstMessage;
        Log4JEventMessage (contextD->First, &firstMessage, &sizeDump);

        *id = currentMessage - firstMessage;
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

LOG4JPARSERC_API void Log4JIteratorInitFilter (Log4JIterator **self, Log4JIterator *inner, const Log4JFilter *filter)
{
    Log4JIteratorFilterContext_ *context = (Log4JIteratorFilterContext_ *) malloc (sizeof (*context));
    *context = (Log4JIteratorFilterContext_) { .Inner = inner, .Filter = filter };

    Log4JIterator *result = (Log4JIterator *) malloc (sizeof (*result));
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
