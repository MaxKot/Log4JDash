using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using Log4JDash.Web.Domain;
using Log4JParserNet;

namespace Log4JDash.Web.Models
{
    public sealed class LogRepository
    {
        private sealed class LogDirectoryConfigMock : ILogDirectoryConfig
        {
            public string Name => "samples";

            public string DirectoryPath => Path.Combine (HostingEnvironment.MapPath ("~"), @"..\Log4JDash");

            public Regex FilenamePattern => new Regex (@".*\.xml");
        }

        private sealed class LogSourceProviderConfigMock : ILogSourceProviderConfig
        {
            public ICollection<ILogDirectoryConfig> Directories => new[] { new LogDirectoryConfigMock () };
        }

        private readonly LogSourceProvider logSourceProvider_ = new LogSourceProvider (new LogSourceProviderConfigMock ());

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

        public ICollection<EventModel> GetEvents (LogQuery query)
        {
            var sourceFile = logSourceProvider_.GetFile (query.Source.Value);

            using (var source = new Log4JFile (sourceFile))
            using (var filters = new List<FilterBase> ().ToDisposable ())
            {
                source.Encoding = Encoding.GetEncoding (1251);

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
                        filteredEvents = source.GetEventsReverse ();
                        break;

                    case 1:
                        filteredEvents = source
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
                        filteredEvents = source
                            .GetEventsReverse ()
                            .Where (rootFilter);
                        break;
                }

                var eventsWindow = filteredEvents
                    .Skip (query.Offset)
                    .Take (query.Quantity);

                var result = eventsWindow
                    .Select (x => new EventModel (x))
                    .ToList ();
                result.Reverse ();
                return result;
            }
        }
    }
}
