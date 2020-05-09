using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using static System.Environment;

namespace CommandDotNet.Extensions
{
    internal static class ObjectExtensions
    {
        internal static bool IsNullValue(this object? obj)
        {
            return obj == null || obj == DBNull.Value;
        }

        /// <summary></summary>
        internal static string? ValueToString(this object value, bool isPassword = false)
        {
            if (value.IsNullValue())
            {
                return null;
            }

            if (isPassword)
            {
                return Password.ValueReplacement;
            }

            if (value is string str)
            {
                return str;
            }

            if (value is IEnumerable collection)
            {
                return collection.Cast<object>().Select(i => i?.ToString()).ToCsv(", ");
            }

            return value.ToString();
        }

        internal static string ToStringFromPublicProperties(this object item, Indent? indent = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            indent ??= new Indent();

            var props = item
                .GetType()
                .GetProperties(BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly)
                .Where(p => !p.HasAttribute<ObsoleteAttribute>())
                .OrderBy(p => p.Name)
                .Select(p =>
                {
                    var value = p.GetValue(item);
                    var stringValue = value is IIndentableToString logToString
                        ? logToString.ToString(indent.Increment())
                        : value;
                    return $"{indent}{p.Name}: {stringValue}";
                })
                .ToCsv(NewLine);

            return $"{item.GetType().Name}:{NewLine}{props}";
        }

        internal static object CloneWithPublicProperties(this object original, bool recurse = true)
        {
            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            var type = original.GetType();
            object clone = Activator.CreateInstance(type);

            type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(p => p.CanRead && p.CanWrite)
                .ForEach(p =>
                {
                    var value = p.GetValue(original);
                    if (value != null)
                    {
                        if (recurse && p.PropertyType != typeof(string) && p.PropertyType.IsClass)
                        {
                            value = CloneWithPublicProperties(value);
                        }
                        p.SetValue(clone, value);
                    }
                });

            return clone;
        }
    }
}