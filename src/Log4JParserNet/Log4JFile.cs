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

            public IEnumerableOfEvents Where (Filter filter)
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

        public long Size { get; }

        private readonly EventSourceHandle impl_;

        private Encoding encoding_;

        public Encoding Encoding
        {
            get { return encoding_; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException (nameof (value));
                }
                encoding_ = value;
            }
        }

        public static Log4JFile Create (string fileName, long? maxSize = null)
        {
            using (var fileStream = File.Open (fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return Create (fileStream, maxSize);
            }
        }

        public static Log4JFile Create (Stream source, long? maxSize = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException (nameof (source));
            }
            if (!source.CanRead)
            {
                throw new ArgumentException ("Source stream must support reading.", nameof (source));
            }
            if (maxSize != null && maxSize < 0L)
            {
                const string message = "Maximum size must be a positive value.";
                throw new ArgumentOutOfRangeException (nameof (maxSize), message);
            }

            if (maxSize == null && !source.CanSeek)
            {
                using (var seekableCopy = new MemoryStream ())
                {
                    Debug.Assert (seekableCopy.CanSeek);
                    source.CopyTo (seekableCopy);
                    seekableCopy.Seek (0, SeekOrigin.Begin);
                    return Create (seekableCopy);
                }
            }

            long bufferSize;
            if (maxSize == null)
            {
                var sourceSize = source.Seek (0, SeekOrigin.End);
                bufferSize = checked (sourceSize + 1L);
                source.Seek (0, SeekOrigin.Begin);
            }
            else
            {
                bufferSize = checked ((long) maxSize + 1);
            }

            UnmanagedMemoryHandle buffer = null;
            long size;
            try
            {
                try { }
                finally
                {
                    buffer = new UnmanagedMemoryHandle (bufferSize);
                }

                unsafe
                {
                    var bufferHandle = (byte*) buffer.DangerousGetHandle ();
                    using (var memory = new UnmanagedMemoryStream (bufferHandle, bufferSize, bufferSize, FileAccess.Write))
                    {
                        var copyBuffer = new byte[Math.Max (bufferSize - 1, 81920)];
                        var remainingToCopy = bufferSize - 1L;
                        while (remainingToCopy > 0L)
                        {
                            var toRead = (int) Math.Min (copyBuffer.Length, remainingToCopy);
                            var read = source.Read (copyBuffer, 0, toRead);
                            if (read != 0)
                            {
                                remainingToCopy -= read;
                                memory.Write (copyBuffer, 0, read);
                            }
                            else
                            {
                                remainingToCopy = 0L;
                            }
                        }

                        size = memory.Position;
                        memory.WriteByte (0);
                    }
                }

                return new Log4JFile (buffer, size);
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

        private Log4JFile (UnmanagedMemoryHandle buffer, long size)
        {
            buffer_ = buffer;
            Size = size;
            var status = Log4JParserC.Log4JEventSourceInitXmlString (out impl_, buffer_.DangerousGetHandle ());

            switch (status)
            {
                case Log4JParserC.Status.Success:
                    break;

                case Log4JParserC.Status.DocumentErrors:
                    break;

                case Log4JParserC.Status.MemoryError:
                    impl_.Dispose ();
                    throw new OutOfMemoryException ();

                default:
                    impl_.Dispose ();
                    throw new InvalidOperationException ();
            }

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
