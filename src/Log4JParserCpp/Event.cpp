#include "Event.h"

namespace Log4JParser
{
    FixedString::FixedString (const char * value, const size_t size)
        : value_ (value), size_ (size)
    {

    }

    const char *FixedString::Value () const
    {
        return value_;
    }

    size_t FixedString::Size () const
    {
        return size_;
    }

    std::basic_ostream<char, std::char_traits<char>> &operator << (std::basic_ostream<char, std::char_traits<char>> &stream, FixedString &str)
    {
        stream.write (str.Value (), str.Size ());
        return stream;
    }

    Property::Property (const FixedString name, const FixedString value)
        : name_ (name), value_ (value)
    {

    }

    const FixedString Property::Name () const
    {
        return name_;
    }

    const FixedString Property::Value () const
    {
        return value_;
    }

    std::basic_ostream<char, std::char_traits<char>> &operator << (std::basic_ostream<char, std::char_traits<char>> &stream, Property &prop)
    {
        auto name = prop.Name ();
        auto value = prop.Value ();
        return stream << name << ": " << value;
    }

    Event::Event (Log4JEvent event)
        : event_ (event)
    {

    }

    Event::~Event ()
    {
        event_ = nullptr;
    }

    const FixedString Event::Level () const
    {
        const char *level;
        size_t levelSize;
        Log4JEventLevel (event_, &level, &levelSize);

        return FixedString (level, levelSize);
    }

    const FixedString Event::Logger () const
    {
        const char *logger;
        size_t loggerSize;
        Log4JEventLogger (event_, &logger, &loggerSize);

        return FixedString (logger, loggerSize);
    }

    const FixedString Event::Thread () const
    {
        const char *thread;
        size_t threadSize;
        Log4JEventThread (event_, &thread, &threadSize);

        return FixedString (thread, threadSize);
    }

    int64_t Event::Timestamp () const
    {
        return Log4JEventTimestamp (event_);
    }

    const FixedString Event::Message () const
    {
        const char *message;
        size_t messageSize;
        Log4JEventMessage (event_, &message, &messageSize);

        return FixedString (message, messageSize);
    }

    const FixedString Event::Throwable () const
    {
        const char *throwable;
        size_t throwableSize;
        Log4JEventThrowable (event_, &throwable, &throwableSize);

        return FixedString (throwable, throwableSize);
    }

    void Event::Properties (std::vector<Property> &properties) const
    {
        const size_t bufferSize = 16U;
        Log4JEventProperty buffer[bufferSize];

        size_t totalEvents;
        size_t totalEventsRead = 0U;
        do
        {
            totalEvents = Log4JEventProperties (event_, totalEventsRead, buffer, bufferSize);
            auto eventsRemaining = totalEvents - totalEventsRead;
            auto eventsRead = eventsRemaining > bufferSize ? bufferSize : eventsRemaining;

            for (size_t i = 0U; i < eventsRead; ++i)
            {
                FixedString name (buffer[i].name, buffer[i].nameSize);
                FixedString value (buffer[i].value, buffer[i].valueSize);
                properties.push_back (Property (name, value));
            }

            totalEventsRead += eventsRead;
        } while (totalEventsRead != totalEvents);
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
