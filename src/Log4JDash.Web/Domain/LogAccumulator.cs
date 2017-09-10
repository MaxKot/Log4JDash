using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Log4JDash.Web.Models;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogAccumulator
    {
        private readonly LogFileStatsCache statsCache_;

        private readonly LogQuery query_;

        private readonly EventGroupFilter groupFilter_;

        private readonly List<EventModel> events_;

        public IReadOnlyList<EventModel> Events => events_;

        private int skipRemaining_;

        public bool IsComplete => events_.Count == query_.Quantity;

        public LogAccumulator (LogFileStatsCache statsCache, LogQuery query)
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

        public LogAccumulator Consume (LazyLogFile logFile)
        {
            if (IsComplete)
            {
                return this;
            }

            var stats = statsCache_.GetOrAdd (logFile, query_);
            if (stats.LatestTimestamp < query_.MinTime || stats.EarliestTimestamp > query_.MaxTime)
            {
                return this;
            }

            var matchingEvents = stats.GroupStats
                .Where (kvp => groupFilter_.Apply (kvp.Key))
                .Sum (kvp => kvp.Value);

            if (matchingEvents > skipRemaining_)
            {
                var filterBuilder = query_.CreateFilter ();
                using (var filter = filterBuilder?.Build ())
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
            }
            else
            {
                skipRemaining_ -= matchingEvents;
            }

            return this;
        }
    }
}
