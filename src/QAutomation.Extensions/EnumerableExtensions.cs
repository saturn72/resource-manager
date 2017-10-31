using System;
using System.Collections;
using System.Collections.Generic;

namespace QAutomation.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEachItem<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var itm in source)
                action(itm);
        }

        public static bool IsEmptyOrNull(this IEnumerable source)
        {
            return source == null || !source.GetEnumerator().MoveNext();
        }
    }
}
