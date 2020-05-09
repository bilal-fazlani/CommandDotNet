using System;
using System.Text;

namespace CommandDotNet
{
    /// <summary>
    /// A terse description of an <see cref="Option"/>s long and/or short name.
    /// e.g. "-u | --username"
    /// </summary>
    public static class OptionTemplate
    {
        public static string BuildTemplate(Option option, string delimiter = " | ")
            => Build(option.LongName, option.ShortName, delimiter);

        public static string Build(string? longName, char? shortName, string delimiter = " | ")
        {
            var sb = new StringBuilder();

            void AppendIfNotNull(string prefix, string? value)
            {
                if (value.IsNullOrWhitespace()) return;
                if (sb!.Length > 0) sb.Append(delimiter);
                sb.Append(prefix);
                sb.Append(value);
            }

            AppendIfNotNull("-", shortName?.ToString());
            AppendIfNotNull("--", longName);

            if (sb.Length == 0)
            {
                throw new Exception("Unable to generate template. " +
                                    $"One of either {nameof(longName)} or {nameof(shortName)} must be specified");
            }

            return sb.ToString();
        }
    }
}