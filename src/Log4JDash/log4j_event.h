#pragma once

#include <rapidxml\rapidxml.hpp>
#include "fixed_string.h"

template<typename Ch = char>
class log4j_event {
private:
    static const Ch zero;

    static const Ch attr_level[];
    static const size_t attr_level_size;

    static const Ch attr_logger[];
    static const size_t attr_logger_size;

    static const Ch attr_thread[];
    static const size_t attr_thread_size;

    static const Ch attr_timestamp[];
    static const size_t attr_timestamp_size;

    static const Ch tag_message[];
    static const size_t tag_message_size;

    static const Ch tag_throwable[];
    static const size_t tag_throwable_size;

public:
    log4j_event (const rapidxml::xml_node<Ch> *node);

    fixed_string<Ch> get_level () const;
    fixed_string<Ch> get_logger () const;
    fixed_string<Ch> get_thread () const;
    fixed_string<Ch> get_timestamp () const;
    int64_t get_time () const;

    fixed_string<Ch> get_message () const;
    fixed_string<Ch> get_throwable () const;

private:
    inline fixed_string<Ch> get_value (const rapidxml::xml_base<Ch> *source) const;

    int64_t parse_timestamp (const Ch *value, const size_t value_size) const;

private:
    const rapidxml::xml_node<Ch> *_node;
};

template<typename Ch>
log4j_event<Ch>::log4j_event (const rapidxml::xml_node<Ch> *node)
    : _node (node) {

}

template<typename Ch>
fixed_string<Ch> log4j_event<Ch>::get_level () const {
    auto xml = _node->first_attribute (attr_level, attr_level_size);
    return get_value (xml);
}

template<typename Ch>
fixed_string<Ch> log4j_event<Ch>::get_logger () const {
    auto xml = _node->first_attribute (attr_logger, attr_logger_size);
    return get_value (xml);
}

template<typename Ch>
fixed_string<Ch> log4j_event<Ch>::get_thread () const {
    auto xml = _node->first_attribute (attr_thread, attr_thread_size);
    return get_value (xml);
}

template<typename Ch>
fixed_string<Ch> log4j_event<Ch>::get_timestamp () const {
    auto xml = _node->first_attribute (attr_timestamp, attr_timestamp_size);
    return get_value (xml);
}

template<typename Ch>
int64_t log4j_event<Ch>::get_time () const {
    auto xml = _node->first_attribute (attr_timestamp, attr_timestamp_size);
    auto result = parse_timestamp (xml->value (), xml->value_size ());

    return result;
}

template<typename Ch>
fixed_string<Ch> log4j_event<Ch>::get_message () const {
    auto xml = _node->first_node (tag_message, tag_message_size);
    return get_value (xml);
}

template<typename Ch>
fixed_string<Ch> log4j_event<Ch>::get_throwable () const {
    auto xml = _node->first_node (tag_throwable, tag_throwable_size);
    return get_value (xml);
}

template<typename Ch>
inline fixed_string<Ch> log4j_event<Ch>::get_value (const rapidxml::xml_base<Ch> *source) const {
    if (source) {
        auto value = source->value ();
        auto value_size = source->value_size ();
        return fixed_string<Ch> (value, value_size);
    } else {
        return fixed_string<Ch> ();
    }
}

template<typename Ch>
int64_t log4j_event<Ch>::parse_timestamp (const Ch *value, const size_t value_size) const {
    int64_t result = 0L;

    for (size_t i = 0; i < value_size; ++i) {
        int64_t digit = value[i] - zero;
        result = result * 10L + digit;
    }

    return result;
}
