#pragma once

#include "level_tree_node.h"

template<typename Ch>
class level_parser {
private:
    level_parser ();

public:
    int get_value (const Ch *level, size_t value_size = std::numeric_limits<size_t>::max ());

    static level_parser instance;

private:
    level_tree_node<char, int> _levels;
};

template<typename Ch>
int level_parser<Ch>::get_value (const Ch *level, size_t value_size = std::numeric_limits<size_t>::max ()) {
    return _levels.get_value (level, value_size);
}
