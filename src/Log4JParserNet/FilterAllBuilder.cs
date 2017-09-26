using System;
using System.Collections.Generic;
using System.Linq;

namespace Log4JParserNet
{
    public sealed class FilterAllBuilder
        : FilterBuilder
        , IEquatable<FilterAllBuilder>
    {
        private HashSet<FilterBuilder> children_ = new HashSet<FilterBuilder> ();

        public IReadOnlyCollection<FilterBuilder> Children
            => new ReadOnlyCollectionAdapter<FilterBuilder> (children_);

        public override bool Equals (object obj)
            => obj is FilterAllBuilder other && Equals (other);

        public bool Equals (FilterAllBuilder other)
            => other != null && children_.SetEquals (other.children_);

        public override int GetHashCode ()
            => children_
                .Select (e => e.GetHashCode ())
                .OrderBy (hc => hc)
                .Aggregate (-422695531, (a, hc) => a * -1521134295 + hc);

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

        internal override HandleGraph<FilterHandle> Build ()
        {
            AssociatedHandlesCollection<HandleGraph<FilterHandle>> children = null;
            FilterHandle primaryFilter = null;

            // There can be a maximum of 3 exceptions if filter initialization fails:
            // * Initial exception that caused the initialization to fail.
            // * Exception raised when calling primaryFilter.Dispose ().
            // * Exception raised when calling children.Dispose ().
            var cleanupErrors = new List<Exception> (3);

            try
            {
                children = Build (children_.ToArray ());

                Log4JParserC.Log4JFilterInitAll (out primaryFilter);
                foreach (var child in children)
                {
                    Log4JParserC.Log4JFilterAllAdd (primaryFilter, child.Handle);
                }

                return HandleGraph.Composite (primaryFilter, children);
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

        public override void AcceptVisitor (IFilterBuilderVisitor visitor)
            => (visitor ?? throw new ArgumentNullException (nameof (visitor))).Visit (this);
    }
}
