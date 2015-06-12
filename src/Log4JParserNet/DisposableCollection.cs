using System;
using System.Collections.Generic;
using System.Linq;

namespace Log4JParserNet
{
    public sealed class DisposableCollection<TCol>
        : IDisposable
        where TCol : IEnumerable<IDisposable>
    {
        private readonly TCol elements_;

        public TCol Elements => elements_;

        public DisposableCollection (TCol elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException (nameof (elements));
            }

            elements_ = elements;
        }

        public void Dispose ()
        {
            var errors = new List<Exception> ();

            foreach (var element in elements_)
            {
                try
                {
                    element?.Dispose ();
                }
                catch (Exception ex)
                {
                    errors.Add (ex);
                }
            }

            if (errors.Any ())
            {
                throw new AggregateException (errors);
            }
        }
    }

    public static class DisposableCollection
    {
        public static DisposableCollection<TCol> ToDisposable<TCol> (this TCol collection)
            where TCol: IEnumerable<IDisposable>
        {
            return new DisposableCollection<TCol> (collection);
        }
    }
}
