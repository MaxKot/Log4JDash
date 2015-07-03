using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Log4JDash.Web
{
    internal static class EnumerableExt
    {
        private struct CycleBuffer<T> : IEnumerable<T>
        {
            private readonly T[] impl_;

            private int nextWriteIndex_;

            private bool isFull_;

            public CycleBuffer (int quantity)
            {
                impl_ = new T[quantity];
                nextWriteIndex_ = 0;
                isFull_ = false;
            }

            public void Add (T element)
            {
                impl_[nextWriteIndex_] = element;
                var newNextWriteIndex = (nextWriteIndex_ + 1) % impl_.Length;

                isFull_ |= newNextWriteIndex < nextWriteIndex_;
                nextWriteIndex_ = newNextWriteIndex;
            }

            public IEnumerator<T> GetEnumerator ()
            {
                var result = isFull_
                    ? impl_.Concat (impl_).Skip (nextWriteIndex_).Take (impl_.Length)
                    : impl_.Take (nextWriteIndex_);

                return result.GetEnumerator ();
            }

            IEnumerator IEnumerable.GetEnumerator ()
            {
                return GetEnumerator ();
            }
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> coll, int quantity)
        {
            var result = new CycleBuffer<T> (quantity);

            using (var enumerator = coll.GetEnumerator ())
            {
                while (enumerator.MoveNext ())
                {
                    result.Add (enumerator.Current);
                }

                foreach (var element in result)
                {
                    yield return element;
                }
            }
        }
    }
}
