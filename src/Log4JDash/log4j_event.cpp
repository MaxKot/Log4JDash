#include "stdafx.h"
#include "log4j_event.h"

const char log4j_event<char>::zero = '0';

const char log4j_event<char>::attr_level[] = "level";
const size_t log4j_event<char>::attr_level_size = sizeof (attr_level) - 1U;

const char log4j_event<char>::attr_logger[] = "logger";
const size_t log4j_event<char>::attr_logger_size = sizeof (attr_logger) - 1U;

const char log4j_event<char>::attr_thread[] = "thread";
const size_t log4j_event<char>::attr_thread_size = sizeof (attr_thread) - 1U;

const char log4j_event<char>::attr_timestamp[] = "timestamp";
const size_t log4j_event<char>::attr_timestamp_size = sizeof (attr_timestamp) - 1U;

const char log4j_event<char>::tag_message[] = "log4j:message";
const size_t log4j_event<char>::tag_message_size = sizeof (tag_message) - 1U;

const char log4j_event<char>::tag_throwable[] = "log4j:throwable";
const size_t log4j_event<char>::tag_throwable_size = sizeof (tag_throwable) - 1U;
