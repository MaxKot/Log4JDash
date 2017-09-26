using System;

namespace Log4JParserNet
{
    public sealed class FilterTimestamp
        : Filter
        , IEquatable<FilterTimestamp>
    {
        public Int64 Min { get; }

        public Int64 Max { get; }

        public DateTime MinDateTime => Log4JParserNet.Timestamp.ToDateTime (Min);

        public DateTime MaxDateTime => Log4JParserNet.Timestamp.ToDateTime (Max);

        public FilterTimestamp (Int64 min, Int64 max)
        {
            Min = min;
            Max = max;
        }

        public FilterTimestamp (DateTime min, DateTime max)
            : this (Log4JParserNet.Timestamp.FromDateTime (min), Log4JParserNet.Timestamp.FromDateTime (max))
        {

        }

        public override bool Equals (object obj)
            => obj is FilterTimestamp other && Equals (other);

        public bool Equals (FilterTimestamp other)
            => other != null && Min == other.Min && Max == other.Max;

        public override int GetHashCode ()
        {
            var hashCode = -320226678;
            hashCode = hashCode * -1521134295 + Min.GetHashCode ();
            hashCode = hashCode * -1521134295 + Max.GetHashCode ();
            return hashCode;
        }

        internal override HandleGraph<FilterHandle> Build ()
        {
            FilterHandle result = null;
            try
            {
                Log4JParserC.Log4JFilterInitTimestamp (out result, Min, Max);
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
