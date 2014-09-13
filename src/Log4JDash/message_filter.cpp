#include "stdafx.h"
#include "message_filter.h"

message_filter<char>::message_filter (const char *message)
    : _message (message)
    , _message_size (strlen (message)) {

}

bool message_filter<char>::operator () (const log4j_event<char> *event) const {
    if (_message_size == 0) {
        return true;
    }

    auto message = event->get_message ();
    size_t value_size = message.size ();

    if (value_size < _message_size) {
        return false;
    }

    const char *value = message.value ();
    const char message_head = *_message;
    const char *message_tail = _message + 1;
    const size_t message_tail_size = _message_size - 1;

    do {
        char sc;

        do {
            sc = *value;
            ++value;
            --value_size;

            if (value_size < message_tail_size) {
                return false;
            }
        } while (sc != message_head);
    } while (strncmp (value, message_tail, message_tail_size) != 0);

    return true;
}
