using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogSourceProvider
    {
        private readonly ILogSourceProviderConfig config_;

        private readonly ILogFileStatsProvider statsProvider_;

        public LogSourceProvider (ILogSourceProviderConfig config, ILogFileStatsProvider statsProvider)
        {
            Debug.Assert (config != null, "LogSourceProvider.ctor: config is null.");
            Debug.Assert (statsProvider != null, "LogSourceProvider.ctor: statsProvider is null.");

            config_ = config;
            statsProvider_ = statsProvider;
        }

        private IReadOnlyDictionary<string, LogSource> DoGetSources ()
        {
            var result = config_.Directories
                .Select (d => new LogSource (d, statsProvider_))
                .Where (s => !s.IsEmpty ())
                .ToDictionary (s => s.Name);

            return result;
        }

        public IEnumerable<string> GetSources ()
        {
            return DoGetSources ().Keys.OrderBy (k => k);
        }

        public LogSource GetSource (string sourceId)
        {
            var sources = DoGetSources ();
            var key = String.IsNullOrWhiteSpace (sourceId)
                ? GetSources ().First ()
                : sourceId;

            try
            {
                return sources[key];
            }
            catch (KeyNotFoundException ex)
            {
                throw new ArgumentOutOfRangeException ("Invalid log source.", ex);
            }
        }
    }
}
