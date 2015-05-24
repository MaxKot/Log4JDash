#include "stdafx.h"
#include "log4j_timestamp.h"

log4j_timestamp::log4j_timestamp (int64_t value)
    : _value (value) {

}

std::ostream& operator<< (std::ostream &out, const log4j_timestamp &timestamp)
{
    auto calue = timestamp._value;
    time_t t = calue / 1000UL;
    auto millis = calue % 1000UL;
    tm tm;

    if (!localtime_s (&tm, &t)) {
        char tbuf[1024];
        strftime (tbuf, sizeof (tbuf), "%Y-%m-%d %H:%M:%S", &tm);

        out << tbuf << ".";
        out.width (3);
        out.fill ('0');
        out << millis;
    } else {
        out << "!!unknown";
    }

    return out;
}
