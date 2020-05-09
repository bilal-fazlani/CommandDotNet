using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static bool IsNullOrEmpty<T>(this IEnumerable<T> items) => items == null || !items.Any();

        internal static bool IsEmpty<T>(this IEnumerable<T> items) => !items.Any();

        internal static IEnumerable<T> ToEnumerable<T>(this T instance)
        {
            yield return instance;
        }

        public static ICollection<T> ToCollection<T>(this IEnumerable<T> items) => items as ICollection<T> ?? items.ToList();

        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> items) => items as IReadOnlyCollection<T> ?? items.ToList().AsReadOnly();

        public static string ToCsv(this IEnumerable<string?> items, string separator = ",")
        {
            return string.Join(separator, items);
        }

        public static string ToCsv<T>(this IEnumerable<T> items, string separator = ",")
        {
            return items.Select(i => i?.ToString()).ToCsv(separator);
        }

        internal static string ToCsv(this IEnumerable items, string separator = ",")
        {
            return items.Cast<object>().ToCsv(separator);
        }

        /// <summary>Joins the string into a sorted delimited string. Useful for logging.</summary>
        public static string ToOrderedCsv(this IEnumerable<string?> items, string separator = ",")
        {
            return items.OrderBy(i => i).ToCsv(separator);
        }

        /// <summary>Joins the object.ToString() into a sorted delimited string. Useful for logging.</summary>
        public static string ToOrderedCsv<T>(this IEnumerable<T> items, string separator = ",")
        {
            return items.Select(i => i?.ToString()).ToOrderedCsv(separator);
        }

        internal static string ToOrderedCsv(this IEnumerable items, string separator = ",")
        {
            return items.Cast<object>().ToOrderedCsv(separator);
        }

        internal static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        internal static IEnumerable<string> EnumeratePipedInput(this Func<string?> readLine)
        {
            while (true)
            {
                var line = readLine();
                if (line == null)
                {
                    yield break;
                }

                yield return line;
            }
        }
        
        internal static T? SingleOrDefaultOrThrow<T>(this IEnumerable<T?> source, Action throwEx) where T: class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (throwEx == null)
            {
                throw new ArgumentNullException(nameof(throwEx));
            }

            if (source is ICollection<T> list)
            {
                switch (list.Count)
                {
                    case 0: return default;
                    case 1: return list.First();
                }
            }
            else
            {
                using var e = source.GetEnumerator();
                if (!e.MoveNext()) return default;
                var result = e.Current;
                if (!e.MoveNext()) return result;
            }

            throwEx();
            return default;
        }
    }
}