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

        new public string Logger { get; }

        public FilterLoggerBuilder (string logger)
        {
            Logger = logger;
        }

        public override bool Equals (object obj)
            => obj is FilterLoggerBuilder other && Equals (other);

        public bool Equals (FilterLoggerBuilder other)
            => other != null && LoggerComparer.Equals (Logger, other.Logger);

        public override int GetHashCode ()
            => -2065107890 + LoggerComparer.GetHashCode (Logger);

        public override Filter Build ()
        {
            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitLoggerNt (out result, Logger);
                return Filter.Simple (result);
            }
            catch (Exception ex)
            {
                Disposable.DisposeAggregateErrors (result, ex);
                throw;
            }
        }

        public override void AcceptVisitor (IFilterBuilderVisitor visitor)
            => (visitor ?? throw new ArgumentNullException (nameof (visitor))).Visit (this);
    }
}
