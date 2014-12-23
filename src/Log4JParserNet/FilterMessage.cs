
namespace Log4JParserNet
{
    public sealed class FilterMessage : FilterBase
    {
        public FilterMessage (string message)
            : base (Init (message))
        {

        }

        private static FilterHandle Init (string message)
        {
            FilterHandle result;
            Log4JParserC.Log4JFilterInitMessageNt (out result, message);

            return result;
        }
    }
}
