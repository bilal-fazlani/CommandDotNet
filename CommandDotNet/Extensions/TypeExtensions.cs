using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CommandDotNet.Extensions
{
    internal static class TypeExtensions
    {
        internal static IEnumerable<MethodInfo> GetDeclaredMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Where(m => !typeof(IDisposable).IsAssignableFrom(type) || m.Name != nameof(IDisposable.Dispose));
        }

        internal static bool IsNullableType(this Type type) => Nullable.GetUnderlyingType(type) != null;

        internal static Type GetUnderlyingType(this Type type)
        {
            return Nullable.GetUnderlyingType(type)
                   ?? type.GetListUnderlyingType()
                   ?? type;
        }

        internal static Type? GetListUnderlyingType(this Type type)
        {
            return type.IsArray
                ? type.GetElementType()
                : typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType
                    ? type.GetGenericArguments().FirstOrDefault()
                    : null;
        }

        internal static bool IsEnumerable(this Type type)
        {
            return type.GetInterfaces()
                .Concat(type.ToEnumerable())
                .Any(x => x == typeof(IEnumerable));
        }

        internal static bool IsCollection(this Type type)
        {
            return type.GetInterfaces()
                .Concat(type.ToEnumerable())
                .Any(x => x.IsGenericType
                    ? x.GetGenericTypeDefinition() == typeof(ICollection<>)
                    : x == typeof(ICollection));
        }

        internal static object GetDefaultValue(this Type type)
        {
            Func<object> f = GetDefaultValue<object>;
            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(null, null);
        }

        internal static bool IsDefaultFor(this object defaultValue, Type type)
        {
            return Equals(defaultValue, type.GetDefaultValue());
        }

        [return: MaybeNull]
        private static T GetDefaultValue<T>()
        {
            return Box<T>.CreateDefault().Value;
        }

        internal static bool IsCompilerGenerated(this Type t) {
            if (t == null)
                return false;

            return t.IsDefined(typeof(CompilerGeneratedAttribute), false)
                   || IsCompilerGenerated(t.DeclaringType);
        }

        internal static IEnumerable<PropertyInfo> GetDeclaredProperties<TAttribute>(this Type type) where TAttribute: Attribute
        {
            return type
                .GetDeclaredProperties()
                .Where(x => x.HasAttribute<TAttribute>());
        }
        
        internal static IEnumerable<PropertyInfo> GetDeclaredProperties(this Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !x.PropertyType.IsCompilerGenerated());
        }

        internal static bool InheritsFrom<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    }
}