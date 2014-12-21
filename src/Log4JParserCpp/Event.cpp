#include "Event.h"

namespace Log4JParser
{
    FixedString::FixedString (const char * value, const size_t size)
        : value_ (value), size_ (size)
    {

    }

    const char * FixedString::Value ()
    {
        return value_;
    }

    size_t FixedString::Size ()
    {
        return size_;
    }

    std::basic_ostream<char, std::char_traits<char>> &operator << (std::basic_ostream<char, std::char_traits<char>> &stream, FixedString &str)
    {
        stream.write (str.Value (), str.Size ());
        return stream;
    }

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
        const char *level;
        size_t levelSize;
        Log4JEventLevel (event_, &level, &levelSize);

        return FixedString (level, levelSize);
    }

    FixedString Event::Logger () const
    {
        const char *logger;
        size_t loggerSize;
        Log4JEventLogger (event_, &logger, &loggerSize);

        return FixedString (logger, loggerSize);
    }

    FixedString Event::Thread () const
    {
        const char *thread;
        size_t threadSize;
        Log4JEventThread (event_, &thread, &threadSize);

        return FixedString (thread, threadSize);
    }

    FixedString Event::Timestamp () const
    {
        const char *timestamp;
        size_t timestampSize;
        Log4JEventTimestamp (event_, &timestamp, &timestampSize);

        return FixedString (timestamp, timestampSize);
    }

    int64_t Event::Time () const
    {
        return Log4JEventTime (event_);
    }

    FixedString Event::Message () const
    {
        const char *message;
        size_t messageSize;
        Log4JEventMessage (event_, &message, &messageSize);

        return FixedString (message, messageSize);
    }

    FixedString Event::Throwable () const
    {
        const char *throwable;
        size_t throwableSize;
        Log4JEventThrowable (event_, &throwable, &throwableSize);

        return FixedString (throwable, throwableSize);
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
