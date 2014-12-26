#include <string.h>
#include <stdio.h>
#include <rapidxml\rapidxml.hpp>
#include <Windows.h>
extern "C"
{
#include "Log4JParserC.h"
}

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

static void GetValue_ (const rapidxml::xml_base<char> *source, const char **value, size_t *size);
static int64_t ParseTimestamp_ (const char *value, const size_t valueSize);

LOG4JPARSERC_API void __cdecl Log4JEventLevel (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrLevel_, AttrLevelSize_);
    GetValue_ (xml, value, size);
}

LOG4JPARSERC_API void __cdecl Log4JEventLogger (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrLogger_, AttrLoggerSize_);
    GetValue_ (xml, value, size);
}

LOG4JPARSERC_API void __cdecl Log4JEventThread (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrThread_, AttrThreadSize_);
    GetValue_ (xml, value, size);
}

LOG4JPARSERC_API void __cdecl Log4JEventTimestamp (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrTimestamp_, AttrTimestampSize_);
    GetValue_ (xml, value, size);
}

LOG4JPARSERC_API int64_t __cdecl Log4JEventTime (const Log4JEvent log4JEvent)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrTimestamp_, AttrTimestampSize_);
    auto result = ParseTimestamp_ (xml->value (), xml->value_size ());

    return result;
}

LOG4JPARSERC_API void __cdecl Log4JEventMessage (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_node (TagMessage_, TagMessageSize_);
    GetValue_ (xml, value, size);
}

LOG4JPARSERC_API void __cdecl Log4JEventThrowable (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_node (TagThrowable_, TagThrowableSize_);
    GetValue_ (xml, value, size);
}

void GetValue_ (const rapidxml::xml_base<char> *source, const char **value, size_t *size)
{
    if (source)
    {
        *value = source->value ();
        *size = source->value_size ();
    }
    else
    {
        *value = nullptr;
        *size = 0UL;
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

struct Log4JEventSource_
{
    const rapidxml::xml_document<char> *Doc;
    char *OwnXmlString;
};

static void Log4JEventSourceInitXmlStringImpl (Log4JEventSource **self, char *xmlString, bool ownString)
{
    auto ownedStringPtr = ownString ? xmlString : nullptr;

    auto doc = new rapidxml::xml_document<char> ();
    doc->parse<rapidxml::parse_fastest> (xmlString);

    auto result = (Log4JEventSource *) malloc (sizeof (Log4JEventSource));
    *result = { doc, ownedStringPtr };

    *self = result;
}

LOG4JPARSERC_API void __cdecl Log4JEventSourceInitXmlString (Log4JEventSource **self, char *xmlString)
{
    Log4JEventSourceInitXmlStringImpl (self, xmlString, false);
}

LOG4JPARSERC_API void Log4JEventSourceInitXmlFile (Log4JEventSource **self, const char *filePath)
{
    char *buffer = nullptr;
    long length = 0L;
    FILE *f = nullptr;

    auto h = LoadLibrary(L"api-ms-win-core-fibers-l1-1-1.dll");
    printf ("%p\n", h);

    auto openResult = fopen_s (&f, filePath, "rb");

    if (!openResult && f)
    {
        fseek (f, 0, SEEK_END);
        length = ftell (f);

        printf ("File length: '%d'...\n", length);

        fseek (f, 0, SEEK_SET);
        buffer = (char *) malloc (length + 1);

        if (buffer)
        {
            fread (buffer, 1, length, f);
            buffer[length] = '\0';
        }

        fclose (f);

        Log4JEventSourceInitXmlStringImpl (self, buffer, true);
    }
    else
    {
        *self = nullptr;
    }
}

LOG4JPARSERC_API void __cdecl Log4JEventSourceDestroy (Log4JEventSource *self)
{
    delete self->Doc;
    if (self->OwnXmlString)
    {
        free (self->OwnXmlString);
    }

    *self = { nullptr, nullptr };
    free (self);
}

LOG4JPARSERC_API Log4JEvent __cdecl Log4JEventSourceFirst (const Log4JEventSource *self)
{
    auto node = self->Doc->first_node (TagEvent_, TagEventSize_);
    return node;
}

LOG4JPARSERC_API Log4JEvent __cdecl Log4JEventSourceNext (const Log4JEventSource *self, const Log4JEvent event)
{
    auto node = (rapidxml::xml_node<char> *) event;
    auto nextNode = node->next_sibling (TagEvent_, TagEventSize_);
    return nextNode;
}
