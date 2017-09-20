using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterNotBuilder
        : FilterBuilder
        , IEquatable<FilterNotBuilder>
    {
        public FilterBuilder Child { get; }

        public FilterNotBuilder (FilterBuilder child)
        {
            if (child == null)
            {
                throw new ArgumentNullException (nameof (child));
            }

            Child = child;
        }

        public override bool Equals (object obj)
            => obj is FilterNotBuilder other && Equals (other);

        public bool Equals (FilterNotBuilder other)
            => other != null && Equals (Child, other.Child);

        public override int GetHashCode ()
            => -1938063594 + Child.GetHashCode ();

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
                child = Child.Build ();

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

        public override void AcceptVisitor (IFilterBuilderVisitor visitor)
            => (visitor ?? throw new ArgumentNullException (nameof (visitor))).Visit (this);
    }
}
