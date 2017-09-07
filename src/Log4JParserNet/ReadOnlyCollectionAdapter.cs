using System;
using System.Collections;
using System.Collections.Generic;

namespace Log4JParserNet
{
    internal sealed class ReadOnlyCollectionAdapter<T> : IReadOnlyCollection<T>
    {
        private readonly ICollection<T> impl_;

        public int Count => impl_.Count;

        public ReadOnlyCollectionAdapter (ICollection<T> impl)
        {
            if (impl == null)
            {
                throw new ArgumentNullException (nameof (impl));
            }

            impl_ = impl;
        }

        public IEnumerator<T> GetEnumerator ()
            => impl_.GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator ()
            => impl_.GetEnumerator ();
    }
}
