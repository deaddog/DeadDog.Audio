using System;
using System.Collections.Generic;

namespace DeadDog.Audio
{
    internal static class LinqExtension
    {
        public static T AllSameOrDefault<T, TValue>(this IEnumerable<T> collection, Func<T, TValue> selector)
        {
            var e = collection.GetEnumerator();

            if (!e.MoveNext())
                return default(T);

            var value = e.Current;
            var cmp = selector(value);

            while (e.MoveNext())
                if (!selector(e.Current).Equals(cmp))
                    return default(T);

            return value;
        }
    }
}
