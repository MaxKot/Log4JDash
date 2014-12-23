
namespace Log4JParserNet
{
    public sealed class EnumeratorEventSource : EnumeratorBase
    {
        public EnumeratorEventSource (EventSource source)
            : base (Init (source.Handle))
        {

        }

        private static IteratorHandle Init (EventSourceHandle source)
        {
            IteratorHandle result;
            Log4JParserC.Log4JIteratorInitEventSource (out result, source);

            return result;
        }
    }
}
