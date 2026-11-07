using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Collections
{
    public static class CollectionUtils
    {
        public static List<T> TakeLast<T>(this List<T> list, int count)
        {
            var result = new List<T>();

            for (int i = list.Count - 1, left = count; i >= 0 && left > 0; i--, left--) result.Insert(0, list[i]);
            return result;
        }

        public static V GetOrDefault<K, V>(this Dictionary<K, V> dict, K key)
            where V : class
        {
            dict.TryGetValue(key, out var val);
            return val;
        }

        public static T Random<T>(this IList<T> collection, int minIndex = -1, int maxIndex = -1)
        {
            return collection[UnityEngine.Random.Range(
                minIndex >= 0 ? minIndex : 0,
                maxIndex >= 0 ? maxIndex : collection.Count)];
        }

        public static T Random<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList().Random();
        }

        public static List<T> Values<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}