#pragma once

#include "Event.h"
#include "Filter.h"
extern "C"
{
#include <Log4JParserC.h>
}

namespace Log4JParser
{
    class IteratorBase
    {
    protected:
        IteratorBase (Log4JIterator *iterator);

    public:
        ~IteratorBase ();

    public:
        bool MoveNext ();

        Event Current (int32_t *id = nullptr) const;

    protected:
        static const Log4JIterator *GetIterator (const IteratorBase *iterator);
        static Log4JIterator *GetIterator (IteratorBase *iterator);

    private:
        Log4JIterator *iterator_;
    };

    class IteratorEventSource : public IteratorBase
    {
    public:
        IteratorEventSource (const EventSource *source);
    };

    class IteratorFilter : public IteratorBase
    {
    public:
        IteratorFilter (IteratorBase *inner, const FilterBase *filter);
    };
}
