using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterNotBuilder
        : FilterBuilder
        , IEquatable<FilterNotBuilder>
    {
        private readonly FilterBuilder child_;

        public FilterNotBuilder (FilterBuilder child)
        {
            if (child == null)
            {
                throw new ArgumentNullException (nameof (child));
            }

            child_ = child;
        }

        public override bool Equals (object obj)
            => obj is FilterNotBuilder other && Equals (other);

        public bool Equals (FilterNotBuilder other)
            => other != null && Equals (child_, other.child_);

        public override int GetHashCode ()
            => -1938063594 + child_.GetHashCode ();

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
