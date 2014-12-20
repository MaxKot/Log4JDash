#include "Event.h"

namespace Log4JParser
{
    Event::Event (Log4JEvent event)
        : event_ (event)
    {

    }

    Event::~Event ()
    {
        event_ = nullptr;
    }

    FixedString Event::Level () const
    {
        return Log4JEventLevel (event_);
    }

    FixedString Event::Logger () const
    {
        return Log4JEventLogger (event_);
    }

    FixedString Event::Thread () const
    {
        return Log4JEventThread (event_);
    }

    FixedString Event::Timestamp () const
    {
        return Log4JEventTimestamp (event_);
    }

    int64_t Event::Time () const
    {
        return Log4JEventTime (event_);
    }

    FixedString Event::Message () const
    {
        return Log4JEventMessage (event_);
    }

    FixedString Event::Throwable () const
    {
        return Log4JEventThrowable (event_);
    }

    Log4JEvent Event::GetEvent (const Event event)
    {
        return event.event_;
    }

    EventSource::EventSource (char *xmlString)
    {
        Log4JEventSourceInitXmlString (&eventSource_, xmlString);
    }

    EventSource::~EventSource ()
    {
        Log4JEventSourceDestroy (eventSource_);
        eventSource_ = nullptr;
    }

    Event EventSource::First () const
    {
        auto event = Log4JEventSourceFirst (eventSource_);

        return Event (event);
    }

    Event EventSource::Next (Event event) const
    {
        auto cEvent = Log4JEventSourceNext (eventSource_, Event::GetEvent (event));
        return Event (cEvent);
    }

    const Log4JEventSource *EventSource::GetEventSource (const EventSource *eventSource)
    {
        return eventSource->eventSource_;
    }

    Log4JEventSource *EventSource::GetEventSource (EventSource *eventSource)
    {
        return eventSource->eventSource_;
    }
}
