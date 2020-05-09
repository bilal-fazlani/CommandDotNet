using CommandDotNet.Tokens;

namespace CommandDotNet
{
    public class ValueFromToken
    {
        public string Value { get; }
        public Token? ValueToken { get; }
        public Token? OptionToken { get; }

        public string? TokenSourceName => ValueToken?.SourceName ?? OptionToken?.SourceName;
        public Token? TokensSourceToken => ValueToken?.SourceToken ?? OptionToken?.SourceToken;
        
        public ValueFromToken(string value, Token? valueToken, Token? optionToken)
        {
            Value = value;
            ValueToken = valueToken;
            OptionToken = optionToken;
        }

        public override string ToString()
        {
            return $"{nameof(ValueFromToken)}={Value} {nameof(ValueToken)}:{ValueToken} {nameof(OptionToken)}:{OptionToken}";
        }
    }
}