using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public sealed class FilterAnyBuilder : FilterBuilder
    {
        private List<FilterBuilder> children_ = new List<FilterBuilder> ();

        public void Add (FilterBuilder childFilter)
            => children_.Add (childFilter);

        public void Remove (FilterBuilder childFilter)
            => children_.Remove (childFilter);

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
                children = AssociatedFiltersCollection.Build (children_);

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
