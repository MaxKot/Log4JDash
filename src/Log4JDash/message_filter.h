#pragma once

#include "log4j_event.h"

template<typename Ch = char>
class message_filter {
public:
    message_filter (const Ch *message, const size_t message_size);

    message_filter (const Ch *message);

    bool operator () (const log4j_event<Ch> *event) const;

private:
    const Ch *_message;
    const size_t _message_size;
};

template<typename Ch>
message_filter<Ch>::message_filter (const Ch *message, const size_t message_size)
    : _message (message)
    , _message_size (message_size) {

}
