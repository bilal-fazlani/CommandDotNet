using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Parsing;

namespace CommandDotNet.Tokens
{
    public static class Tokenizer
    {
        internal static Token SeparatorToken { get; } = new Token("--", "--", TokenType.Separator);

        public static TokenCollection Tokenize(this IEnumerable<string> args, bool includeDirectives = false, string sourceName = "args")
        {
            return new TokenCollection(ParseTokens(args, includeDirectives, sourceName));
        }

        public static Token Tokenize(string arg, bool includeDirectives = false, string sourceName = "args")
        {
            Token token = (includeDirectives && TryTokenizeDirective(arg, out Token? parsedToken))
                                || TryTokenizeSeparator(arg, out parsedToken)
                                || TryTokenizeOption(arg, out parsedToken)
                ? parsedToken!
                : TokenizeValue(arg);
            token.SourceName = sourceName;
            return token;
        }

        public static bool TryTokenizeDirective(string arg, out Token? token)
        {
            if (arg.Length > 2 && arg[0] == '[' && arg[arg.Length - 1] == ']')
            {
                token = new Token(arg, arg.Substring(1, arg.Length - 2), TokenType.Directive);
                return true;
            }

            token = null;
            return false;
        }

        public static bool TryTokenizeSeparator(string arg, out Token? token)
        {
            if (arg == SeparatorToken.Value)
            {
                token = new Token(arg, arg, TokenType.Separator);
                return true;
            }

            token = null;
            return false;
        }

        public static bool TryTokenizeOption(string arg, out Token? token)
        {
            if (arg.Length <= 1 || arg[0] != '-' || arg.StartsWith("---"))
            {
                token = null;
                return false;
            }

            bool isLongOption = arg[1] == '-';
            bool isShortOption = !isLongOption;

            var value = arg.Substring(isShortOption ? 1 : 2);
            var assignmentIndex = value.IndexOfAny(new[] { ':', '=' });
            var hasValue = assignmentIndex > 0;
            var isClubbed = isShortOption && !hasValue && value.Length > 1;

            token = new Token(arg, value, TokenType.Option,
                new OptionTokenType(value, isLongOption, isClubbed, hasValue, assignmentIndex));
            return true;
        }

        public static Token TokenizeValue(string argValue)
        {
            return new Token(argValue, argValue, TokenType.Value);
        }

        public static string[] ToArgsArray(this IEnumerable<Token> tokens)
        {
            return tokens.Select(t => t.RawValue).ToArray();
        }

        private static IEnumerable<Token> ParseTokens(IEnumerable<string> args, bool includeDirectives, string sourceName)
        {
            bool foundSeparator = false;

            foreach (var arg in args)
            {
                if (foundSeparator)
                {
                    yield return new Token(arg, arg, TokenType.Value){SourceName = sourceName};
                }
                else
                {
                    var token = Tokenize(arg, includeDirectives, sourceName);

                    includeDirectives = includeDirectives && token.TokenType == TokenType.Directive;
                    foundSeparator = token.TokenType == TokenType.Separator;

                    yield return token;
                }
            }
        }
    }
}