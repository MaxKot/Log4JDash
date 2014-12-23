using System;

namespace Log4JParserNet
{
    public sealed class FilterLevel : FilterBase
    {
        public FilterLevel (Int32 min, Int32 max)
            : base (Init (min, max))
        {

        }

        public FilterLevel (string min, string max)
            : base (Init (min, max))
        {

        }

        private static FilterHandle Init (Int32 min, Int32 max)
        {
            FilterHandle result;
            Log4JParserC.Log4JFilterInitLevelI (out result, min, max);

            return result;
        }

        private static FilterHandle Init (string min, string max)
        {
            FilterHandle result;
            Log4JParserC.Log4JFilterInitLevelC (out result, min, max);

            return result;
        }
    }
}
