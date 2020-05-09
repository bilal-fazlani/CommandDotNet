using System;
using CommandDotNet.Parsing;

namespace CommandDotNet.Tokens
{
    public class Token
    {
        /// <summary>
        /// The raw value from the user input.
        /// This will contain the punctuation used to denote option and argument names.
        /// </summary>
        public string RawValue { get; }

        /// <summary>Can be an Option name or an argument value</summary>
        public string Value { get; }

        /// <summary>The <see cref="Tokens.TokenType"/></summary>
        public TokenType TokenType { get; }

        /// <summary>When <see cref="TokenType"/> is <see cref="Tokens.TokenType.Option"/>, this will be populated.</summary>
        public OptionTokenType? OptionTokenType { get; }

        public string? SourceName { get; internal set; }

        public Token? SourceToken { get; internal set; }

        public Token(
            string rawValue,
            string value, 
            TokenType tokenType, 
            OptionTokenType? optionTokenType = null)
        {
            RawValue = rawValue;
            Value = value;
            TokenType = tokenType;
            if (tokenType == TokenType.Option && optionTokenType is null)
            {
                throw new ArgumentNullException($"{nameof(optionTokenType)} cannot be null when {nameof(tokenType)} is {TokenType.Option}");
            }
            OptionTokenType = optionTokenType;
        }

        public override string ToString()
        {
            return $"{nameof(Token)}:{TokenType}>{RawValue}";
        }
    }
}