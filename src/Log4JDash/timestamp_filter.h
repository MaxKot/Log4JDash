#pragma once

#include "log4j_event.h"

template<typename Ch = char>
class timestamp_filter {
public:
    timestamp_filter (int64_t min, int64_t max);

    bool operator () (const log4j_event<Ch> *event) const;

private:
    int64_t _min;
    int64_t _max;
};

template<typename Ch>
timestamp_filter<Ch>::timestamp_filter (int64_t min, int64_t max)
    : _min (min)
    , _max (max) {

}

template<typename Ch>
bool timestamp_filter<Ch>::operator () (const log4j_event<Ch> *event) const {
    auto value = event->get_time ();

    return _min <= value && value <= _max;
}
