using System;
using System.Collections.Generic;
using System.Linq;

namespace Log4JParserNet
{
    public sealed class FilterAnyBuilder
        : FilterBuilder
        , IEquatable<FilterAnyBuilder>
    {
        private HashSet<FilterBuilder> children_ = new HashSet<FilterBuilder> ();

        public override bool Equals (object obj)
            => obj is FilterAnyBuilder other && Equals (other);

        public bool Equals (FilterAnyBuilder other)
            => other != null && children_.SetEquals (other.children_);

        public override int GetHashCode ()
            => children_
                .Select (e => e.GetHashCode ())
                .OrderBy (hc => hc)
                .Aggregate (-1313666667, (a, hc) => a * -1521134295 + hc);

        public void Add (FilterBuilder child)
            => children_.Add (child ?? throw new ArgumentNullException (nameof (child)));

        public void AddRange (IEnumerable<FilterBuilder> children)
        {
            if (children == null)
            {
                throw new ArgumentNullException (nameof (children));
            }

            foreach (var child in children)
            {
                Add (child);
            }
        }

        public void Remove (FilterBuilder child)
            => children_.Remove (child);

        public void Clear ()
            => children_.Clear ();

        public override Filter Build ()
        {
            AssociatedFiltersCollection children = null;
            FilterHandle primaryFilter = null;

            // There can be a maximum of 3 exceptions if filter initialization fails:
            // * Initial exception that caused the initialization to fail.
            // * Exception raised when calling primaryFilter.Dispose ().
            // * Exception raised when calling children.Dispose ().
            var cleanupErrors = new List<Exception> (3);

            try
            {
                children = AssociatedFiltersCollection.Build (children_.ToArray ());

                Log4JParserC.Log4JFilterInitAny (out primaryFilter);
                foreach (var child in children)
                {
                    Log4JParserC.Log4JFilterAnyAdd (primaryFilter, child.Handle);
                }

                return Filter.Composite (primaryFilter, children);
            }
            catch (Exception initEx)
            {
                cleanupErrors.Add (initEx);
                Disposable.TryDispose (primaryFilter, cleanupErrors);
                Disposable.TryDispose (children, cleanupErrors);

                if (cleanupErrors.Count > 1)
                {
                    throw new AggregateException (cleanupErrors);
                }

                throw;
            }
        }
    }
}
