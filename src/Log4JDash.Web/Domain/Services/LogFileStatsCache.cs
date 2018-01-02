using System;
using System.Collections.Concurrent;
using System.Threading;
using Log4JParserNet;

namespace Log4JDash.Web.Domain.Services
{
    internal sealed partial class LogFileStatsCache
    {
        private readonly ConcurrentDictionary<Key, LogFileStats> impl_
            = new ConcurrentDictionary<Key, LogFileStats> (KeyComparer.Instance);

        private IPrecache precache_
            = NullPrecache.Instance;

        private void OnPrecacheThreadStop (PrecacheThread thread)
            => Interlocked.CompareExchange (ref precache_, NullPrecache.Instance, thread);

        public IDisposable StartPrecacheThread ()
        {
            PrecacheThread newThread = null;
            try
            {
                newThread = new PrecacheThread (this);
                var oldPrecache = Interlocked.CompareExchange (ref precache_, newThread, NullPrecache.Instance);
                if (!ReferenceEquals (oldPrecache, NullPrecache.Instance))
                {
                    throw new InvalidOperationException ("Precache thread is aleady running.");
                }

                return newThread;
            }
            catch (Exception ex)
            {
                try
                {
                    newThread?.Dispose ();
                }
                catch (Exception cleanupEx)
                {
                    throw new AggregateException (ex, cleanupEx);
                }

                throw;
            }
        }

        public LogFileStats GetStats (ILogFile logFile, Filter filter)
            => impl_.GetOrAdd (new Key (logFile, filter), k => LogFileStats.GatherStats (logFile, k.UnstatableFilter));

        public void Hint (ILogFile logFile, Filter filter)
            => precache_.EnqueuePrecache (logFile, filter);

        public static LogFileStatsCache Default { get; } = new LogFileStatsCache ();
    }
}
