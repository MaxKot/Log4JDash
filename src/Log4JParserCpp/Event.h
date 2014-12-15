#pragma once

#include <stdint.h>

namespace Log4JParser
{
    namespace CApi
    {
#include <Log4JEvent.h>
    }

    class Event
    {
        friend class EventSource;

    public:
        Event (CApi::Log4JEvent event);
        ~Event ();

    public:
        CApi::FixedString Level () const;

        CApi::FixedString Logger () const;

        CApi::FixedString Thread () const;

        CApi::FixedString Timestamp () const;
        int64_t Time () const;

        CApi::FixedString Message () const;

        CApi::FixedString Throwable () const;

    private:
        static CApi::Log4JEvent GetEvent (const Event event);

    private:
        CApi::Log4JEvent event_;
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
        static const CApi::Log4JEventSource *GetEventSource (const EventSource *eventSource);
        static CApi::Log4JEventSource *GetEventSource (EventSource *eventSource);

    private:
        CApi::Log4JEventSource *eventSource_;
    };
}
