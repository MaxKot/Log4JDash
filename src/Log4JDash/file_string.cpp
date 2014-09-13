#include "stdafx.h"
#include "file_string.h"
#include "time_trace.h"

file_string::file_string (const char * filename) {
    TIME_TRACE_BEGIN (load_file);

    _buffer = nullptr;
    long length;
    auto f = fopen (filename, "rb");

    if (f) {
        fseek (f, 0, SEEK_END);
        length = ftell (f);
        fseek (f, 0, SEEK_SET);
        _buffer = malloc (length + 1);

        if (_buffer) {
            fread (_buffer, 1, length, f);
            _str = reinterpret_cast<char *> (_buffer);
            _str[length] = '\0';
        }

        fclose (f);
    }

    TIME_TRACE_END (load_file);
}

file_string::~file_string () {
    if (_buffer) {
        free (_buffer);
        _buffer = nullptr;
        _str = nullptr;
    }
}

char *file_string::get () {
    return _str;
}
