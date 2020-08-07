using System;
using System.Collections.Generic;

namespace Either.Extension
{
    internal static class IEnumerableExtension
    {
        public static void ForEach<TValue>(this IEnumerable<TValue> collection, Action<TValue> action)
        {
            foreach(var value in collection)
            {
                action(value);
            }
        }
    }
}
