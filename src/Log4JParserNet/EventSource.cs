using System;
using System.IO;

namespace Log4JParserNet
{
    public sealed class EventSource : IDisposable
    {
        private readonly UnmanagedMemoryHandle buffer_;

        private readonly EventSourceHandle impl_;

        internal EventSourceHandle Handle
        {
            get
            {
                if (disposedValue_)
                {
                    throw new ObjectDisposedException ("EventSource");
                }
                return impl_;
            }
        }

        public EventSource (string fileName)
        {
            using (var file = File.Open (fileName, FileMode.Open))
            {
                var size = file.Seek (0, SeekOrigin.End) + 1L;
                file.Seek (0, SeekOrigin.Begin);

                buffer_ = new UnmanagedMemoryHandle (size);

                unsafe
                {
                    using (var memory = new UnmanagedMemoryStream ((byte*) buffer_.DangerousGetHandle (), size, size, FileAccess.Write))
                    {
                        file.CopyTo (memory);
                    }
                }
            }

            Log4JParserC.Log4JEventSourceInitXmlString (out impl_, buffer_.DangerousGetHandle ());
        }

        public Event First ()
        {
            if (disposedValue_)
            {
                throw new ObjectDisposedException ("EventSource");
            }
            var eventHandle = Log4JParserC.Log4JEventSourceFirst (impl_);
            return !eventHandle.IsInvalid ? new Event (eventHandle) : null;
        }

        public Event Next (Event @event)
        {
            if (disposedValue_)
            {
                throw new ObjectDisposedException ("EventSource");
            }
            var eventHandle = Log4JParserC.Log4JEventSourceNext (impl_, @event.Handle);
            return !eventHandle.IsInvalid ? new Event (eventHandle) : null;
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
