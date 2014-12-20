#pragma once

#include <stdint.h>
extern "C"
{
#include <Log4JEvent.h>
}

namespace Log4JParser
{
    class Event
    {
        friend class EventSource;
        friend class FilterBase;

    public:
        Event (Log4JEvent event);
        ~Event ();

    public:
        FixedString Level () const;

        FixedString Logger () const;

        FixedString Thread () const;

        FixedString Timestamp () const;
        int64_t Time () const;

        FixedString Message () const;

        FixedString Throwable () const;

    private:
        static Log4JEvent GetEvent (const Event event);

    private:
        Log4JEvent event_;
    };

    class EventSource
    {
        friend class IteratorEventSource;

    public:
        EventSource (char *xmlString);
        ~EventSource ();

    public:
        Event First () const;

        Event Next (Event event) const;

    private:
        static const Log4JEventSource *GetEventSource (const EventSource *eventSource);
        static Log4JEventSource *GetEventSource (EventSource *eventSource);

    private:
        Log4JEventSource *eventSource_;
    };
}
