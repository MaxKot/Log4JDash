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
#include "filter.h"
#include "log4j_iterator.h"
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

    //timestamp_filter<> ts_filter (1411231371536L, 1411231371556L);
    //level_filter<> lvl_filter ("INFO", "ERROR");
    //logger_filter<> logger_filter ("Root.ChildB");
    //message_filter<> message_filter ("#2");

    filter *filter_ts;
    filter_init_timestamp (&filter_ts, 1411231371536L, 1411231371556L);
    filter *filter_lvl;
    filter_init_level_c (&filter_lvl, "INFO", "ERROR");
    filter *filter_lgr;
    filter_init_logger_nt (&filter_lgr, "Root.ChildB");
    filter *filter_msg1;
    filter_init_message_nt (&filter_msg1, "#2");
    filter *filter_msg2;
    filter_init_message_nt (&filter_msg2, "#3");

    filter *filter_not;
    filter_init_not (&filter_not, filter_lvl);

    filter *filter_any;
    filter_init_any (&filter_any);
    filter_any_add (filter_any, filter_msg1);
    filter_any_add (filter_any, filter_msg2);

    filter *filter_all;
    filter_init_all (&filter_all);
    filter_all_add (filter_all, filter_ts);
    filter_all_add (filter_all, filter_not);
    filter_all_add (filter_all, filter_any);
    filter_all_add (filter_all, filter_lgr);

    log4j_iterator *iterator_doc;
    log4j_iterator_init_document (&iterator_doc, doc.document ());

    log4j_iterator *iterator_filter;
    log4j_iterator_init_filter (&iterator_filter, iterator_doc, filter_all);

    auto count = 0;

    while (log4j_iterator_move_next (iterator_filter)) {
        auto event = log4j_iterator_current (iterator_filter);
        print_event (event);
        ++count;
    }

    cout << "Found events: " << count << endl;

    log4j_iterator_destroy (iterator_filter);
    log4j_iterator_destroy (iterator_doc);

    filter_destroy (filter_all);
    filter_destroy (filter_any);
    filter_destroy (filter_not);
    filter_destroy (filter_ts);
    filter_destroy (filter_msg1);
    filter_destroy (filter_msg2);
    filter_destroy (filter_lgr);
    filter_destroy (filter_lvl);

    TIME_TRACE_END (process);

    TIME_TRACE_END (total);
}

int main (int argc, char **argv)
{
    _CrtSetDbgFlag (_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);

    char buffer_in[10 * 1204];

    //fgets (buffer_in, sizeof (buffer_in), stdin);

    setlocale (LC_ALL, "ru-RU");
    const char filename[] = "test-log.cyr.xml";

    parse_xml (filename);

    return 0;
}
