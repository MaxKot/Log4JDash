using System;

namespace Log4JParserNet
{
    public sealed class FilterLevel : FilterBase
    {
        public FilterLevel (string min, string max)
            : base (Init (min, max))
        {

        }

        private static FilterHandle Init (string min, string max)
        {
            FilterHandle result;
            Log4JParserC.Log4JFilterInitLevelC (out result, min, max);

            return result;
        }
    }
}
