using System;
using System.Reflection;

namespace CommandDotNet.Extensions
{
    internal static class CustomAttributeProviderExtensions
    {
        internal static bool HasAttribute<T>(this ICustomAttributeProvider attributeProvider) where T : Attribute
        {
            return attributeProvider.IsDefined(typeof(T), true);
        }
    }
}