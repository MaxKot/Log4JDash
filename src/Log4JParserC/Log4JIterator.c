#include <assert.h>
#include <memory.h>
#include <stdbool.h>
#include <stdlib.h>
#include "Log4JParserC.h"
#include "Private.h"

typedef void Log4JIteratorDestroyCb (void *context);

typedef bool Log4JIteratorMoveNextCb (void *context);

typedef const Log4JEvent Log4JIteratorCurrentCb (const void *context, int32_t *id);

#ifdef _DEBUG
    typedef enum { EventSource, Filter } IteratorType;
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
static const Log4JEvent Log4JIteratorEventSourceCurrent_ (const void *context, int32_t *id);

static void Log4JIteratorFilterDestroy_ (void *context);
static bool Log4JIteratorFilterMoveNext_ (void *context);
static const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context, int32_t *id);

#ifdef _DEBUG
    static void AssertLog4JIteratorSanityImpl_ (Log4JIterator *self)
    {
        assert (self != NULL);
        assert (self->Context != NULL);
        assert (self->Type == EventSource || self->Type == Filter);
        assert (self->Type != EventSource || self->Current == Log4JIteratorEventSourceCurrent_);
        assert (self->Type != Filter || self->Current == Log4JIteratorFilterCurrent_);
        assert (self->Type != EventSource || self->MoveNext == Log4JIteratorEventSourceMoveNext_);
        assert (self->Type != Filter || self->MoveNext == Log4JIteratorFilterMoveNext_);
        assert (self->Type != EventSource || self->Destroy == Log4JIteratorEventSourceDestroy_);
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

LOG4JPARSERC_API const Log4JEvent Log4JIteratorCurrent (const Log4JIterator *self, int32_t *id)
{
    AssertLog4JIteratorSanity_ (self);

    Log4JEvent result = self->Current (self->Context, id);

    AssertLog4JIteratorSanity_ (self);
    return result;
}

#pragma region XML string log4j_iterator

typedef struct
{
    const Log4JEventSource *Source;
    Log4JEvent Current;
    int32_t Count;
} Log4JIteratorEventSourceContext_;

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
        .Count = 0
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

const Log4JEvent Log4JIteratorEventSourceCurrent_ (const void *context, int32_t *id)
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

LOG4JPARSERC_API void Log4JIteratorInitFilter (Log4JIterator **self, Log4JIterator *inner, const Log4JFilter *filter)
{
    Log4JIteratorFilterContext_ *context = (Log4JIteratorFilterContext_ *) malloc (sizeof (Log4JIteratorFilterContext_));
    *context = (Log4JIteratorFilterContext_) { .Inner = inner, .Filter = filter };

    Log4JIterator *result = (Log4JIterator *) malloc (sizeof (Log4JIterator));
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

const Log4JEvent Log4JIteratorFilterCurrent_ (const void *context, int32_t *id)
{
    Log4JIteratorFilterContext_ *contextF = (Log4JIteratorFilterContext_ *) context;

    return Log4JIteratorCurrent (contextF->Inner, id);
}

#pragma endregion
