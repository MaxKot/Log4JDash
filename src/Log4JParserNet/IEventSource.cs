using System.Text;

namespace Log4JParserNet
{
    internal interface IEventSource
    {
        EnumeratorBase GetEnumerator ();

        bool IsInvalid { get; }

        Encoding Encoding { get; }
    }
}
