using System;

namespace Log4JParserNet
{
    public sealed class FilterTimestamp : FilterBase
    {
        public FilterTimestamp (Int64 min, Int64 max)
            : base (Init (min, max))
        {

        }

        public FilterTimestamp (DateTime min, DateTime max)
            : base (Init (min, max))
        {

        }

        private static FilterHandle Init (Int64 min, Int64 max)
        {
            FilterHandle result;
            Log4JParserC.Log4JFilterInitTimestamp (out result, min, max);

            return result;
        }

        private static FilterHandle Init (DateTime min, DateTime max)
        {
            var minTs = Timestamp.FromDateTime (min);
            var maxTs = Timestamp.FromDateTime (max);

            return Init (minTs, maxTs);
        }
    }
}
