using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterLevelBuilder
        : FilterBuilder
        , IEquatable<FilterLevelBuilder>
    {
        private static readonly IEqualityComparer<string> LevelComparer
            = StringComparer.Ordinal;

        private readonly string min_;

        private readonly string max_;

        public FilterLevelBuilder (string min, string max)
        {
            min_ = min;
            max_ = max;
        }

        public override bool Equals (object obj)
            => obj is FilterLevelBuilder other && Equals (other);

        public bool Equals (FilterLevelBuilder other)
            => other != null &&
               LevelComparer.Equals (min_, other.min_) &&
               LevelComparer.Equals (max_, other.max_);

        public override int GetHashCode ()
        {
            var hashCode = -320226678;
            hashCode = hashCode * -1521134295 + LevelComparer.GetHashCode (min_);
            hashCode = hashCode * -1521134295 + LevelComparer.GetHashCode (max_);
            return hashCode;
        }

        public override Filter Build ()
        {
            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitLevelC (out result, min_, max_);
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
