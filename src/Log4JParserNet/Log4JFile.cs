using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    public sealed class Log4JFile : IDisposable
    {
        private abstract class EventsCollectionBase
            : IEventSource
            , IEnumerableOfEvents
        {
            private readonly Log4JFile file_;

            protected Log4JFile File => file_;

            public SafeHandle Owner => file_.impl_;

            public EventsCollectionBase (Log4JFile file)
            {
                System.Diagnostics.Debug.Assert (file != null, "Log4JFile.EventsCollectionBase.ctor: file is null.");

                file_ = file;
            }

            protected abstract EnumeratorBase DoGetEnumerator ();

            EnumeratorBase IEventSource.GetEnumerator ()
            {
                File.GuardNotDisposed ();
                return DoGetEnumerator ();
            }

            IEnumerator<Event> IEnumerable<Event>.GetEnumerator ()
            {
                File.GuardNotDisposed ();
                return DoGetEnumerator ();
            }

            IEnumerator IEnumerable.GetEnumerator ()
            {
                File.GuardNotDisposed ();
                return DoGetEnumerator ();
            }

            public IEnumerableOfEvents Where (FilterBase filter)
            {
                File.GuardNotDisposed ();
                return new FilteredEventSource (this, filter);
            }
        }

        private sealed class EventsCollection : EventsCollectionBase
        {
            private sealed class Enumerator : EnumeratorBase
            {
                public Enumerator (EventSourceHandle source)
                    : base (Init (source), source)
                {

                }

                private static IteratorHandle Init (EventSourceHandle source)
                {
                    IteratorHandle result;
                    Log4JParserC.Log4JIteratorInitEventSource (out result, source);

                    return result;
                }
            }

            public EventsCollection (Log4JFile file) : base (file)
            {

            }

            protected override EnumeratorBase DoGetEnumerator () => new Enumerator (File.impl_);
        }

        private sealed class EventsCollectionReverse : EventsCollectionBase
        {
            private sealed class Enumerator : EnumeratorBase
            {
                public Enumerator (EventSourceHandle source)
                    : base (Init (source), source)
                {

                }

                private static IteratorHandle Init (EventSourceHandle source)
                {
                    IteratorHandle result;
                    Log4JParserC.Log4JIteratorInitEventSourceReverse (out result, source);

                    return result;
                }
            }

            public EventsCollectionReverse (Log4JFile file) : base (file)
            {

            }

            protected override EnumeratorBase DoGetEnumerator () => new Enumerator (File.impl_);
        }

        private readonly UnmanagedMemoryHandle buffer_;

        private readonly EventSourceHandle impl_;

        public Log4JFile (string fileName)
        {
            using (var file = File.Open (fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var fileSize = file.Seek (0, SeekOrigin.End);
                var bufferSize = fileSize + 1L;
                file.Seek (0, SeekOrigin.Begin);

                buffer_ = new UnmanagedMemoryHandle (bufferSize);

                unsafe
                {
                    using (var memory = new UnmanagedMemoryStream ((byte*) buffer_.DangerousGetHandle (), bufferSize, bufferSize, FileAccess.Write))
                    {
                        file.CopyTo (memory);
                        memory.WriteByte (0);
                    }
                }
            }

            Log4JParserC.Log4JEventSourceInitXmlString (out impl_, buffer_.DangerousGetHandle ());
        }

        public IEnumerableOfEvents GetEvents ()
        {
            GuardNotDisposed ();
            return new EventsCollection (this);
        }

        public IEnumerableOfEvents GetEventsReverse ()
        {
            GuardNotDisposed ();
            return new EventsCollectionReverse (this);
        }

        private void GuardNotDisposed ()
        {
            if (disposedValue_)
            {
                throw new ObjectDisposedException ("Log4JFile");
            }
        }

        #region IDisposable Support

        private bool disposedValue_ = false;

        void Dispose (bool disposing)
        {
            if (!disposedValue_)
            {
                if (disposing)
                {
                    impl_.Dispose ();
                    if (buffer_ != null)
                    {
                        buffer_.Dispose ();
                    }
                }

                disposedValue_ = true;
            }
        }

        public void Dispose ()
        {
            Dispose (true);
        }

        #endregion
    }
}
