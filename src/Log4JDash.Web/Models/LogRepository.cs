using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Log4JParserNet;

namespace Log4JDash.Web.Models
{
    public sealed class LogRepository
    {
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

        public IEnumerable<EventModel> GetEvents (LogQuery query)
        {
            string sourceFile;
            switch (query.SourceId)
            {
                case 1:
                    sourceFile = Path.Combine (HostingEnvironment.MapPath ("~"), @"..\Log4JDash\test-log.cyr.xml");
                    break;

                default:
                    throw new ArgumentException ("Unrecognized log source identifier.", nameof (query));
            }

            using (var source = new FileEventSource (sourceFile))
            using (var filters = new List<FilterBase> ().ToDisposable ())
            {
                if (query.MinLevel.Value != Level.Debug)
                {
                    AddFilter (filters, () => new FilterLevel (query.MinLevel.Value, Level.Off));
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
                        filteredEvents = source;
                        break;

                    case 1:
                        filteredEvents = source.Where (filters.Elements.Single ());
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
                        filteredEvents = source.Where (rootFilter);
                        break;
                }

                IEnumerable<Event> eventsWindow;
                if (query.MinId == null)
                {
                    eventsWindow = filteredEvents
                        .TakeLast (query.Quantity);
                }
                else
                {
                    eventsWindow = filteredEvents
                        .Skip ((int) query.MinId)
                        .Take (query.Quantity);
                }

                return eventsWindow
                    .Select (x => new EventModel (x))
                    .ToList ();
            }
        }
    }
}
