#pragma once

#include "time_trace.h"

class file_string {
public:
    file_string (const char *filename);

    ~file_string ();

    char *get ();

private:
    void *_buffer;
    char *_str;
};
