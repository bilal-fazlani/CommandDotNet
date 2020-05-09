using System;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    internal class UnrecognizedArgumentCommandParsingException : CommandParsingException
    {
        public Token Token { get; }

        public UnrecognizedArgumentCommandParsingException(Command command, Token token, string message) 
            : base(command, message)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
    }
}