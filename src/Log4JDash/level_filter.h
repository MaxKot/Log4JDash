#pragma once

#include "log4j_event.h"
#include "level_parser.h"

template<typename Ch = char>
class level_filter {
public:
    level_filter (int32_t min, int32_t max);

    level_filter (const Ch *min, const Ch *max);

    bool operator () (const log4j_event<Ch> *event_node) const;

private:
    int32_t _min;
    int32_t _max;
};

template<typename Ch>
level_filter<Ch>::level_filter (int32_t min, int32_t max)
    : _min (min)
    , _max (max) {

}

template<typename Ch>
level_filter<Ch>::level_filter (const Ch *min, const Ch *max)
    : _min (level_parser<Ch>::instance.get_value (min))
    , _max (level_parser<Ch>::instance.get_value (max)) {

}

template<typename Ch>
bool level_filter<Ch>::operator () (const log4j_event<Ch> *event) const {
    auto val_string = event->get_level ();
    int32_t value = level_parser<char>::instance.get_value (val_string.value (), val_string.size ());

    return _min <= value && value <= _max;
}
