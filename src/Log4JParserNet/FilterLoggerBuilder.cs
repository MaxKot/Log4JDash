using System;

namespace Log4JParserNet
{
    public sealed class FilterLoggerBuilder : FilterBuilder
    {
        private readonly string logger_;

        public FilterLoggerBuilder (string logger)
        {
            logger_ = logger;
        }

        public override Filter Build ()
        {
            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitLoggerNt (out result, logger_);
                return Filter.Simple (result);
            }
            catch (Exception ex)
            {
                Disposable.DisposeAggregateErrors (result, ex);
                throw;
            }
        }
    }
}
