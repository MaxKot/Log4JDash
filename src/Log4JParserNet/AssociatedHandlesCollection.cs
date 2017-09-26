using System;
using System.Collections;
using System.Collections.Generic;

namespace Log4JParserNet
{
    internal sealed class AssociatedHandlesCollection<THandle>
        : IDisposable
        , IReadOnlyCollection<THandle>
        where THandle : class, IDisposable
    {
        private List<THandle> handles_;

        public int Count => handles_.Count;

        public AssociatedHandlesCollection ()
        {
            handles_ = new List<THandle> ();
        }

        public AssociatedHandlesCollection (int capacity)
        {
            handles_ = new List<THandle> (capacity);
        }

        public void Add (THandle handle)
        {
            if (handle == null)
            {
                throw new ArgumentNullException (nameof (handle));
            }

            handles_.Add (handle);
        }

        public void Dispose ()
        {
            var cleanupErrors = new List<Exception> (handles_.Count);

            foreach (var handle in handles_)
            {
                try
                {
                    handle.Dispose ();
                }
                catch (Exception cleanupEx)
                {
                    cleanupErrors.Add (cleanupEx);
                }
            }

            if (cleanupErrors.Count > 0)
            {
                throw new AggregateException (cleanupErrors);
            }
        }

        public IEnumerator<THandle> GetEnumerator ()
            => handles_.GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator ()
            => GetEnumerator ();
    }
}
