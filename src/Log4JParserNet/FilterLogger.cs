
namespace Log4JParserNet
{
    public sealed class FilterLogger : FilterBase
    {
        public FilterLogger (string logger)
            : base (Init (logger))
        {

        }

        private static FilterHandle Init (string logger)
        {
            FilterHandle result;
            Log4JParserC.Log4JFilterInitLoggerNt (out result, logger);

            return result;
        }
    }
}
