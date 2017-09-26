using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Log4JParserNet
{
    internal sealed class FilteredEventSource
        : IEnumerableOfEvents
        , IEventSource
    {
        private sealed class Enumerator : EnumeratorBase
        {
            private readonly EnumeratorBase inner_;

            private readonly HandleGraph<FilterHandle> filterHandle_;

            public Enumerator (EnumeratorBase inner, HandleGraph<FilterHandle> filterHandle)
                : base (Init (inner.Handle, filterHandle), inner.Owner)
            {
                inner_ = inner;
                filterHandle_ = filterHandle;
            }

            private static IteratorHandle Init (IteratorHandle inner, HandleGraph<FilterHandle> filterHandle)
            {
                IteratorHandle result;
                Log4JParserC.Log4JIteratorInitFilter (out result, inner, filterHandle.Handle);

                return result;
            }

            protected override void Dispose (bool disposing)
            {
                if (disposing)
                {
                    try
                    {
                        filterHandle_.Dispose ();
                    }
                    finally
                    {
                        inner_.Dispose ();
                    }
                }
                base.Dispose (disposing);
            }
        }

        private readonly IEventSource source_;

        private readonly FilterBuilder filter_;

        bool IEventSource.IsInvalid => source_.IsInvalid;

        Encoding IEventSource.Encoding => source_.Encoding;

        public FilteredEventSource (IEventSource source, FilterBuilder filter)
        {
            Debug.Assert (source != null, "FilteredEventSource.ctor: source is null.");
            Debug.Assert (filter != null, "FilteredEventSource.ctor: filter is null.");

            source_ = source;
            filter_ = filter;
        }

        internal EnumeratorBase GetEnumerator ()
        {
            HandleGraph<FilterHandle> filterHandle = null;
            try
            {
                filterHandle = filter_.Build ();
                return new Enumerator (source_.GetEnumerator (), filterHandle);
            }
            catch
            {
                filterHandle?.Dispose ();
                throw;
            }
        }

        EnumeratorBase IEventSource.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        IEnumerator<Event> IEnumerable<Event>.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        public IEnumerableOfEvents Where (FilterBuilder filter)
        {
            return new FilteredEventSource (this, filter);
        }
    }
}
