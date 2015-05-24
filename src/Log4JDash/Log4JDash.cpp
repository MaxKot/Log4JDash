// Log4JDash.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <ctime>
#include <Event.h>
#include <Filter.h>
#include <Iterator.h>
#include "time_trace.h"
#include "xml_file.h"

using namespace std;
using namespace rapidxml;

void print_event (const Log4JParser::Event &event) {
    auto level = event.Level ();
    auto logger = event.Logger ();
    auto thread = event.Thread ();
    auto message = event.Message ();
    auto throwable = event.Throwable ();

    auto time = event.Timestamp ();
    time_t t = time / 1000UL;
    auto millis = time % 1000UL;
    tm tm;
    auto isTmValid = false;
    if (!localtime_s (&tm, &t)) {
        char tbuf[1024];
        strftime (tbuf, sizeof (tbuf), "%Y-%m-%d %H:%M:%S", &tm);

        cout << tbuf << ".";
        cout.width(3);
        cout.fill('0');
        cout << millis;
    } else {
        cout << "!!unknown";
    }

    cout << " [" << level << "] " << logger << " (" << thread << ") " << message << endl;
    if (throwable.Size () > 0) {
        cout << throwable << endl;
    }
}

void parse_xml (const char *filename) {
    TIME_TRACE_BEGIN (total);

    {
        file_string input (filename);

        TIME_TRACE_BEGIN (process);

        //timestamp_filter<> ts_filter (1407784678030L, 1407784678050L);
        //level_filter<> lvl_filter ("INFO", "ERROR");
        //logger_filter<> logger_filter ("Root.ChildB");
        //message_filter<> message_filter ("#4");

        //timestamp_filter<> ts_filter (1411231371536L, 1411231371556L);
        //level_filter<> lvl_filter ("INFO", "ERROR");
        //logger_filter<> logger_filter ("Root.ChildB");
        //message_filter<> message_filter ("#2");

        {
            Log4JParser::FilterTimestamp filter_ts (1411231371536L, 1411231371556L);
            Log4JParser::FilterLevel filter_lvl ("INFO", "ERROR");
            Log4JParser::FilterLogger filter_lgr ("Root.ChildB");
            Log4JParser::FilterMessage filter_msg1 ("#2");
            Log4JParser::FilterMessage filter_msg2 ("#3");

            Log4JParser::FilterNot filter_not (&filter_lvl);

            Log4JParser::FilterAny filter_any;
            filter_any.Add (&filter_msg1);
            filter_any.Add (&filter_msg2);

            Log4JParser::FilterAll filter_all;
            filter_all.Add (&filter_ts);
            filter_all.Add (&filter_not);
            filter_all.Add (&filter_any);
            filter_all.Add (&filter_lgr);

            Log4JParser::EventSource event_source (input.get ());
            Log4JParser::IteratorEventSource iterator_es (&event_source);

            Log4JParser::IteratorFilter iterator_filter (&iterator_es, &filter_all);

            auto count = 0;

            while (iterator_filter.MoveNext ()) {
                auto event = iterator_filter.Current ();
                print_event (event);
                ++count;
            }

            cout << "Found events: " << count << endl;
        }

        TIME_TRACE_END (process);

        TIME_TRACE_BEGIN(count_all);

        {
            Log4JParser::EventSource event_source(input.get());
            Log4JParser::IteratorEventSource iterator_es(&event_source);

            auto count = 0;

            while (iterator_es.MoveNext()) {
                auto event = iterator_es.Current();
                ++count;
            }

            cout << "Total number of events: " << count << endl;
        }

        TIME_TRACE_END(count_all);
    }

    TIME_TRACE_END (total);
}

int main (int argc, char **argv)
{
    _CrtSetDbgFlag (_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);

    //char buffer_in[10 * 1204];

    //fgets (buffer_in, sizeof (buffer_in), stdin);

    setlocale (LC_ALL, "ru-RU");
    const char filename[] = "test-log.cyr.xml";

    parse_xml (filename);

    return 0;
}
