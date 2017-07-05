using System;
using System.Collections.Generic;

namespace Ak.Generic.Extensions
{
    ///<summary>
    ///</summary>
    public static class DictionaryExtension
    {
        internal static void SumItem<TKey>(this IDictionary<TKey, Double> sumList, TKey key, Double value)
        {
            if (sumList.ContainsKey(key))
                sumList[key] += value;
            else
                sumList.Add(key, value);
        }
    }
}
