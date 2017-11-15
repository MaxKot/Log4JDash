using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal sealed partial class LogFileStatsCache
    {
        private sealed class NullPrecache : IPrecache
        {
            public static readonly IPrecache Instance = new NullPrecache ();

            public void EnqueuePrecache (ILogFile logFile, Filter filter)
            {

            }
        }
    }
}
