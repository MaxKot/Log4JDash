using System.Collections.Generic;
using Log4JDash.Web.Configuration;
using Log4JDash.Web.Domain;

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
            var source = logSourceProvider_.GetSource (query.SourceId);
            return source.GetEvents (query);
        }
    }
}
