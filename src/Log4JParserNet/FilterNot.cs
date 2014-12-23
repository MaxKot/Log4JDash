
namespace Log4JParserNet
{
    public sealed class FilterNot : FilterBase
    {
        public FilterNot (FilterBase childFilter)
            : base (Init (childFilter.Handle))
        {

        }

        private static FilterHandle Init (FilterHandle childFilter)
        {
            FilterHandle result;
            Log4JParserC.Log4JFilterInitNot (out result, childFilter);

            return result;
        }
    }
}
