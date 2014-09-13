#include "stdafx.h"
#include "logger_filter.h"

logger_filter<char>::logger_filter (const char *logger)
    : _logger (logger)
    , _logger_size (strlen (logger)) {

}

bool logger_filter<char>::operator () (const log4j_event<char> *event) const {
    auto value = event->get_logger ();

    return value.size () >= _logger_size && strncmp (value.value (), _logger, _logger_size) == 0;
}
