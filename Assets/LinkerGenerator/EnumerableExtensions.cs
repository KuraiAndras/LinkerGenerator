using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkerGenerator
{
    internal static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static IEnumerable<TItem> Concat<TItem>(this IEnumerable<TItem> enumerable, TItem item) => enumerable.Concat(new[] { item });
    }
}