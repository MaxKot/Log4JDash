#include "Iterator.h"

namespace Log4JParser
{
    IteratorBase::IteratorBase (CApi::Log4JIterator *iterator)
        : iterator_ (iterator)
    {

    }

    IteratorBase::~IteratorBase ()
    {
        CApi::Log4JIteratorDestroy (iterator_);
    }

    bool IteratorBase::MoveNext ()
    {
        auto result = CApi::Log4JIteratorMoveNext (iterator_);
        return result;
    }

    Event IteratorBase::Current () const
    {
        auto event = CApi::Log4JIteratorCurrent (iterator_);
        return Event (event);
    }

    const CApi::Log4JIterator *IteratorBase::GetIterator (const IteratorBase *iterator)
    {
        return iterator->iterator_;
    }

    CApi::Log4JIterator *IteratorBase::GetIterator (IteratorBase *iterator)
    {
        return iterator->iterator_;
    }

    static CApi::Log4JIterator *Log4JIteratorInitEventSource_ (const CApi::Log4JEventSource *source)
    {
        CApi::Log4JIterator *iterator;
        CApi::Log4JIteratorInitEventSource (&iterator, source);

        return iterator;
    }

    IteratorEventSource::IteratorEventSource (const EventSource *source)
        : IteratorBase (Log4JIteratorInitEventSource_ (EventSource::GetEventSource (source)))
    {

    }

    static CApi::Log4JIterator *Log4JIteratorInitFilter_ (CApi::Log4JIterator *inner, const CApi::Log4JFilter *filter)
    {
        CApi::Log4JIterator *iterator;
        CApi::Log4JIteratorInitFilter (&iterator, inner, filter);

        return iterator;
    }

    IteratorFilter::IteratorFilter (IteratorBase *inner, const FilterBase *filter)
        : IteratorBase (Log4JIteratorInitFilter_ (GetIterator (inner), FilterBase::GetFilter (filter)))
    {

    }
}
