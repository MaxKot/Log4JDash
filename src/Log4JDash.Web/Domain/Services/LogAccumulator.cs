using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Log4JDash.Web.Models;
using Log4JParserNet;

namespace Log4JDash.Web.Domain.Services
{
    internal sealed class CacheHintMaker
    {
        private readonly LogFileStatsCache statsCache_;

        private readonly Filter filter_;

        private LogAccumulator accumulator_;

        public IReadOnlyList<EventModel> Events => accumulator_.Events;

        public CacheHintMaker
            (LogFileStatsCache statsCache, ILogQuery query, LogAccumulator accumulator)
        {
            Debug.Assert (statsCache != null, "PrecacheScheduler.ctor: statsCache is null.");
            Debug.Assert (query != null, "PrecacheScheduler.ctor: query is null.");
            Debug.Assert (accumulator != null, "PrecacheScheduler.ctor: accumulator is null.");

            statsCache_ = statsCache;
            filter_ = query.CreateFilter ();
            accumulator_ = accumulator;
        }

        public CacheHintMaker Consume (ILogFile logFile)
        {
            if (accumulator_.IsComplete)
            {
                statsCache_.Hint (logFile, filter_);
            }
            else
            {
                accumulator_ = accumulator_.Consume (logFile);
            }

            return this;
        }
    }

    internal sealed class LogAccumulator
    {
        private readonly LogFileStatsCache statsCache_;

        private readonly ILogQuery query_;

        private readonly EventGroupFilter groupFilter_;

        private readonly List<EventModel> events_;

        public IReadOnlyList<EventModel> Events => events_;

        private int skipRemaining_;

        public bool IsComplete => events_.Count == query_.Quantity;

        public LogAccumulator (LogFileStatsCache statsCache, ILogQuery query)
        {
            Debug.Assert (statsCache != null, "LogAccumulator.ctor: statsCache is null.");
            Debug.Assert (query != null, "LogAccumulator.ctor: query is null.");
            statsCache_ = statsCache;
            query_ = query;

            var filter = query_.CreateFilter ();
            groupFilter_ = EventGroupFilter.Convert (filter);
            events_ = new List<EventModel> (query_.Quantity);
            skipRemaining_ = query_.Offset;
        }

        public LogAccumulator Consume (ILogFile logFile)
        {
            if (IsComplete)
            {
                return this;
            }

            var filter = query_.CreateFilter ();

            var stats = statsCache_.GetStats (logFile, filter);
            var matchesTimeWindow = query_.MinTimestamp <= stats.LatestTimestamp &&
                                    stats.EarliestTimestamp <= query_.MaxTimestamp;
            if (!matchesTimeWindow)
            {
                return this;
            }

            var matchingEvents = stats.GroupStats
                .Where (kvp => groupFilter_.Apply (kvp.Key))
                .Sum (kvp => kvp.Value);

            if (matchingEvents > skipRemaining_)
            {
                var fileEvents = logFile.GetEventsReverse ();
                var filteredFileEvents = filter != null
                    ? fileEvents.Where (filter)
                    : fileEvents;

                var selectedFileEvents = filteredFileEvents
                    .Skip (skipRemaining_)
                    .Take (query_.Quantity - events_.Count)
                    .Select (x => new EventModel (x));
                events_.AddRange (selectedFileEvents);

                skipRemaining_ = 0;
            }
            else
            {
                skipRemaining_ -= matchingEvents;
            }

            return this;
        }
    }
}
