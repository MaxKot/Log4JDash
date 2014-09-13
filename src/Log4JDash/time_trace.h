#pragma once

#include <ctime>

#define TIME_TRACE

#ifdef TIME_TRACE
#define TIME_TRACE_BEGIN(name) clock_t name = clock ();
#else
#define TIME_TRACE_BEGIN(name)
#endif

#ifdef TIME_TRACE
#define TIME_TRACE_ENDM(name, message)                                  \
{                                                                       \
    clock_t name##_end = clock ();                                      \
    double name##_secs = double (name##_end - name) / CLOCKS_PER_SEC;   \
    printf (message " Elapsed time: %f sec.\n", name##_secs);           \
}
#else
#define TIME_TRACE_ENDM(name, message)
#endif

#define TIME_TRACE_END(name) TIME_TRACE_ENDM(name, "Time trace: " #name ".")
