using System;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    internal class UnrecognizedValueCommandParsingException : CommandParsingException
    {
        public IArgument Argument { get; }
        public Token Token { get; }

        public UnrecognizedValueCommandParsingException(Command command, IArgument argument, Token unrecognizedValue, string message) 
            : base(command, message)
        {
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
            Token = unrecognizedValue ?? throw new ArgumentNullException(nameof(unrecognizedValue));
        }
    }
}