#pragma once

#include <rapidxml\rapidxml.hpp>
#include "file_string.h"
#include "time_trace.h"

class xml_file {
public:
    xml_file (const char *filename);

    const rapidxml::xml_document<> *document ();

private:
    file_string _str;
    rapidxml::xml_document<> _doc;
};
