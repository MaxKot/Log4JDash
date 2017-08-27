using System;

namespace Log4JParserNet
{
    public abstract class Filter : IDisposable
    {
        private sealed class SimpleFilter : Filter
        {
            public SimpleFilter (FilterHandle impl)
                : base (impl)
            {

            }

            protected override void DisposeChildren ()
            {

            }
        }

        private sealed class CompositeFilter : Filter
        {
            private readonly AssociatedFiltersCollection children_;

            public CompositeFilter (FilterHandle impl, AssociatedFiltersCollection children)
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

        private sealed class CompositeOfTwoFilter : Filter
        {
            private readonly Filter child_;

            public CompositeOfTwoFilter (FilterHandle impl, Filter child)
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

        private readonly FilterHandle impl_;

        internal FilterHandle Handle
        {
            get
            {
                GuardState ();
                return impl_;
            }
        }

        private Filter (FilterHandle impl)
        {
            if (impl == null)
            {
                throw new ArgumentNullException (nameof (impl));
            }

            impl_ = impl;
        }

        public bool Apply (Event @event)
            => Log4JParserC.Log4JFilterApply (Handle, @event.Handle);

        internal static Filter Simple (FilterHandle impl)
            => new SimpleFilter (impl);

        internal static Filter Composite (FilterHandle impl, AssociatedFiltersCollection children)
            => new CompositeFilter (impl, children);

        internal static Filter Composite (FilterHandle impl, Filter child)
            => new CompositeOfTwoFilter (impl, child);

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
}
