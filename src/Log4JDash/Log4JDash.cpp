// Log4JDash.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "time_trace.h"
#include "xml_file.h"
#include "timestamp_filter.h"
#include "level_filter.h"
#include "logger_filter.h"
#include "message_filter.h"
#include "log4j_event.h"
#include <ctime>

using namespace std;
using namespace rapidxml;

void print_event (const log4j_event<> &event) {
    auto level = event.get_level ();
    auto logger = event.get_logger ();
    auto thread = event.get_thread ();
    auto message = event.get_message ();
    auto throwable = event.get_throwable ();

    auto time = event.get_time ();
    time_t t = time / 1000UL;
    auto millis = time % 1000UL;
    auto tm = localtime (&t);
    char tbuf[1024];
    strftime (tbuf, sizeof (tbuf), "%Y-%m-%d %H:%M:%S", tm);

    cout << tbuf << ".";
    cout.width (3);
    cout.fill ('0');
    cout << millis;
    cout << " [" << level << "] " << logger << " (" << thread << ") " << message << endl;
    if (throwable.size () > 0) {
        cout << throwable << endl;
    }
}

void parse_xml (const char *filename) {
    TIME_TRACE_BEGIN (total);

    xml_file doc (filename);

    TIME_TRACE_BEGIN (process);

    //timestamp_filter<> ts_filter (1407784678030L, 1407784678050L);
    //level_filter<> lvl_filter ("INFO", "ERROR");
    //logger_filter<> logger_filter ("Root.ChildB");
    //message_filter<> message_filter ("#4");
    timestamp_filter<> ts_filter (1411231371536L, 1411231371556L);
    level_filter<> lvl_filter ("INFO", "ERROR");
    logger_filter<> logger_filter ("Root.ChildB");
    message_filter<> message_filter ("#2");

    auto count = 0;
    auto node = log4j_event<>::first_node (doc.document ());

    for (; node; node = log4j_event<>::next_sibling (node)) {
        log4j_event<> event (node);

        auto match =
            ts_filter (&event) &&
            lvl_filter (&event) &&
            logger_filter (&event) &&
            message_filter (&event);

        if (match) {
            print_event (event);
            ++count;
        }
    }

    cout << "Found events: " << count << endl;

    TIME_TRACE_END (process);

    TIME_TRACE_END (total);
}

int main (int argc, char **argv)
{
    char buffer_in[10 * 1204];

    //fgets (buffer_in, sizeof (buffer_in), stdin);

    setlocale (LC_ALL, "ru-RU");
    const char filename[] = "test-log.cyr.xml";

    parse_xml (filename);

    return 0;
}
