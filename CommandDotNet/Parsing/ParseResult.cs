using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Tokens;
using static System.Environment;

namespace CommandDotNet.Parsing
{
    public class ParseResult : IIndentableToString
    {
        /// <summary>The command that addressed by the command line arguments</summary>
        public Command TargetCommand { get; }

        /// <summary>
        /// If extra operands were provided and <see cref="AppSettings.IgnoreUnexpectedOperands"/> is true,
        /// The extra operands will be stored in the <see cref="RemainingOperands"/> collection.
        /// </summary>
        public IReadOnlyCollection<string> RemainingOperands { get; }

        /// <summary>
        /// All arguments provided after the argument separator "--" will be stored
        /// in the <see cref="SeparatedArguments"/> collection.
        /// </summary>
        public IReadOnlyCollection<string> SeparatedArguments { get; }

        /// <summary>
        /// An exception encountered while parsing the commands.
        /// Help should be printed for the <see cref="TargetCommand"/>
        /// </summary>
        public Exception? ParseError { get; }

        /// <summary>
        /// Returns true if the help option was specified
        /// for the target command or any of its parent commands
        /// </summary>
        public bool HelpWasRequested() =>
            TargetCommand.GetParentCommands(includeCurrent: true).Any(c => c.HelpWasRequested());

        public ParseResult(Command command,
            IReadOnlyCollection<Token> remainingOperands,
            IReadOnlyCollection<Token> separatedArguments)
        {
            TargetCommand = command ?? throw new ArgumentNullException(nameof(command));
            RemainingOperands = remainingOperands?.ToArgsArray() ?? new string[0];
            SeparatedArguments = separatedArguments?.ToArgsArray() ?? new string[0];
        }

        public ParseResult(Command command, Exception exception)
        {
            TargetCommand = command ?? throw new ArgumentNullException(nameof(command));
            ParseError = exception ?? throw new ArgumentNullException(nameof(exception));
            RemainingOperands = new string[0];
            SeparatedArguments = new string[0];
        }

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            return $"{nameof(ParseResult)}:{NewLine}" +
                   $"{indent}{nameof(TargetCommand)}:{TargetCommand}{NewLine}" +
                   $"{indent}{nameof(RemainingOperands)}:{RemainingOperands.ToCsv()}{NewLine}" +
                   $"{indent}{nameof(SeparatedArguments)}:{SeparatedArguments.ToCsv()}{NewLine}" +
                   $"{indent}{nameof(ParseError)}:{ParseError?.Message}";
        }
    }
}