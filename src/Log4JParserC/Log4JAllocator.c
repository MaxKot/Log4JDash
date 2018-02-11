#include <stdlib.h>
#include "Log4JParserC.h"

static void* CallocWrapper_ (size_t size);

static Log4JAlloc *alloc_ = &CallocWrapper_;

static Log4JFree *free_ = &free;

static void* CallocWrapper_ (size_t size)
{
    return calloc (size, 1);
}

LOG4JPARSERC_API void Log4JSetDefaultAllocator ()
{
    Log4JSetAllocator (&CallocWrapper_, &free);
}

LOG4JPARSERC_API void Log4JSetAllocator (Log4JAlloc *alloc, Log4JFree *free)
{
    alloc_ = alloc;
    free_ = free;
}

void* Alloc_ (size_t size)
{
    return alloc_ (size);
}

void Free_ (void* block)
{
    free_ (block);
}
