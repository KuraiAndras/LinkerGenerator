using System.Collections.Generic;
using System.Linq;

namespace LinkerGenerator
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<TItem> Concat<TItem>(this IEnumerable<TItem> enumerable, TItem item) => enumerable.Concat(new[] { item });
    }
}