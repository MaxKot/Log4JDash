
namespace Log4JParserNet
{
    public sealed class FilterAll : FilterBase
    {
        public FilterAll ()
            : base (Init ())
        {

        }

        public void Add (FilterBase childFilter)
        {
            Log4JParserC.Log4JFilterAllAdd (Handle, childFilter.Handle);
        }

        public void Remove (FilterBase childFilter)
        {
            Log4JParserC.Log4JFilterAllRemove (Handle, childFilter.Handle);
        }

        private static FilterHandle Init ()
        {
            FilterHandle result;
            Log4JParserC.Log4JFilterInitAll (out result);

            return result;
        }
    }
}
