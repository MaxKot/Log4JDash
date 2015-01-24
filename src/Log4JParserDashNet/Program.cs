using System;
using System.Diagnostics;
using Log4JParserNet;

namespace Log4JParserDashNet
{
    internal sealed class TimeTrace : IDisposable
    {
        private readonly string name_;

        private readonly Stopwatch stopwatch_;

        public TimeTrace (string name)
        {
            name_ = name;
            stopwatch_ = new Stopwatch ();
            stopwatch_.Start ();
        }

        public void Dispose ()
        {
            stopwatch_.Stop ();
            var seconds = stopwatch_.Elapsed.TotalSeconds;
            Console.WriteLine ("Time trace: {0}. Elapsed time: {1} sec.", name_, seconds);
        }
    }

    public static class Program
    {
        public static void Main (string[] args)
        {
            const string filename = "test-log.cyr.xml";

            ParseXml (filename);
        }

        private static void ParseXml (string filename)
        {
            using (new TimeTrace ("total"))
            {
                using (new TimeTrace ("process with E/S init"))
                {
                    using (var eventSource = new EventSource (filename))
                    using (var enumeratorEs = new EnumeratorEventSource (eventSource))
                    using (new TimeTrace ("process inner"))
                    using (var filterTs = new FilterTimestamp (1411231371536L, 1411231371556L))
                    using (var filterLvl = new FilterLevel ("INFO", "ERROR"))
                    using (var filterLgr = new FilterLogger ("Root.ChildB"))
                    using (var filterMsg1 = new FilterMessage ("#2"))
                    using (var filterMsg2 = new FilterMessage ("#3"))
                    using (var filterNot = new FilterNot (filterLvl))
                    using (var filterAny = new FilterAny ())
                    using (var filterAll = new FilterAll ())
                    {
                        filterAny.Add (filterMsg1);
                        filterAny.Add (filterMsg2);

                        filterAll.Add (filterTs);
                        filterAll.Add (filterNot);
                        filterAll.Add (filterAny);
                        filterAll.Add (filterLgr);

                        using (var enumeratorFilter = new EnumeratorFilter (enumeratorEs, filterAll))
                        {
                            var count = 0;

                            while (enumeratorFilter.MoveNext () && count < 20)
                            {
                                var @event = enumeratorFilter.Current;
                                PrintEvent (@event);
                                ++count;
                            }

                            Console.WriteLine ("Found events: {0}", count);
                        }
                    }
                }

                using (new TimeTrace ("count all events"))
                {
                    using (var eventSource = new EventSource (filename))
                    using (var enumeratorEs = new EnumeratorEventSource (eventSource))
                    {
                        var count = 0;

                        while (enumeratorEs.MoveNext ())
                        {
                            var @event = enumeratorEs.Current;
                            ++count;
                        }

                        Console.WriteLine ("Found events: {0}", count);
                    }
                }
            }
        }

        private static void PrintEvent (Event @event)
        {
            var level = @event.Level;
            var logger = @event.Logger;
            var thread = @event.Thread;
            var message = @event.Message;
            var throwable = @event.Throwable;
            var time = @event.Time;

            const string format = "{0:yyyy-MM-dd hh:mm:ss.fff} [{1}] {2} ({3}) {4}";
            Console.WriteLine (format, time, level, logger, thread, message);
            if (!String.IsNullOrWhiteSpace (throwable))
            {
                Console.WriteLine (throwable);
            }
        }
    }
}
