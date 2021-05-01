using System;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    internal static class StringExtensions
    {
        internal static bool IsNullOrEmpty(this string? value) => 
            string.IsNullOrEmpty(value);

        internal static bool IsNullOrWhitespace(this string? value) => 
            string.IsNullOrWhiteSpace(value);

        internal static string? UnlessNullOrWhitespace(this string? value, Func<string, string>? map = null) =>
            value.IsNullOrWhitespace()
                ? null
                : map == null
                    ? value
                    : map(value!);

        internal static string Repeat(this string value, int count) => Enumerable.Repeat(value, count).ToCsv("");
    }
}