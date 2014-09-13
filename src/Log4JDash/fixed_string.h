#pragma once

#include <ostream>

template<typename Ch = char>
class fixed_string {
public:
    fixed_string ();

    fixed_string (const Ch *value, const size_t size);

    const char *value () const;

    size_t size () const;

private:
    const Ch *_value;
    const size_t _size;
};

template<typename Ch>
fixed_string<Ch>::fixed_string ()
    : _value (nullptr)
    , _size (0UL) {

}

template<typename Ch>
fixed_string<Ch>::fixed_string (const Ch *value, const size_t size)
    : _value (value)
    , _size (size) {

}

template<typename Ch>
const char *fixed_string<Ch>::value () const {
    return _value;
}

template<typename Ch>
size_t fixed_string<Ch>::size () const {
    return _size;
}

template<typename Ch = char>
std::basic_ostream<Ch, std::char_traits<Ch>> &operator << (std::basic_ostream<Ch, std::char_traits<Ch>> &stream, fixed_string<Ch> &str) {
    stream.write (str.value (), str.size ());
    return stream;
}
