#pragma once

template<typename Ch, typename T>
class level_tree_node {
public:
    level_tree_node ();
    ~level_tree_node ();

    void add (const Ch *level, T value);
    T get_value (const Ch *value, size_t value_size = std::numeric_limits<size_t>::max ()) const;

private:
    static int get_index (Ch c);

private:
    level_tree_node* _children[26];
    T _value;
};

template<typename Ch, typename T>
level_tree_node<Ch, T>::level_tree_node ()
    : _value ({0}) {
    memset (&_children, 0, sizeof (_children));
}

template<typename Ch, typename T>
level_tree_node<Ch, T>::~level_tree_node () {
    for (auto i = 0; i < sizeof (_children) / sizeof (level_tree_node *); ++i) {
        if (_children[i]) {
            delete _children[i];
            _children[i] = nullptr;
        }
    }
}

template<typename Ch, typename T>
void level_tree_node<Ch, T>::add (const Ch *level, T value) {
    if (*level) {
        auto index = get_index (*level);

        auto child = _children[index];
        if (!child) {
            child = new level_tree_node ();
            _children[index] = child;
        }

        child->add (level + 1, value);
    } else {
        _value = value;
    }
}

template<typename Ch, typename T>
T level_tree_node<Ch, T>::get_value (const Ch *value, size_t value_size) const {
    T result;
    const level_tree_node *current_node = this;

    while (current_node) {
        result = current_node->_value;
        if (value_size <= 0 || !*value) {
            break;
        }

        auto idx = get_index (*value);
        current_node = current_node->_children[idx];

        ++value;
        --value_size;
    }

    if (value_size > 0 && *value) {
        throw std::exception ("Failed to match level name.");
    }

    return result;
}

template<typename Ch, typename T>
int level_tree_node<Ch, T>::get_index (Ch c) {
    return c - 'A';
}
