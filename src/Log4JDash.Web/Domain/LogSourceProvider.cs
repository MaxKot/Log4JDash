using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Log4JDash.Web.Domain.Services;

namespace Log4JDash.Web.Domain
{
    internal sealed class LogSourceProvider
    {
        private readonly ILogSourceProviderConfig config_;

        private readonly LogFileStatsCache statsCache_;

        public LogSourceProvider (ILogSourceProviderConfig config, LogFileStatsCache statsCache)
        {
            Debug.Assert (config != null, "LogSourceProvider.ctor: config is null.");
            Debug.Assert (statsCache != null, "LogSourceProvider.ctor: statsCache is null.");

            config_ = config;
            statsCache_ = statsCache;
        }

        private IReadOnlyDictionary<string, LogSource> DoGetSources ()
        {
            var result = config_.Directories
                .Select (d => new LogSource (d, statsCache_))
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
