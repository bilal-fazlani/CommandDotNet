using System;
using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.TestTools
{
    internal static class AssertionExtensions
    {
        internal static void ShouldBe(this string? actual, string? expected, string valueName)
        {
            if (expected?.Equals(actual) ?? actual == null)
            {
                return;
            }

            Fail(valueName, expected, actual);
        }

        private static void Fail(string valueName, object? expected, object? actual)
        {
            throw new AssertFailedException(
                $"expected value for {valueName} to be {FormatMessage(expected)} but was {FormatMessage(actual)}");
        }

        private static string FormatMessage(object? item) => item == null ? "<null>" : $"{Environment.NewLine}\"{item}\"{Environment.NewLine}";
    }
}