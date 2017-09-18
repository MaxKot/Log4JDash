using Log4JParserNet;

namespace Log4JDash.Web.Domain
{
    internal interface ILogFileStatsProvider
    {
        LogFileStats GetStats (LazyLogFile logFile, FilterBuilder filter);
    }
}
