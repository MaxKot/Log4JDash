#include <rapidxml\rapidxml.hpp>
#include "Log4JEvent.h"

const char TagEvent_[] = "log4j:event";
const size_t TagEventSize_ = sizeof (TagEvent_) - 1U;

const char AttrLevel_[] = "level";
const size_t AttrLevelSize_ = sizeof (AttrLevel_) - 1U;

const char AttrLogger_[] = "logger";
const size_t AttrLoggerSize_ = sizeof (AttrLogger_) - 1U;

const char AttrThread_[] = "thread";
const size_t AttrThreadSize_ = sizeof (AttrThread_) - 1U;

const char AttrTimestamp_[] = "timestamp";
const size_t AttrTimestampSize_ = sizeof (AttrTimestamp_) - 1U;

const char TagMessage_[] = "log4j:message";
const size_t TagMessageSize_ = sizeof (TagMessage_) - 1U;

const char TagThrowable_[] = "log4j:throwable";
const size_t TagThrowableSize_ = sizeof (TagThrowable_) - 1U;

static FixedString GetValue_ (const rapidxml::xml_base<char> *source);
static int64_t ParseTimestamp_ (const char *value, const size_t valueSize);

FixedString Log4JEventLevel (const Log4JEvent log4JEvent)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrLevel_, AttrLevelSize_);
    return GetValue_ (xml);
}

FixedString Log4JEventLogger (const Log4JEvent log4JEvent)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrLogger_, AttrLoggerSize_);
    return GetValue_ (xml);
}

FixedString Log4JEventThread (const Log4JEvent log4JEvent)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrThread_, AttrThreadSize_);
    return GetValue_ (xml);
}

FixedString Log4JEventTimestamp (const Log4JEvent log4JEvent)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrTimestamp_, AttrTimestampSize_);
    return GetValue_ (xml);
}

int64_t Log4JEventTime (const Log4JEvent log4JEvent)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrTimestamp_, AttrTimestampSize_);
    auto result = ParseTimestamp_ (xml->value (), xml->value_size ());

    return result;
}

FixedString Log4JEventMessage (const Log4JEvent log4JEvent)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_node (TagMessage_, TagMessageSize_);
    return GetValue_ (xml);
}

FixedString Log4JEventThrowable (const Log4JEvent log4JEvent)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_node (TagThrowable_, TagThrowableSize_);
    return GetValue_ (xml);
}

FixedString GetValue_ (const rapidxml::xml_base<char> *source)
{
    if (source)
    {
        auto value = source->value ();
        auto valueSize = source->value_size ();
        return FixedString { value, valueSize };
    }
    else
    {
        return FixedString { nullptr, 0UL };
    }
}

int64_t ParseTimestamp_ (const char *value, const size_t valueSize)
{
    int64_t result = 0L;

    for (size_t i = 0; i < valueSize; ++i)
    {
        int64_t digit = value[i] - '0';
        result = result * 10L + digit;
    }

    return result;
}

void Log4JEventInitFirst (Log4JEvent *event, const char *xmlString)
{

}

bool Log4JEventNext (Log4JEvent *event)
{

}

void Log4JEventDestroy (Log4JEvent *event)
{

}
