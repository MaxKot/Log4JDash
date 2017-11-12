using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal sealed partial class LogFileStatsCache
    {
        private interface IPrecache
        {
            void EnqueuePrecache (ILogFile logFile, Filter filter);
        }
    }
}
