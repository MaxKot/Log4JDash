using System;

namespace Log4JParserNet
{
    public sealed class EventSource : IDisposable
    {
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

        public EventSource (string filePath)
        {
            Log4JParserC.Log4JEventSourceInitXmlFile (out impl_, filePath);
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
