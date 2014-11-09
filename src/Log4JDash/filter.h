#pragma once

#include "log4j_event.h"
#include "level_parser.h"

typedef struct _filter filter;

void filter_destroy (filter *self);

bool filter_apply (const filter *self, const log4j_event<> *event);

// Level filter

void filter_init_level_i (filter **self, int32_t min, int32_t max);
void filter_init_level_c (filter **self, const char *min, const char *max);

// Logger filter

void filter_init_logger_fs (filter **self, const char *logger, const size_t logger_size);
void filter_init_logger_nt (filter **self, const char *logger);

// Message filter

void filter_init_message_fs (filter **self, const char *message, const size_t message_size);
void filter_init_message_nt (filter **self, const char *message);

// Timestamp filter

void filter_init_timestamp (filter **self, int64_t min, int64_t max);

// All filter

void filter_init_all (filter **self);

void filter_all_add (filter *self, filter *child_filter);
void filter_all_remove (filter *self, filter *child_filter);

// Any filter

void filter_init_any (filter **self);

void filter_any_add (filter *self, filter *child_filter);
void filter_any_remove (filter *self, filter *child_filter);

// Not filter

void filter_init_not (filter **self, filter *child_filter);
