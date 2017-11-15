using System.Collections.Generic;
using System.Diagnostics;
using Log4JDash.Web.Domain;

namespace Log4JDash.Web.Models
{
    internal sealed class LogRepository
    {
        private readonly LogSourceProvider logSourceProvider_;

        public LogRepository (LogSourceProvider logSourceProvider)
        {
            Debug.Assert (logSourceProvider != null, "LogRepository.ctor: logSourceProvider is null.");

            logSourceProvider_ = logSourceProvider;
        }

        public IEnumerable<string> GetSources ()
            => logSourceProvider_.GetSources ();

        public EventsCollection GetEvents (LogQuery query)
        {
            var source = logSourceProvider_.GetSource (query.SourceId);
            return source.GetEvents (query);
        }
    }
}
