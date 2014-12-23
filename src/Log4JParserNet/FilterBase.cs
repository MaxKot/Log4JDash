using System;

namespace Log4JParserNet
{
    public abstract class FilterBase : IDisposable
    {
        private readonly FilterHandle impl_;

        internal FilterHandle Handle
        {
            get
            {
                if (disposedValue_)
                {
                    throw new ObjectDisposedException ("FilterBase");
                }
                return impl_;
            }
        }

        internal FilterBase (FilterHandle impl)
        {
            impl_ = impl;
        }

        public bool Apply (Event @event)
        {
            if (disposedValue_)
            {
                throw new ObjectDisposedException ("FilterBase");
            }
            return Log4JParserC.Log4JFilterApply (impl_, @event.Handle);
        }

        #region IDisposable Support

        private bool disposedValue_ = false;

        protected virtual void Dispose (bool disposing)
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
