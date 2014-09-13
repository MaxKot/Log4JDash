#include "stdafx.h"
#include "level_parser.h"

template<>
level_parser<char>::level_parser () {
    _levels.add ("OFF", std::numeric_limits<int>::max ());
    //_levels.add ("log4net:DEBUG", 120000);
    _levels.add ("EMERGENCY", 120000);
    _levels.add ("FATAL", 110000);
    _levels.add ("ALERT", 100000);
    _levels.add ("CRITICAL", 90000);
    _levels.add ("SEVERE", 80000);
    _levels.add ("ERROR", 70000);
    _levels.add ("WARN", 60000);
    _levels.add ("NOTICE", 50000);
    _levels.add ("INFO", 40000);
    _levels.add ("DEBUG", 30000);
    _levels.add ("FINE", 30000);
    _levels.add ("TRACE", 20000);
    _levels.add ("FINER", 20000);
    _levels.add ("VERBOSE", 10000);
    _levels.add ("FINEST", 10000);
    _levels.add ("ALL", std::numeric_limits<int>::min ());
}

template<>
level_parser<char> level_parser<char>::instance;
