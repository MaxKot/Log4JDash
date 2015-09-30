#include "Iterator.h"

namespace Log4JParser
{
    IteratorBase::IteratorBase (Log4JIterator *iterator)
        : iterator_ (iterator)
    {

    }

    IteratorBase::~IteratorBase ()
    {
        Log4JIteratorDestroy (iterator_);
    }

    bool IteratorBase::MoveNext ()
    {
        auto result = Log4JIteratorMoveNext (iterator_);
        return result;
    }

    Event IteratorBase::Current (size_t *id) const
    {
        auto event = Log4JIteratorCurrent (iterator_, id);
        return Event (event);
    }

    const Log4JIterator *IteratorBase::GetIterator (const IteratorBase *iterator)
    {
        return iterator->iterator_;
    }

    Log4JIterator *IteratorBase::GetIterator (IteratorBase *iterator)
    {
        return iterator->iterator_;
    }

    static Log4JIterator *Log4JIteratorInitEventSource_ (const Log4JEventSource *source)
    {
        Log4JIterator *iterator;
        Log4JIteratorInitEventSource (&iterator, source);

        return iterator;
    }

    IteratorEventSource::IteratorEventSource (const EventSource *source)
        : IteratorBase (Log4JIteratorInitEventSource_ (EventSource::GetEventSource (source)))
    {

    }

    static Log4JIterator *Log4JIteratorInitFilter_ (Log4JIterator *inner, const Log4JFilter *filter)
    {
        Log4JIterator *iterator;
        Log4JIteratorInitFilter (&iterator, inner, filter);

        return iterator;
    }

    IteratorFilter::IteratorFilter (IteratorBase *inner, const FilterBase *filter)
        : IteratorBase (Log4JIteratorInitFilter_ (GetIterator (inner), FilterBase::GetFilter (filter)))
    {

    }
}
