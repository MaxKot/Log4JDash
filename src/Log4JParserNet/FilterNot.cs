using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterNot
        : Filter
        , IEquatable<FilterNot>
    {
        public Filter Child { get; }

        public FilterNot (Filter child)
        {
            if (child == null)
            {
                throw new ArgumentNullException (nameof (child));
            }

            Child = child;
        }

        public override bool Equals (object obj)
            => obj is FilterNot other && Equals (other);

        public bool Equals (FilterNot other)
            => other != null && Equals (Child, other.Child);

        public override int GetHashCode ()
            => -1938063594 + Child.GetHashCode ();

        internal override HandleGraph<FilterHandle> Build ()
        {
            HandleGraph<FilterHandle> child = null;
            FilterHandle primaryFilter = null;

            // There can be a maximum of 3 exceptions if filter initialization fails:
            // * Initial exception that caused the initialization to fail.
            // * Exception raised when calling primaryFilter.Dispose ().
            // * Exception raised when calling child.Dispose ().
            var cleanupErrors = new List<Exception> (3);

            try
            {
                child = Child.Build ();

                Log4JParserC.Log4JFilterInitNot (out primaryFilter, child.Handle);

                return HandleGraph.Composite (primaryFilter, child);
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

        public override void AcceptVisitor (IFilterVisitor visitor)
            => (visitor ?? throw new ArgumentNullException (nameof (visitor))).Visit (this);
    }
}
