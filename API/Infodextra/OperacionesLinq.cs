using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Infodextra
{
    public static class OperacionesLinq
    {
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }
    }
}
