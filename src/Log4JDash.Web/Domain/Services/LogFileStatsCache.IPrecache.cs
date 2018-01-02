using Log4JParserNet;

namespace Log4JDash.Web.Domain.Services
{
    internal sealed partial class LogFileStatsCache
    {
        private interface IPrecache
        {
            void EnqueuePrecache (ILogFile logFile, Filter filter);
        }
    }
}
