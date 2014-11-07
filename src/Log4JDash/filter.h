#pragma once

#include "log4j_event.h"
#include "level_parser.h"

typedef bool filter (void *context, const log4j_event<> *event);

// Level filter

typedef struct _filter_level_context filter_level_context;

void filter_level_init_i (filter_level_context **context, int32_t min, int32_t max);
void filter_level_init_c (filter_level_context **context, const char *min, const char *max);

void filter_level_destroy (filter_level_context *context);

bool filter_level (void *context, const log4j_event<> *event);

// Logger filter

typedef struct _filter_logger_context filter_logger_context;

void filter_logger_init_fs (filter_logger_context **context, const char *logger, const size_t logger_size);
void filter_logger_init_nt (filter_logger_context **context, const char *logger);

void filter_logger_destroy (filter_logger_context *context);

bool filter_logger (void *context, const log4j_event<> *event);

// Message filter

typedef struct _filter_message_context filter_message_context;

void filter_message_init_fs (filter_message_context **context, const char *message, const size_t message_size);
void filter_message_init_nt (filter_message_context **context, const char *message);

void filter_message_destroy (filter_message_context *context);

bool filter_message (void *context, const log4j_event<> *event);

// Timestamp filter

typedef struct _filter_timestamp_context filter_timestamp_context;

void filter_timestamp_init (filter_timestamp_context **context, int64_t min, int64_t max);

void filter_timestamp_destroy (filter_timestamp_context *context);

bool filter_timestamp (void *context, const log4j_event<> *event);
