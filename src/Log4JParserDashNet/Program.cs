using System;
using System.Diagnostics;
using System.Linq;
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
                    using (var eventSource = Log4JFile.Create (filename))
                    using (new TimeTrace ("process inner"))
                    {
                        var filterAll = Filter.All
                        (
                            Filter.Timestamp (1411231371536L, 1411231371556L),
                            Filter.Not (Filter.Level ("INFO", "ERROR")),
                            Filter.Any
                            (
                                Filter.Message ("#2"),
                                Filter.Message ("#3")
                            ),
                            Filter.Logger ("Root.ChildB")
                        );

                        var matchingEvents = eventSource
                            .GetEvents ()
                            .Where (filterAll)
                            .Take (20)
                            .ToList ();

                        foreach (var @event in matchingEvents)
                        {
                            PrintEvent (@event);
                        }

                        Console.WriteLine ("Found events: {0}", matchingEvents.Count);
                    }
                }

                using (new TimeTrace ("count all events"))
                using (var eventSource = Log4JFile.Create (filename))
                {
                    var count = eventSource.GetEvents ().Count ();

                    Console.WriteLine ("Found events: {0}", count);
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
            var id = @event.Id;

            const string format = "{5}. {0:yyyy-MM-dd hh:mm:ss.fff} [{1}] {2} ({3}) {4}";
            Console.WriteLine (format, time, level, logger, thread, message, id);
            if (!String.IsNullOrWhiteSpace (throwable))
            {
                Console.WriteLine (throwable);
            }
        }
    }
}
