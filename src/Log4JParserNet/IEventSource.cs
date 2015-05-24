using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal interface IEventSource
    {
        EnumeratorBase GetEnumerator ();

        SafeHandle Owner { get; }
    }
}
