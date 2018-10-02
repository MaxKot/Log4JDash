#include <string.h>
#include <memory>
#include <vector>
#include <stdio.h>
#include <rapidxml\rapidxml.hpp>
#include <Windows.h>
extern "C"
{
#include "Log4JParserC.h"
#include "Private.h"
}

const char LogFragmentStart_[] = "<log4j:event";

const char TagEvent_[] = "log4j:event";
const size_t TagEventSize_ = sizeof TagEvent_ / sizeof TagEvent_[0] - 1U;

const char AttrLevel_[] = "level";
const size_t AttrLevelSize_ = sizeof AttrLevel_ / sizeof AttrLevel_[0] - 1U;

const char AttrLogger_[] = "logger";
const size_t AttrLoggerSize_ = sizeof AttrLogger_ / sizeof AttrLogger_[0] - 1U;

const char AttrThread_[] = "thread";
const size_t AttrThreadSize_ = sizeof AttrThread_ / sizeof AttrThread_[0] - 1U;

const char AttrTimestamp_[] = "timestamp";
const size_t AttrTimestampSize_ = sizeof AttrTimestamp_ / sizeof AttrTimestamp_[0] - 1U;

const char TagMessage_[] = "log4j:message";
const size_t TagMessageSize_ = sizeof TagMessage_ / sizeof TagMessage_[0] - 1U;

const char TagThrowable_[] = "log4j:throwable";
const size_t TagThrowableSize_ = sizeof TagThrowable_ / sizeof TagThrowable_[0] - 1U;

const char TagProperties_[] = "log4j:properties";
const size_t TagPropertiesSize_ = sizeof TagProperties_ / sizeof TagProperties_[0] - 1U;

const char TagData_[] = "log4j:data";
const size_t TagDataSize_ = sizeof TagData_ / sizeof TagData_[0] - 1U;

const char AttrDataName_[] = "name";
const size_t AttrDataNameSize_ = sizeof AttrDataName_ / sizeof AttrDataName_[0] - 1U;

const char AttrDataValue_[] = "value";
const size_t AttrDataValueSize_ = sizeof AttrDataValue_ / sizeof AttrDataValue_[0] - 1U;

static void GetAttributeValue_ (const rapidxml::xml_attribute<char> *source, const char **value, size_t *size);
static void GetNodeValue_ (rapidxml::xml_node<char> *source, const char **value, size_t *size);
static int64_t ParseTimestamp_ (const char *value, const size_t valueSize);

template <class T>
struct Log4JAllocator
{
    typedef T value_type;

    Log4JAllocator () noexcept
    {

    }

    template <class U>
    Log4JAllocator (const Log4JAllocator<U>&) noexcept
    {

    }

    T* allocate (std::size_t n)
    {
        return static_cast<T*>(Alloc_ (n*sizeof T));
    }

    void deallocate (T* p, std::size_t n)
    {
        Free_ (p);
    }
};

template <class T, class U>
constexpr bool operator== (const Log4JAllocator<T>&, const Log4JAllocator<U>&) noexcept
{
    return true;
}

template <class T, class U>
constexpr bool operator!= (const Log4JAllocator<T>&, const Log4JAllocator<U>&) noexcept
{
    return false;
}

LOG4JPARSERC_API void __cdecl Log4JEventLevel (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (const rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrLevel_, AttrLevelSize_);
    GetAttributeValue_ (xml, value, size);
}

LOG4JPARSERC_API void __cdecl Log4JEventLogger (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (const rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrLogger_, AttrLoggerSize_);
    GetAttributeValue_ (xml, value, size);
}

LOG4JPARSERC_API void __cdecl Log4JEventThread (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (const rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrThread_, AttrThreadSize_);
    GetAttributeValue_ (xml, value, size);
}

