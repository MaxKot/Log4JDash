using System;

namespace Log4JParserNet
{
    public sealed class FilterLevelBuilder : FilterBuilder
    {
        private readonly string min_;

        private readonly string max_;

        public FilterLevelBuilder (string min, string max)
        {
            min_ = min;
            max_ = max;
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
