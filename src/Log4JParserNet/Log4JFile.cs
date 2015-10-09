using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

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

            public bool IsInvalid => file_.impl_.IsInvalid;

            public Encoding Encoding => file_.Encoding;

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
                public Enumerator (EventsCollection source)
                    : base (Init (source.File.impl_), source)
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

            protected override EnumeratorBase DoGetEnumerator () => new Enumerator (this);
        }

        private sealed class EventsCollectionReverse : EventsCollectionBase
        {
            private sealed class Enumerator : EnumeratorBase
            {
                public Enumerator (EventsCollectionReverse source)
                    : base (Init (source.File.impl_), source)
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

            protected override EnumeratorBase DoGetEnumerator () => new Enumerator (this);
        }

        private readonly UnmanagedMemoryHandle buffer_;

        private readonly EventSourceHandle impl_;

        public Encoding Encoding { get; set; }

        public static Log4JFile Create (string fileName)
        {
            using (var fileStream = File.Open (fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return Create (fileStream);
            }
        }

        public static Log4JFile Create (Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException (nameof (source));
            }
            if (!source.CanRead)
            {
                throw new ArgumentException ("Source stream must support reading.", nameof (source));
            }

            if (!source.CanSeek)
            {
                using (var seekableCopy = new MemoryStream ())
                {
                    Debug.Assert (seekableCopy.CanSeek);
                    source.CopyTo (seekableCopy);
                    seekableCopy.Seek (0, SeekOrigin.Begin);
                    return Create (seekableCopy);
                }
            }

            var sourceSize = source.Seek (0, SeekOrigin.End);
            var bufferSize = sourceSize + 1L;
            source.Seek (0, SeekOrigin.Begin);

            UnmanagedMemoryHandle buffer = null;
            try
            {
                try { }
                finally
                {
                    buffer = new UnmanagedMemoryHandle (bufferSize);
                }

                unsafe
                {
                    using (var memory = new UnmanagedMemoryStream ((byte*) buffer.DangerousGetHandle (), bufferSize, bufferSize, FileAccess.Write))
                    {
                        source.CopyTo (memory);
                        memory.WriteByte (0);
                    }
                }

                return new Log4JFile (buffer);
            }
            catch
            {
                if (buffer != null)
                {
                    buffer.Dispose ();
                }
                throw;
            }
        }

        private Log4JFile (UnmanagedMemoryHandle buffer)
        {
            buffer_ = buffer;
            Log4JParserC.Log4JEventSourceInitXmlString (out impl_, buffer_.DangerousGetHandle ());

            Encoding = Encoding.ASCII;
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
