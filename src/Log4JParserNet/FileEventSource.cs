using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    public sealed class FileEventSource
        : IDisposable
        , IEnumerable<Event>
        , IEventSource
    {
        private sealed class Enumerator : EnumeratorBase
        {
            public Enumerator (FileEventSource source)
                : base (Init (source.impl_), source.impl_)
            {

            }

            private static IteratorHandle Init (EventSourceHandle source)
            {
                IteratorHandle result;
                Log4JParserC.Log4JIteratorInitEventSource (out result, source);

                return result;
            }
        }

        private readonly UnmanagedMemoryHandle buffer_;

        private readonly EventSourceHandle impl_;

        SafeHandle IEventSource.Owner => impl_;

        public FileEventSource (string fileName)
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

        public IEnumerable<Event> Where (FilterBase filter)
        {
            if (disposedValue_)
            {
                throw new ObjectDisposedException ("FileEventSource");
            }

            return new FilteredEventSource (this, filter);
        }

        #region IEnumerable Support

        internal EnumeratorBase GetEnumerator ()
        {
            if (disposedValue_)
            {
                throw new ObjectDisposedException ("FileEventSource");
            }

            return new Enumerator (this);
        }

        EnumeratorBase IEventSource.GetEnumerator()
        {
            return GetEnumerator ();
        }

        IEnumerator<Event> IEnumerable<Event>.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        #endregion

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
