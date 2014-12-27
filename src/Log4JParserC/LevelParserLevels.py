z = [
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

class TN (object):
    def __init__ (self):
        self.V = -1;
        self.C = [None for x in range (26)];

def add (t, z):
    (n, v) = z
    if len (n) == 0:
        t.V = v
    else:
        i = ord (n[0]) - ord ('A')
        t.C[i] = TN ()
        add (t.C[i], (n[1:], v))

r = TN ()
for x in z: add (r, x)

def node_name (rp, i = None):
    if i is None:
        p = rp
    else:
        ch = chr(i + ord ('A'))
        p = rp + ch
    suf = p.title () if p != '' else 's'
    return 'Level{}_'.format (suf)

def tree_node_ref (r, rp, i):
    c = r.C[i]
    return 'NULL' if r.C[i] is None else '&{}'.format (node_name (rp, i))

def tree_node_name (r, rp, i):
    c = r.C[i]
    ch = chr(i + ord ('A'))
    l = len (tree_node_ref (r, rp, i))
    return ch.rjust (l)

def print_tree (t, p = ''):
    if t is None:
        raise 't is None'
    
    for i in range (26):
        c = t.C[i]
        if c is not None:
            ch = chr(i + ord ('A'))
            cp = p + ch
            print_tree (c, cp)
    print ('static const LevelTreeNode_ {} ='.format (node_name (p)))
    print ('{')
    
    print ('    {},'.format (t.V))
    
    childref_comment = '  '.join ([tree_node_name (t, p, i) for i in range (26)])
    print ('    //{}'.format (childref_comment))
    childrefs = ', '.join ([tree_node_ref (t, p, i) for i in range (26)])
    print ('    {{ {} }}'.format (childrefs))
    
    print ('};')

print ('#pragma region Levels definition')
print ()

print_tree (r)

print ()
print ('#pragma endregion')
