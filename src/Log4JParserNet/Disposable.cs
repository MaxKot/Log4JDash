using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    internal static class Disposable
    {
        public static void TryDispose (IDisposable disposable, List<Exception> cleanupErrors)
        {
            try
            {
                disposable?.Dispose ();
            }
            catch (Exception cleanupEx)
            {
                cleanupErrors.Add (cleanupEx);
            }
        }

        public static void DisposeAggregateErrors (IDisposable disposable, Exception primaryException)
        {
            try
            {
                disposable?.Dispose ();
            }
            catch (Exception cleanupEx)
            {
                throw new AggregateException (primaryException, cleanupEx);
            }
        }
    }
}
