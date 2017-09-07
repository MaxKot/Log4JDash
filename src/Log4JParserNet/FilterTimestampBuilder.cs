using System;

namespace Log4JParserNet
{
    public sealed class FilterTimestampBuilder
        : FilterBuilder
        , IEquatable<FilterTimestampBuilder>
    {
        private readonly Int64 min_;

        private readonly Int64 max_;

        public FilterTimestampBuilder (Int64 min, Int64 max)
        {
            min_ = min;
            max_ = max;
        }

        public FilterTimestampBuilder (DateTime min, DateTime max)
            : this (Log4JParserNet.Timestamp.FromDateTime (min), Log4JParserNet.Timestamp.FromDateTime (max))
        {

        }

        public override bool Equals (object obj)
            => obj is FilterTimestampBuilder other && Equals (other);

        public bool Equals (FilterTimestampBuilder other)
            => other != null && min_ == other.min_ && max_ == other.max_;

        public override int GetHashCode ()
        {
            var hashCode = -320226678;
            hashCode = hashCode * -1521134295 + min_.GetHashCode ();
            hashCode = hashCode * -1521134295 + max_.GetHashCode ();
            return hashCode;
        }

        public override Filter Build ()
        {
            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitTimestamp (out result, min_, max_);
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
