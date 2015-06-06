log4JLevels = [
    ('ALERT', '100000'),
    ('ALL', 'INT_MIN'),
    ('CRITICAL', '90000'),
    ('DEBUG', '30000'),
    ('EMERGENCY', '120000'),
    ('ERROR', '70000'),
    ('FATAL', '110000'),
    ('FINE', '30000'),
    ('FINER', '20000'),
    ('FINEST', '10000'),
    ('INFO', '40000'),
    ('NOTICE', '50000'),
    ('OFF', 'INT_MAX'),
    ('SEVERE', '80000'),
    ('TRACE', '20000'),
    ('VERBOSE', '10000'),
    ('WARN', '60000'),
    #('log4net:DEBUG', '120000')
    ]

class PrefixTreeNode(object):
    def __init__(self):
        self.Value = -1;
        self.Children = [None for _ in range(26)];

    def add(self, name, value):
        if len(name) == 0:
            self.Value = value
        else:
            i = ord(name[0]) - ord ('A')
            self.Children[i] = PrefixTreeNode()
            self.Children[i].add(name[1:], value)

root = PrefixTreeNode()
for (name, value) in log4JLevels:
    root.add(name, value)

def node_c_id(parent_node_name, child_node_index = None):
    if child_node_index is None:
        node_name = parent_node_name
    else:
        ch = chr(child_node_index + ord('A'))
        node_name = parent_node_name + ch
    if node_name != '':
        result ='Level{}_'.format(node_name.title())
    else:
        result = 'Levels_'
    return result

def tree_node_ref(tree_node, node_name, child_node_index):
    if tree_node.Children[child_node_index] is None:
        result = 'NULL'
    else:
        c_id = node_c_id (node_name, child_node_index)
        result = '&{}'.format(c_id)
    return result

def tree_node_slot_name(tree_node, node_name, child_node_index):
    ch = chr(child_node_index + ord('A'))
    child_ref = tree_node_ref(tree_node, node_name, child_node_index)
    l = len(child_ref)
    return ch.rjust(l)

def print_tree(tree_node, node_name = ''):
    if tree_node is None:
        raise 't is None'
    
    for i in range(len(tree_node.Children)):
        child = tree_node.Children[i]
        if child is not None:
            ch = chr(i + ord('A'))
            childId = node_name + ch
            print_tree(child, childId)
    print('static const LevelTreeNode_ {} ='.format(node_c_id(node_name)))
    print('{')
    
    print('    {},'.format(tree_node.Value))

    slot_names = [tree_node_slot_name(tree_node, node_name, i) for i in range(len(tree_node.Children))]
    childrefs = [tree_node_ref(tree_node, node_name, i) for i in range(len(tree_node.Children))]
    print('    //{}'.format('  '.join(slot_names)))
    print('    {{ {} }}'.format(', '.join(childrefs)))
    
    print('};')

print('#pragma region Levels definition')
print()

print_tree(root)

print()
print('#pragma endregion')
