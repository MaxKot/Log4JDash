
namespace Log4JParserNet
{
    public sealed class FilterAny : FilterBase
    {
        public FilterAny ()
            : base (Init ())
        {

        }

        public void Add (FilterBase childFilter)
        {
            Log4JParserC.Log4JFilterAnyAdd (Handle, childFilter.Handle);
        }

        public void Remove (FilterBase childFilter)
        {
            Log4JParserC.Log4JFilterAnyRemove (Handle, childFilter.Handle);
        }

        private static FilterHandle Init ()
        {
            FilterHandle result;
            Log4JParserC.Log4JFilterInitAny (out result);

            return result;
        }
    }
}
