using System;
using System.Collections.Generic;
using System.Linq;
using Log4JDash.Web.Configuration;
using Log4JDash.Web.Domain;
using Log4JParserNet;

namespace Log4JDash.Web.Models
{
    public sealed class LogRepository
    {
        private readonly LogSourceProvider logSourceProvider_ = new LogSourceProvider (KnownSections.LogSourceProvider ());

        public IEnumerable<string> GetSources ()
        {
            return logSourceProvider_.GetSources ();
        }

        public EventsCollection GetEvents (LogQuery query)
        {
            var sourceModel = query.Source.Value;
            var source = logSourceProvider_.GetSource (sourceModel?.Id);

            using (var logFile = source.Open (sourceModel?.Size))
            {
                var filters = new List<FilterBuilder> ();
                if (query.MinLevel.Value != Level.Debug)
                {
                    var filter = FilterBuilder.Level (query.MinLevel.Value, Level.MaxValue);
                    filters.Add (filter);
                }

                if (!String.IsNullOrWhiteSpace (query.Logger))
                {
                    var filter = FilterBuilder.Logger (query.Logger);
                    filters.Add (filter);
                }

                if (!String.IsNullOrWhiteSpace (query.Message))
                {
                    var filter = FilterBuilder.Message (query.Message);
                    filters.Add (filter);
                }

                if (query.MinTime > DateTime.MinValue || query.MaxTime < DateTime.MaxValue)
                {
                    var filter = FilterBuilder.Timestamp (query.MinTime, query.MaxTime);
                    filters.Add (filter);
                }

                IEnumerableOfEvents allEvents = logFile.GetEventsReverse ();

                IEnumerable<Event> filteredEvents;
                switch (filters.Count)
                {
                    case 0:
                        filteredEvents = allEvents;
                        break;

                    case 1:
                        using (var filter = filters.Single ().Build ())
                        {
                            filteredEvents = allEvents.Where (filter);
                        }
                        break;

                    default:
                        var rootFilter = FilterBuilder.All (filters);
                        using (var filter = rootFilter.Build ())
                        {
                            filteredEvents = allEvents.Where (filter);
                        }
                        break;
                }

                var eventsWindow = filteredEvents
                    .Skip (query.Offset)
                    .Take (query.Quantity);

                var events = eventsWindow
                    .Select (x => new EventModel (x))
                    .ToList ();
                events.Reverse ();
                var sourceName = source.Name;
                var sourceSize = logFile.Size;
                return new EventsCollection (events, sourceName, sourceSize);
            }
        }
    }
}
