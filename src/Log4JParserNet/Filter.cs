using System;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public abstract class Filter
    {
        internal Filter ()
        {

        }

        internal abstract HandleGraph<FilterHandle> Build ();

        public abstract void AcceptVisitor (IFilterVisitor visitor);

        public static FilterAll All (params Filter[] filters)
            => All ((IEnumerable<Filter>) filters);

        public static FilterAll All (IEnumerable<Filter> filters)
        {
            var result = new FilterAll ();
            result.AddRange (filters);

            return result;
        }

        public static FilterAny Any (params Filter[] filters)
            => Any ((IEnumerable<Filter>) filters);

        public static FilterAny Any (IEnumerable<Filter> filters)
        {
            var result = new FilterAny ();
            result.AddRange (filters);

            return result;
        }

        public static FilterLevel Level (string min, string max)
            => new FilterLevel (min, max);

        public static FilterLogger Logger (string logger)
            => new FilterLogger (logger);

        public static FilterMessage Message (string message)
            => new FilterMessage (message);

        public static FilterNot Not (Filter child)
            => new FilterNot (child);

        public static FilterTimestamp Timestamp (Int64 min, Int64 max)
            => new FilterTimestamp (min, max);

        public static FilterTimestamp Timestamp (DateTime min, DateTime max)
            => new FilterTimestamp (min, max);

        internal static AssociatedHandlesCollection<HandleGraph<FilterHandle>> Build (IReadOnlyList<Filter> filters)
        {
            var result = new AssociatedHandlesCollection<HandleGraph<FilterHandle>> (filters.Count);

            try
            {
                foreach (var filter in filters)
                {
                    var filterHandle = filter.Build ();
                    result.Add (filterHandle);
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
