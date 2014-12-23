
namespace Log4JParserNet
{
    public sealed class EnumeratorFilter : EnumeratorBase
    {
        public EnumeratorFilter (EnumeratorBase inner, FilterBase filter)
            : base (Init (inner.Handle, filter.Handle))
        {

        }

        private static IteratorHandle Init (IteratorHandle inner, FilterHandle filter)
        {
            IteratorHandle result;
            Log4JParserC.Log4JIteratorInitFilter (out result, inner, filter);

            return result;
        }
    }
}
