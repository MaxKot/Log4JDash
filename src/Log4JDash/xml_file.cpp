#include "stdafx.h"
#include "xml_file.h"

using rapidxml::xml_document;
using rapidxml::parse_fastest;

xml_file::xml_file (const char *filename) : _str (filename) {
    TIME_TRACE_BEGIN (parse_xml);

    _doc.parse<parse_fastest> (_str.get ());

    TIME_TRACE_END (parse_xml);
}

xml_document<> &xml_file::document () {
    return _doc;
}
