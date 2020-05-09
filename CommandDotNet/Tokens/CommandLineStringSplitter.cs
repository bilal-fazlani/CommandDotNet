using System.Collections.Generic;

namespace CommandDotNet.Tokens
{
    public class CommandLineStringSplitter : ICommandLineStringSplitter
    {
        // copied from System.CommandLine
        // https://github.com/dotnet/command-line-api/blob/master/src/System.CommandLine/Parsing/CommandLineStringSplitter.cs

        internal static readonly ICommandLineStringSplitter Instance = new CommandLineStringSplitter();

        private enum Boundary
        {
            TokenStart,
            WordEnd,
            QuoteEnd
        }

        public IEnumerable<string> Split(string commandLine)
        {
            if (commandLine.IsNullOrEmpty())
            {
                yield break;
            }

            var memory = commandLine.ToCharArray();

            var startTokenIndex = 0;

            var pos = 0;

            var seeking = Boundary.TokenStart;
            int? skipQuoteAtIndex = null;

            while (pos < memory.Length)
            {
                var c = memory[pos];

                if (char.IsWhiteSpace(c))
                {
                    switch (seeking)
                    {
                        case Boundary.WordEnd:
                            yield return CurrentToken();
                            startTokenIndex = pos;
                            seeking = Boundary.TokenStart;
                            break;

                        case Boundary.TokenStart:
                            startTokenIndex = pos;
                            break;

                        case Boundary.QuoteEnd:
                            break;
                    }
                }
                else if (c == '\"')
                {
                    switch (seeking)
                    {
                        case Boundary.QuoteEnd:
                            yield return CurrentToken();
                            startTokenIndex = pos;
                            seeking = Boundary.TokenStart;
                            break;

                        case Boundary.TokenStart:
                            startTokenIndex = pos + 1;
                            seeking = Boundary.QuoteEnd;
                            break;

                        case Boundary.WordEnd:
                            seeking = Boundary.QuoteEnd;
                            skipQuoteAtIndex = pos;
                            break;
                    }
                }
                else if (seeking == Boundary.TokenStart)
                {
                    seeking = Boundary.WordEnd;
                    startTokenIndex = pos;
                }

                Advance();

                if (IsAtEndOfInput())
                {
                    switch (seeking)
                    {
                        case Boundary.TokenStart:
                            break;
                        default:
                            yield return CurrentToken();
                            break;
                    }
                }
            }

            void Advance() => pos++;

            string CurrentToken()
            {
                if (skipQuoteAtIndex == null)
                {
                    return commandLine.Substring(
                        startTokenIndex,
                        IndexOfEndOfToken());
                }
                else
                {
                    var beforeQuote = commandLine.Substring(
                        startTokenIndex,
                        skipQuoteAtIndex.Value - startTokenIndex);

                    var indexOfCharAfterQuote = skipQuoteAtIndex.Value + 1;

                    var afterQuote = commandLine.Substring(
                        indexOfCharAfterQuote,
                        pos - skipQuoteAtIndex.Value - 1);

                    skipQuoteAtIndex = null;

                    return $"{beforeQuote}{afterQuote}";
                }
            }

            int IndexOfEndOfToken() => pos - startTokenIndex;

            bool IsAtEndOfInput() => pos == memory.Length;
        }
    }
}