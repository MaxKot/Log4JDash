#pragma once

#include "log4j_event.h"

template<typename Ch = char>
class logger_filter {
public:
    logger_filter (const Ch *logger, const size_t logger_size);

    logger_filter (const Ch *logger);

    bool operator () (const log4j_event<Ch> *event) const;

private:
    const Ch *_logger;
    const size_t _logger_size;
};

template<typename Ch>
logger_filter<Ch>::logger_filter (const Ch *logger, const size_t logger_size)
    : _logger (logger)
    , _logger_size (logger_size) {

}
