using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterLoggerBuilder
        : FilterBuilder
        , IEquatable<FilterLoggerBuilder>
    {
        private static readonly IEqualityComparer<string> LoggerComparer
            = StringComparer.OrdinalIgnoreCase;

        private readonly string logger_;

        public FilterLoggerBuilder (string logger)
        {
            logger_ = logger;
        }

        public override bool Equals (object obj)
            => obj is FilterLoggerBuilder other && Equals (other);

        public bool Equals (FilterLoggerBuilder other)
            => other != null && LoggerComparer.Equals (logger_, other.logger_);

        public override int GetHashCode ()
            => -2065107890 + LoggerComparer.GetHashCode (logger_);

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
