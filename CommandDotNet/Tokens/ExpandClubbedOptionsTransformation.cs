using System.Collections.Generic;
using CommandDotNet.Parsing;

namespace CommandDotNet.Tokens
{
    internal static class ExpandClubbedOptionsTransformation
    {
        internal static TokenCollection Transform(CommandContext commandContext, TokenCollection tokenCollection)
        {
            return tokenCollection.Transform(ExpandClubbedOption, skipDirectives: true, skipSeparated: true);
        }

        private static IEnumerable<Token> ExpandClubbedOption(Token token)
        {
            if (token.TokenType == TokenType.Option && token.OptionTokenType!.IsClubbed)
            {
                foreach (var flag in token.Value.ToCharArray())
                {
                    var value = flag.ToString();
                    yield return new Token($"-{value}", value, TokenType.Option, new OptionTokenType(value));
                }
            }
            else
            {
                yield return token;
            }
        }
    }
}