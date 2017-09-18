using System;
using System.Collections.Generic;

namespace Log4JDash.Web
{
    internal static class Enumerable
    {
        public static IEnumerable<TAccumulate> Scan<TSource, TAccumulate>
        (
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func
        )
        {
            if (source == null)
            {
                throw new ArgumentNullException (nameof (source));
            }
            if (func == null)
            {
                throw new ArgumentNullException (nameof (func));
            }

            var accumulator = seed;
            yield return accumulator;

            foreach (var e in source)
            {
                accumulator = func (accumulator, e);
                yield return accumulator;
            }
        }
    }
}
