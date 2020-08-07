using System;
using System.Collections.Generic;
using System.Text;

namespace Either.Extension
{
    public static class DictionaryExtension
    {
        public static void SetValuesToDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            foreach(var key in dictionary.Keys)
            {
                dictionary[key] = default;
            }
        }
    }
}