LOG4JPARSERC_API int64_t __cdecl Log4JEventTimestamp (const Log4JEvent log4JEvent)
{
    auto node = (const rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_attribute (AttrTimestamp_, AttrTimestampSize_);
    auto result = ParseTimestamp_ (xml->value (), xml->value_size ());

    return result;
}

LOG4JPARSERC_API void __cdecl Log4JEventMessage (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_node (TagMessage_, TagMessageSize_);
    GetNodeValue_ (xml, value, size);
}

LOG4JPARSERC_API void __cdecl Log4JEventThrowable (const Log4JEvent log4JEvent, const char **value, size_t *size)
{
    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto xml = node->first_node (TagThrowable_, TagThrowableSize_);
    GetNodeValue_ (xml, value, size);
}

LOG4JPARSERC_API size_t Log4JEventProperties (const Log4JEvent log4JEvent, size_t skip, Log4JEventProperty *properties, size_t propertiesSize)
{
    if (properties == nullptr)
    {
        propertiesSize = 0U;
    }
    size_t actualProperties = 0U;

    auto node = (rapidxml::xml_node<char> *) log4JEvent;
    auto propertiesNode = node->first_node (TagProperties_, TagPropertiesSize_);
    if (propertiesNode != nullptr)
    {
        auto dataNode = propertiesNode->first_node (TagData_, TagDataSize_);
        while (dataNode != nullptr)
        {
            ++actualProperties;

            if (skip != 0U)
            {
                --skip;
            }
            else if (propertiesSize != 0U)
            {
                auto name = dataNode->first_attribute (AttrDataName_, AttrDataNameSize_);
                GetAttributeValue_ (name, &properties->name, &properties->nameSize);

                auto value = dataNode->first_attribute (AttrDataValue_, AttrDataValueSize_);
                GetAttributeValue_ (value, &properties->value, &properties->valueSize);

                ++properties;
                --propertiesSize;
            }

            dataNode = dataNode->next_sibling (TagData_, TagDataSize_);
        }
    }

    return actualProperties;
}

void GetAttributeValue_ (const rapidxml::xml_attribute<char> *source, const char **value, size_t *size)
{
    if (source)
    {
        *value = source->value ();
        *size = source->value_size ();
    }
    else
    {
        *value = nullptr;
        *size = 0U;
    }
}

void GetNodeValue_ (rapidxml::xml_node<char> *source, const char **value, size_t *size)
{
    if (source)
    {
        auto firstChild = source->first_node ();
        if (!firstChild)
        {
            *value = source->value ();
            *size = source->value_size ();
        }
        else if (firstChild == source->last_node ())
        {
            *value = firstChild->value ();
            *size = firstChild->value_size ();
        }
        else
        {
            size_t bufferLength = 0U;
            for (auto node = firstChild; node; node = node->next_sibling ())
            {
                bufferLength += node->value_size ();
            }

            auto bufferSize = bufferLength + 1;
            auto buffer = source->document ()->allocate_string (nullptr, bufferSize);
            buffer[bufferSize - 1] = '\0';

            auto concatHead = buffer;
            auto bufferFreeSize = bufferSize;
            for (auto node = firstChild; node; node = node->next_sibling ())
            {
                auto nodeValue = node->value ();
                auto nodeValueSize = node->value_size ();

                strncpy_s (concatHead, bufferFreeSize, nodeValue, nodeValueSize);
                auto copied = min (bufferFreeSize, nodeValueSize);

                bufferFreeSize -= copied;
                concatHead += copied;
            }

            source->remove_all_nodes ();
            source->value (buffer, bufferLength);

            *value = buffer;
            *size = bufferLength;
        }
    }
    else
    {
        *value = nullptr;
        *size = 0U;
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

typedef rapidxml::xml_document<char> XmlDocument_;

void DeleteXmlDocument_ (XmlDocument_ *obj)
{
    obj->~XmlDocument_ ();
    Free_ (obj);
}

typedef std::unique_ptr<XmlDocument_, decltype(&DeleteXmlDocument_)> XmlDocumentUPtr_;

typedef std::vector<XmlDocumentUPtr_, Log4JAllocator<XmlDocumentUPtr_>> XmlDocumentsVector_;

struct Log4JEventSource_
{
    XmlDocumentsVector_ Docs;
    char *OwnXmlString;
};

static Log4JStatus Log4JEventSourceInitXmlStringImpl (Log4JEventSource **self, char *xmlString, bool ownString)
{
    *self = nullptr;

    Log4JEventSource *result = (Log4JEventSource *) Alloc_ (sizeof *result);
    if (result == nullptr)
    {
        return Log4JStatus::E_MEMORY_ERROR;
    }

    XmlDocumentsVector_ &docs = result->Docs;
    new(&docs) XmlDocumentsVector_ ();
    result->OwnXmlString = ownString ? xmlString : nullptr;

    auto status = Log4JStatus::E_SUCCESS;
    auto nextPart = xmlString;

    while (nextPart)
    {
        auto doc = (XmlDocument_ *) Alloc_ (sizeof XmlDocument_);
        if (doc == nullptr)
        {
            docs.~XmlDocumentsVector_ ();
            Free_ (result);
            return Log4JStatus::E_MEMORY_ERROR;
        }
        new(doc) XmlDocument_ ();

        doc->set_allocator (Alloc_, Free_);
        docs.push_back (XmlDocumentUPtr_ (doc, &DeleteXmlDocument_));
        try
        {
            doc->parse<rapidxml::parse_default> (nextPart);
            nextPart = nullptr;
        }
        catch (const rapidxml::parse_error &ex)
        {
            status = Log4JStatus::E_DOCUMENT_ERRORS;
            nextPart = strstr (ex.where<char> (), LogFragmentStart_);
        }
    }

    *self = result;

    return status;
}

LOG4JPARSERC_API Log4JStatus __cdecl Log4JEventSourceInitXmlString (Log4JEventSource **self, char *xmlString)
{
    return Log4JEventSourceInitXmlStringImpl (self, xmlString, false);
}

LOG4JPARSERC_API void __cdecl Log4JEventSourceDestroy (Log4JEventSource *self)
{
    self->Docs.~XmlDocumentsVector_ ();

    if (self->OwnXmlString)
    {
        Free_ (self->OwnXmlString);
    }

    Free_ (self);
}

static size_t IndexOfCurrentDocument (const XmlDocumentsVector_ *docs, const rapidxml::xml_node<char> *node)
{
    auto currentDocument = node->document ();
    auto docsSize = docs->size ();
    for (size_t i = 0; i < docsSize; ++i)
    {
        if (docs->at (i).get () == currentDocument)
        {
            return i;
        }
    }

    return docsSize;
}

LOG4JPARSERC_API Log4JEvent __cdecl Log4JEventSourceFirst (const Log4JEventSource *self)
{
    auto node = self->Docs.front ()->first_node (TagEvent_, TagEventSize_);
    return node;
}

LOG4JPARSERC_API Log4JEvent __cdecl Log4JEventSourceNext (const Log4JEventSource *self, const Log4JEvent event)
{
    const XmlDocumentsVector_ &docs = self->Docs;
    auto node = (rapidxml::xml_node<char> *) event;

    auto nextNode = node->next_sibling (TagEvent_, TagEventSize_);
    if (!nextNode)
    {
        auto currentDocumentIndex = IndexOfCurrentDocument (&self->Docs, node);
        auto nextDocumentIndex = currentDocumentIndex + 1;
        if (nextDocumentIndex < docs.size ())
        {
            nextNode = docs.at (nextDocumentIndex)->first_node (TagEvent_, TagEventSize_);
        }
    }

    return nextNode;
}

LOG4JPARSERC_API Log4JEvent __cdecl Log4JEventSourceLast (const Log4JEventSource *self)
{
    auto node = self->Docs.back ()->last_node (TagEvent_, TagEventSize_);
    return node;
}

LOG4JPARSERC_API Log4JEvent __cdecl Log4JEventSourcePrev (const Log4JEventSource *self, const Log4JEvent event)
{
    const XmlDocumentsVector_ &docs = self->Docs;
    auto node = (rapidxml::xml_node<char> *) event;

    auto prevNode = node->previous_sibling (TagEvent_, TagEventSize_);
    if (!prevNode)
    {
        auto currentDocumentIndex = IndexOfCurrentDocument (&self->Docs, node);
        auto prevDocumentIndex = currentDocumentIndex - 1;
        // if currentDocumentIndex == 0 then prevDocumentIndex would overflow to SIZE_MAX
        if (prevDocumentIndex < docs.size ())
        {
            prevNode = docs.at (prevDocumentIndex)->first_node (TagEvent_, TagEventSize_);
        }
    }

    return prevNode;
}
