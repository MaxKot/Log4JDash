using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterLevelBuilder
        : FilterBuilder
        , IEquatable<FilterLevelBuilder>
    {
        public static readonly IEqualityComparer<string> LevelComparer
            = StringComparer.Ordinal;

        public const StringComparison LevelComparison
            = StringComparison.Ordinal;

        public string Min { get; }

        public string Max { get; }

        public FilterLevelBuilder (string min, string max)
        {
            Min = min;
            Max = max;
        }

        public override bool Equals (object obj)
            => obj is FilterLevelBuilder other && Equals (other);

        public bool Equals (FilterLevelBuilder other)
            => other != null &&
               LevelComparer.Equals (Min, other.Min) &&
               LevelComparer.Equals (Max, other.Max);

        public override int GetHashCode ()
        {
            var hashCode = -320226678;
            hashCode = hashCode * -1521134295 + LevelComparer.GetHashCode (Min);
            hashCode = hashCode * -1521134295 + LevelComparer.GetHashCode (Max);
            return hashCode;
        }

        public override Filter Build ()
        {
            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitLevelC (out result, Min, Max);
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
