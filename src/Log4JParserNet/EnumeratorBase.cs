using System;
using System.Collections;
using System.Collections.Generic;

namespace Log4JParserNet
{
    public abstract class EnumeratorBase : IEnumerator<Event>
    {
        private readonly IteratorHandle impl_;

        internal IteratorHandle Handle
        {
            get
            {
                if (disposedValue_)
                {
                    throw new ObjectDisposedException ("IteratorBase");
                }
                return impl_;
            }
        }

        internal EnumeratorBase (IteratorHandle impl)
        {
            impl_ = impl;
        }

        public bool MoveNext ()
        {
            if (disposedValue_)
            {
                throw new ObjectDisposedException ("IteratorBase");
            }
            var result = Log4JParserC.Log4JIteratorMoveNext (impl_);
            return result;
        }

        public void Reset ()
        {
            if (disposedValue_)
            {
                throw new ObjectDisposedException ("IteratorBase");
            }
            throw new NotSupportedException ();
        }

        public Event Current
        {
            get
            {
                if (disposedValue_)
                {
                    throw new ObjectDisposedException ("IteratorBase");
                }
                var handle = Log4JParserC.Log4JIteratorCurrent (impl_);
                return new Event (handle, impl_);
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        #region IDisposable Support

        private bool disposedValue_ = false; // To detect redundant calls

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
