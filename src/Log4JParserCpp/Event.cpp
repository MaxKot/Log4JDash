#include "Event.h"

namespace Log4JParser
{
    Event::Event (CApi::Log4JEvent event)
        : event_ (event)
    {

    }

    Event::~Event ()
    {
        event_ = nullptr;
    }

    CApi::FixedString Event::Level () const
    {
        return CApi::Log4JEventLevel (event_);
    }

    CApi::FixedString Event::Logger () const
    {
        return CApi::Log4JEventLogger (event_);
    }

    CApi::FixedString Event::Thread () const
    {
        return CApi::Log4JEventThread (event_);
    }

    CApi::FixedString Event::Timestamp () const
    {
        return CApi::Log4JEventTimestamp (event_);
    }

    int64_t Event::Time () const
    {
        return CApi::Log4JEventTime (event_);
    }

    CApi::FixedString Event::Message () const
    {
        return CApi::Log4JEventMessage (event_);
    }

    CApi::FixedString Event::Throwable () const
    {
        return CApi::Log4JEventThrowable (event_);
    }

    CApi::Log4JEvent Event::GetEvent (const Event event)
    {
        return event.event_;
    }

    EventSource::EventSource (char *xmlString)
    {
        CApi::Log4JEventSourceInitXmlString (&eventSource_, xmlString);
    }

    EventSource::~EventSource ()
    {
        CApi::Log4JEventSourceDestroy (eventSource_);
        eventSource_ = nullptr;
    }

    Event EventSource::First () const
    {
        auto event = CApi::Log4JEventSourceFirst (eventSource_);

        return Event (event);
    }

    Event EventSource::Next (Event event) const
    {
        auto cEvent = CApi::Log4JEventSourceNext (eventSource_, Event::GetEvent (event));
        return Event (cEvent);
    }

    const CApi::Log4JEventSource *EventSource::GetEventSource (const EventSource *eventSource)
    {
        return eventSource->eventSource_;
    }

    CApi::Log4JEventSource *EventSource::GetEventSource (EventSource *eventSource)
    {
        return eventSource->eventSource_;
    }
}
