using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal abstract class EnumeratorBase : IEnumerator<Event>
    {
        private readonly IteratorHandle impl_;

        private readonly SafeHandle owner_;

        internal SafeHandle Owner
        {
            get { return owner_; }
        }

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

        internal EnumeratorBase (IteratorHandle impl, SafeHandle owner)
        {
            Debug.Assert (impl != null, "EnumeratorBase.ctor: impl is null.");
            Debug.Assert (!impl.IsInvalid, "EnumeratorBase.ctor: impl is invalid.");
            Debug.Assert (!impl.IsClosed, "EnumeratorBase.ctor: impl is closed.");

            Debug.Assert (owner != null, "EnumeratorBase.ctor: owner is null.");
            Debug.Assert (!owner.IsInvalid, "EnumeratorBase.ctor: owner is invalid.");
            Debug.Assert (!owner.IsClosed, "EnumeratorBase.ctor: owner is closed.");

            impl_ = impl;
            owner_ = owner;
        }

        public bool MoveNext ()
        {
            if (disposedValue_ || impl_.IsInvalid || impl_.IsClosed)
            {
                throw new ObjectDisposedException ("IteratorBase");
            }
            var result = Log4JParserC.Log4JIteratorMoveNext (impl_);
            return result;
        }

        public void Reset ()
        {
            if (disposedValue_ || impl_.IsInvalid || impl_.IsClosed)
            {
                throw new ObjectDisposedException ("IteratorBase");
            }
            throw new NotSupportedException ();
        }

        public Event Current
        {
            get
            {
                if (disposedValue_ || impl_.IsInvalid || impl_.IsClosed)
                {
                    throw new ObjectDisposedException ("IteratorBase");
                }
                UIntPtr id;
                var handle = Log4JParserC.Log4JIteratorCurrent (impl_, out id);
                return new Event (handle, id.ToUInt64 (), owner_);
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
