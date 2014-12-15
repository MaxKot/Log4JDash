#pragma once

#include "Event.h"
#include "Filter.h"

namespace Log4JParser
{
    namespace CApi
    {
#include <Log4JIterator.h>
    }

    class IteratorBase
    {
    protected:
        IteratorBase (CApi::Log4JIterator *iterator);

    public:
        ~IteratorBase ();

    public:
        bool MoveNext ();

        Event Current () const;

    protected:
        static const CApi::Log4JIterator *GetIterator (const IteratorBase *iterator);
        static CApi::Log4JIterator *GetIterator (IteratorBase *iterator);

    private:
        CApi::Log4JIterator *iterator_;
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
