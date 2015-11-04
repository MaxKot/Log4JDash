#pragma once

#include <iostream>
#include <vector>
extern "C"
{
#include <Log4JParserC.h>
}

namespace Log4JParser
{
    class FixedString
    {
    public:
        FixedString (const char *value, const size_t size);

        const char *Value () const;
        size_t Size () const;

    private:
        const char *value_;
        const size_t size_;
    };

    std::basic_ostream<char, std::char_traits<char>> &operator << (std::basic_ostream<char, std::char_traits<char>> &stream, FixedString &str);

    class Property
    {
    public:
        Property (const FixedString name, const FixedString value);

        const FixedString Name () const;
        const FixedString Value () const;

    private:
        const FixedString name_;
        const FixedString value_;
    };

    std::basic_ostream<char, std::char_traits<char>> &operator << (std::basic_ostream<char, std::char_traits<char>> &stream, Property &str);

    class Event
    {
        friend class EventSource;
        friend class FilterBase;

    public:
        Event (Log4JEvent event);
        ~Event ();

    public:
        const FixedString Level () const;

        const FixedString Logger () const;

        const FixedString Thread () const;

        int64_t Timestamp () const;

        const FixedString Message () const;

        const FixedString Throwable () const;

        void Properties (std::vector<Property> &properties) const;

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
