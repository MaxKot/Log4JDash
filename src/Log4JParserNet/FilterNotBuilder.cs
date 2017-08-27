using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterNotBuilder : FilterBuilder
    {
        private readonly FilterBuilder child_;

        public FilterNotBuilder (FilterBuilder child)
        {
            child_ = child;
        }

        public override Filter Build ()
        {
            Filter child = null;
            FilterHandle primaryFilter = null;

            // There can be a maximum of 3 exceptions if filter initialization fails:
            // * Initial exception that caused the initialization to fail.
            // * Exception raised when calling primaryFilter.Dispose ().
            // * Exception raised when calling child.Dispose ().
            var cleanupErrors = new List<Exception> (3);

            try
            {
                child = child_.Build ();

                Log4JParserC.Log4JFilterInitNot (out primaryFilter, child.Handle);

                return Filter.Composite (primaryFilter, child);
            }
            catch (Exception initEx)
            {
                cleanupErrors.Add (initEx);
                Disposable.TryDispose (primaryFilter, cleanupErrors);
                Disposable.TryDispose (child, cleanupErrors);

                if (cleanupErrors.Count > 1)
                {
                    throw new AggregateException (cleanupErrors);
                }

                throw;
            }
        }
    }
}
