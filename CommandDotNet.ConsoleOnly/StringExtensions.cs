using System;

namespace CommandDotNet.ConsoleOnly
{
    internal static class StringExtensions
    {
        internal static string[] SplitIntoLines(this string text, StringSplitOptions stringSplitOptions = StringSplitOptions.None) =>
            text.Split(new[] { "\r\n", "\r", "\n" }, stringSplitOptions);
    }
}