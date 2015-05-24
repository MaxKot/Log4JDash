using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal sealed class FilteredEventSource
        : IEnumerable<Event>
        , IEventSource
    {
        private sealed class Enumerator : EnumeratorBase
        {
            public Enumerator (EnumeratorBase inner, FilterBase filter)
                : base (Init (inner.Handle, filter.Handle), inner.Owner)
            {

            }

            private static IteratorHandle Init (IteratorHandle inner, FilterHandle filter)
            {
                IteratorHandle result;
                Log4JParserC.Log4JIteratorInitFilter (out result, inner, filter);

                return result;
            }
        }

        private readonly IEventSource source_;

        private readonly FilterBase filter_;

        SafeHandle IEventSource.Owner
        {
            get { return source_.Owner; }
        }

        public FilteredEventSource (IEventSource source, FilterBase filter)
        {
            Debug.Assert (source != null, "FilteredEventSource.ctor: source is null.");
            Debug.Assert (filter != null, "FilteredEventSource.ctor: filter is null.");

            source_ = source;
            filter_ = filter;
        }

        internal EnumeratorBase GetEnumerator ()
        {
            return new Enumerator (source_.GetEnumerator (), filter_);
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
    }
}
