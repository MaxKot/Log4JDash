using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterLogger
        : Filter
        , IEquatable<FilterLogger>
    {
        public static readonly IEqualityComparer<string> LoggerComparer
            = StringComparer.OrdinalIgnoreCase;

        public const StringComparison LoggerComparison
            = StringComparison.OrdinalIgnoreCase;

        new public string Logger { get; }

        public FilterLogger (string logger)
        {
            Logger = logger;
        }

        public override bool Equals (object obj)
            => obj is FilterLogger other && Equals (other);

        public bool Equals (FilterLogger other)
            => other != null && LoggerComparer.Equals (Logger, other.Logger);

        public override int GetHashCode ()
            => -2065107890 + LoggerComparer.GetHashCode (Logger);

        internal override HandleGraph<FilterHandle> Build ()
        {
            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitLoggerNt (out result, Logger);
                return HandleGraph.Simple (result);
            }
            catch (Exception ex)
            {
                Disposable.DisposeAggregateErrors (result, ex);
                throw;
            }
        }

        public override void AcceptVisitor (IFilterVisitor visitor)
            => (visitor ?? throw new ArgumentNullException (nameof (visitor))).Visit (this);
    }
}
