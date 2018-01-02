using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Log4JParserNet;

namespace Log4JDash.Web.Domain.Services
{
    internal sealed partial class LogFileStatsCache
    {
        private sealed class PrecacheThread
            : IPrecache
            , IDisposable
        {
            private const int AddTimeoutMs = 500;

            private const int StopTimeoutMs = 5000;

            private readonly LogFileStatsCache statsCache_;

            private readonly CancellationTokenSource cancellation_;

            private readonly BlockingCollection<Tuple<ILogFile, Filter>> queries_;

            private readonly Task consumerThread_;

            public PrecacheThread (LogFileStatsCache statsCache)
            {
                System.Diagnostics.Debug.Assert (statsCache != null, "PrecacheThread.ctor: statsCache is null.");

                statsCache_ = statsCache;
                cancellation_ = new CancellationTokenSource ();
                queries_ = new BlockingCollection<Tuple<ILogFile, Filter>> ();
                consumerThread_ = Task.Run (() => ConsumerThread (), cancellation_.Token);
            }

            private void ConsumerThread ()
            {
                foreach (var query in queries_.GetConsumingEnumerable (cancellation_.Token))
                {
                    using (var file = query.Item1)
                    {
                        var filter = query.Item2;
                        statsCache_.GetStats (file, filter);
                    }
                }
            }

            public void EnqueuePrecache (ILogFile logFile, Filter filter)
            {
                var ownedLogFile = (ILogFile) logFile.Clone ();
                var query = Tuple.Create (ownedLogFile, filter);
                queries_.TryAdd (query, AddTimeoutMs, cancellation_.Token);
            }

            public void Dispose ()
            {
                statsCache_.OnPrecacheThreadStop (this);

                queries_.CompleteAdding ();
                consumerThread_.Wait (StopTimeoutMs);

                cancellation_.Cancel ();
                queries_.Dispose ();
                cancellation_.Dispose ();
            }
        }
    }
}
