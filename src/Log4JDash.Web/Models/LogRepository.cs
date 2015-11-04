using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Log4JDash.Web.Configuration;
using Log4JDash.Web.Domain;
using Log4JParserNet;

namespace Log4JDash.Web.Models
{
    public sealed class LogRepository
    {
        private readonly LogSourceProvider logSourceProvider_ = new LogSourceProvider (KnownSections.LogSourceProvider ());

        private static void AddFilter
            (DisposableCollection<List<FilterBase>> filters, Func<FilterBase> filterFactory)
        {
            if (filters.Elements.Capacity < filters.Elements.Count + 1)
            {
                filters.Elements.Capacity = filters.Elements.Count + 1;
            }

            FilterBase filter = null;
            try
            {
                filter = filterFactory ();
            }
            finally
            {
                filters.Elements.Add (filter);
            }
        }

        public IEnumerable<string> GetSources ()
        {
            return logSourceProvider_.GetSources ();
        }

        public EventsCollection GetEvents (LogQuery query)
        {
            var source = logSourceProvider_.GetSource (query.Source.Value);

            using (var sourceStream = source.Open ())
            using (var logFile = Log4JFile.Create (sourceStream, query.Size))
            using (var filters = new List<FilterBase> ().ToDisposable ())
            {
                logFile.Encoding = Encoding.GetEncoding (1251);

                if (query.MinLevel.Value != Level.Debug)
                {
                    AddFilter (filters, () => new FilterLevel (query.MinLevel.Value, Level.MaxValue));
                }

                if (!String.IsNullOrWhiteSpace (query.Logger))
                {
                    AddFilter (filters, () => new FilterLogger (query.Logger));
                }

                if (!String.IsNullOrWhiteSpace (query.Message))
                {
                    AddFilter (filters, () => new FilterMessage (query.Message));
                }

                if (query.MinTime > DateTime.MinValue || query.MaxTime < DateTime.MaxValue)
                {
                    AddFilter (filters, () => new FilterTimestamp (query.MinTime, query.MaxTime));
                }

                IEnumerable<Event> filteredEvents;
                switch (filters.Elements.Count)
                {
                    case 0:
                        filteredEvents = logFile.GetEventsReverse ();
                        break;

                    case 1:
                        filteredEvents = logFile
                            .GetEventsReverse ()
                            .Where (filters.Elements.Single ());
                        break;

                    default:
                        FilterAll rootFilter = null;
                        try
                        {
                            rootFilter = new FilterAll ();
                            foreach (var filter in filters.Elements)
                            {
                                rootFilter.Add (filter);
                            }
                        }
                        finally
                        {
                            if (rootFilter != null)
                            {
                                filters.Elements.Add (rootFilter);
                            }
                        }
                        filteredEvents = logFile
                            .GetEventsReverse ()
                            .Where (rootFilter);
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
