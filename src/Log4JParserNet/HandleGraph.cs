using System;
using System.Runtime.InteropServices;

namespace Log4JParserNet
{
    internal abstract class HandleGraph<THandle>
        : IDisposable
        where THandle : SafeHandle
    {
        private sealed class StandaloneHandle : HandleGraph<THandle>
        {
            public StandaloneHandle (THandle impl)
                : base (impl)
            {

            }

            protected override void DisposeChildren ()
            {

            }
        }

        private sealed class ChildHandles : HandleGraph<THandle>
        {
            private readonly AssociatedHandlesCollection<HandleGraph<THandle>> children_;

            public ChildHandles (THandle impl, AssociatedHandlesCollection<HandleGraph<THandle>> children)
                : base (impl)
            {
                if (children == null)
                {
                    throw new ArgumentNullException (nameof (children));
                }

                children_ = children;
            }

            protected override void DisposeChildren () => children_.Dispose ();
        }

        private sealed class SingleChildHandle : HandleGraph<THandle>
        {
            private readonly HandleGraph<THandle> child_;

            public SingleChildHandle (THandle impl, HandleGraph<THandle> child)
                : base (impl)
            {
                if (child == null)
                {
                    throw new ArgumentNullException (nameof (child));
                }

                child_ = child;
            }

            protected override void DisposeChildren () => child_.Dispose ();
        }

        private readonly THandle impl_;

        internal THandle Handle
        {
            get
            {
                GuardState ();
                return impl_;
            }
        }

        private HandleGraph (THandle impl)
        {
            if (impl == null)
            {
                throw new ArgumentNullException (nameof (impl));
            }

            impl_ = impl;
        }

        internal static HandleGraph<THandle> Simple (THandle impl)
            => new StandaloneHandle (impl);

        internal static HandleGraph<THandle> Composite (THandle impl, AssociatedHandlesCollection<HandleGraph<THandle>> children)
            => new ChildHandles (impl, children);

        internal static HandleGraph<THandle> Composite (THandle impl, HandleGraph<THandle> child)
            => new SingleChildHandle (impl, child);

        #region IDisposable Support

        private bool disposedValue_ = false;

        public void Dispose ()
        {
            if (!disposedValue_)
            {
                // TODO: Ensure no inconsistent state if impl_ is disposed and DisposeChildren fails.
                impl_.Dispose ();
                DisposeChildren ();
                disposedValue_ = true;
            }
        }

        protected abstract void DisposeChildren ();

        private void GuardState ()
        {
            if (disposedValue_)
            {
                throw new ObjectDisposedException ("FilterBase");
            }
        }

        #endregion
    }

    internal static class HandleGraph
    {
        internal static HandleGraph<THandle> Simple<THandle> (THandle impl)
            where THandle : SafeHandle
            => HandleGraph<THandle>.Simple (impl);

        internal static HandleGraph<THandle> Composite<THandle> (THandle impl, AssociatedHandlesCollection<HandleGraph<THandle>> children)
            where THandle : SafeHandle
            => HandleGraph<THandle>.Composite (impl, children);

        internal static HandleGraph<THandle> Composite<THandle> (THandle impl, HandleGraph<THandle> child)
            where THandle : SafeHandle
            => HandleGraph<THandle>.Composite (impl, child);
    }
}
