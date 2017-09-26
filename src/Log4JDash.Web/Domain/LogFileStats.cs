using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogFileStats
    {
        public struct EventGroupKey : IEquatable<EventGroupKey>
        {
            public string Level { get; }

            public string Logger { get; }

            public EventGroupKey (string level, string logger)
            {
                Level = level;
                Logger = logger;
            }

            public EventGroupKey (Event @event)
                : this (@event.Level, @event.Logger ?? String.Empty)
            {

            }

            public override bool Equals (object obj)
                => obj is EventGroupKey other && Equals (other);

            public bool Equals (EventGroupKey other)
                => FilterLevel.LevelComparer.Equals (Level, other.Level) &&
                   FilterLogger.LoggerComparer.Equals (Logger, other.Logger);

            public override int GetHashCode ()
            {
                var hashCode = 726632474;
                hashCode = hashCode * -1521134295 + base.GetHashCode ();
                hashCode = hashCode * -1521134295 + FilterLevel.LevelComparer.GetHashCode (Level);
                hashCode = hashCode * -1521134295 + FilterLogger.LoggerComparer.GetHashCode (Logger);
                return hashCode;
            }
        }

        private readonly string fileName_;

        private readonly long size_;

        public int EventCount { get; }

        public IReadOnlyDictionary<EventGroupKey, int> GroupStats { get; }

        public Int64 EarliestTimestamp { get; }

        public Int64 LatestTimestamp { get; }

        public LogFileStats
        (
            string fileName,
            long size,
            int eventCount,
            IReadOnlyDictionary<EventGroupKey, int> groupStats,
            Int64 earliestTimestamp,
            Int64 latestTimestamp
        )
        {
            Debug.Assert (groupStats != null, "LogFileStats.ctor: groupStats is null.");

            fileName_ = fileName;
            size_ = size;
            EventCount = eventCount;
            GroupStats = groupStats;
            EarliestTimestamp = earliestTimestamp;
            LatestTimestamp = latestTimestamp;
        }

        public static LogFileStats GatherStats (ILogFile logFile, Filter filter)
        {
            var events = logFile.GetEvents ();
            var filteredEvents = filter != null
                ? events.Where (filter)
                : events;

            var stats = filteredEvents.Aggregate
            (
                new
                {
                    EventCount = 0,
                    GroupStats = new Dictionary<EventGroupKey, int> (),
                    EarliestTimestamp = Timestamp.MaxValue,
                    LatestTimestamp = Timestamp.MinValue
                },
                (a, e) =>
                {
                    var ts = e.Timestamp;

                    var gs = a.GroupStats;
                    var groupKey = new EventGroupKey (e);
                    if (gs.TryGetValue (groupKey, out var groupCount))
                    {
                        gs[groupKey] = groupCount + 1;
                    }
                    else
                    {
                        gs.Add (groupKey, 1);
                    }

                    return new
                    {
                        EventCount = a.EventCount + 1,
                        GroupStats = gs,
                        EarliestTimestamp = ts < a.EarliestTimestamp ? ts : a.EarliestTimestamp,
                        LatestTimestamp = ts > a.LatestTimestamp ? ts : a.LatestTimestamp
                    };
                }
            );

            var eventCount = stats.EventCount;
            var groupStats = stats.GroupStats;
            Int64 earliestTimestamp;
            Int64 latestTimestamp;
            if (eventCount > 0)
            {
                earliestTimestamp = stats.EarliestTimestamp;
                latestTimestamp = stats.LatestTimestamp;
            }
            else
            {
                earliestTimestamp = Timestamp.MinValue;
                latestTimestamp = Timestamp.MaxValue;
            }

            var result = new LogFileStats
                (logFile.FileName, logFile.Size, eventCount, groupStats, earliestTimestamp, latestTimestamp);
            return result;
        }
    }
}
