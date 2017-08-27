using System;
using System.Collections;
using System.Collections.Generic;

namespace Log4JParserNet
{
    internal sealed class AssociatedFiltersCollection
        : IDisposable
        , IReadOnlyCollection<Filter>
    {
        private List<Filter> filters_;

        public int Count => filters_.Count;

        public AssociatedFiltersCollection ()
        {
            filters_ = new List<Filter> ();
        }

        public AssociatedFiltersCollection (int capacity)
        {
            filters_ = new List<Filter> (capacity);
        }

        public void Add (Filter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException (nameof (filter));
            }

            filters_.Add (filter);
        }

        public void Dispose ()
        {
            var cleanupErrors = new List<Exception> (filters_.Count);

            foreach (var filter in filters_)
            {
                try
                {
                    filter.Dispose ();
                }
                catch (Exception cleanupEx)
                {
                    cleanupErrors.Add (cleanupEx);
                }
            }

            if (cleanupErrors.Count > 0)
            {
                throw new AggregateException (cleanupErrors);
            }
        }

        public IEnumerator<Filter> GetEnumerator ()
            => filters_.GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator ()
            => GetEnumerator ();

        public static AssociatedFiltersCollection Build (IReadOnlyList<FilterBuilder> filterBuilders)
        {
            var result = new AssociatedFiltersCollection (filterBuilders.Count);

            try
            {
                foreach (var filterBuilder in filterBuilders)
                {
                    var filter = filterBuilder.Build ();
                    result.Add (filter);
                }

                return result;
            }
            catch (Exception initEx)
            {
                try
                {
                    result.Dispose ();
                }
                catch (Exception cleanupEx)
                {
                    throw new AggregateException (initEx, cleanupEx);
                }

                throw;
            }
        }
    }
}
