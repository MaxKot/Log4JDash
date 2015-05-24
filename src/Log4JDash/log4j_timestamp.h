#pragma once

#include <iostream>

class log4j_timestamp {
public:
    log4j_timestamp (int64_t value);

    friend std::ostream& operator<< (std::ostream &out, const log4j_timestamp &timestamp);

private:
    int64_t _value;
};

std::ostream& operator<< (std::ostream &out, const log4j_timestamp &timestamp);
