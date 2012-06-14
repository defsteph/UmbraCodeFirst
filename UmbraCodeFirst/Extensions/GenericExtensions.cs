using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace UmbraCodeFirst.Extensions
{
    internal static class GenericExtensions
    {
        public static T SafeCast<T>(this object input)
        {
            if (ReferenceEquals(null, input) || ReferenceEquals(default(T), input))
                return default(T);
            
            if (input is T)
                return (T) input;
            
            return default(T);
        }

        public static IList<T> Copy<T>(this IList<T> list)
        {
            return new List<T>(list);
        }

        public static T Clone<T>(this T original)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, original);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static bool IsNullOrDefault<T>(this T obj)
        {
            return Equals(obj, default(T));
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || !items.Any();
        }

        /// <summary>
        /// If a string contains csv int's, return an array of those valid int's.
        /// </summary>
        public static IEnumerable<int> ToIntArray(this string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                yield break;

            foreach (var s in text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int id;
                if (!int.TryParse(s, out id))
                    continue;

                yield return id;
            }
        }

        public static int FirstIndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var count = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                    return count;
                count++;
            }
            return -1;
        }
    }
}
