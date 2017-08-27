using System;

namespace Log4JParserNet
{
    public sealed class FilterTimestampBuilder : FilterBuilder
    {
        private readonly Int64 min_;

        private readonly Int64 max_;

        public FilterTimestampBuilder (Int64 min, Int64 max)
        {
            min_ = min;
            max_ = max;
        }

        public FilterTimestampBuilder (DateTime min, DateTime max)
            : this (Timestamp.FromDateTime (min), Timestamp.FromDateTime (max))
        {

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
