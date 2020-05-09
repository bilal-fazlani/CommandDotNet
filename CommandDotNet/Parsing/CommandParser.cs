using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    internal class CommandParser
    {
        private readonly CommandContext _commandContext;
        private readonly AppSettings _appSettings;

        private CommandParser(CommandContext commandContext)
        {
            _commandContext = commandContext;
            _appSettings = commandContext.AppConfig.AppSettings;
        }

        internal static Task<int> ParseInputMiddleware(CommandContext commandContext, ExecutionDelegate next)
        {
            try
            {
                new CommandParser(commandContext).ParseCommand(commandContext);
            }
            catch (CommandParsingException ex)
            {
                commandContext.ParseResult = new ParseResult(ex.Command, ex);
            }
            return next(commandContext);
        }

        private void ParseCommand(CommandContext commandContext)
        {
            bool ignoreRemainingArguments = false;
            var remainingOperands = new List<Token>();

            Command currentCommand = commandContext.RootCommand!;
            Option? currentOption = null;
            Token? currentOptionToken = null;
            var operands = new OperandEnumerator(currentCommand.Operands);

            void ParseNonOptionValue(Token token)
            {
                var operandResult = ParseArgumentValue(token, ref currentCommand, operands!);

                switch (operandResult)
                {
                    case ParseOperandResult.Succeeded:
                        break;
                    case ParseOperandResult.UnexpectedArgument:
                        ignoreRemainingArguments = true;
                        remainingOperands!.Add(token);
                        break;
                    case ParseOperandResult.NewSubCommand:
                        operands = new OperandEnumerator(currentCommand.Operands);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(operandResult.ToString());
                }
            }

            foreach (var token in commandContext.Tokens.Arguments)
            {
                switch (token.TokenType)
                {
                    case TokenType.Option:
                        if (currentOption is { })
                        {
                            throw new CommandParsingException(currentCommand, $"Missing value for option '{currentOption.Name}'");
                        }
                        currentOption = ParseOption(token, currentCommand);
                        if (currentOption is { })
                        {
                            currentOptionToken = token;
                        }
                        break;
                    case TokenType.Value:
                        if (currentOption is { })
                        {
                            ParseOptionValue(token, currentCommand, currentOption, currentOptionToken!);
                            currentOption = null;
                            currentOptionToken = null;
                        }
                        else if (ignoreRemainingArguments)
                        {
                            remainingOperands.Add(token);
                        }
                        else
                        {
                            ParseNonOptionValue(token);
                        }
                        break;
                    case TokenType.Separator:
                        throw new ArgumentOutOfRangeException($"The argument separator should have already been processed and removed: {token.RawValue}");
                    case TokenType.Directive:
                        throw new ArgumentOutOfRangeException($"Directives should have already been processed and removed: {token.RawValue}");
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown {nameof(TokenType)}: {token.TokenType}");
                }
            }

            if (currentOption is { }) // an option was left without a value
            {
                throw new CommandParsingException(currentCommand, $"Missing value for option '{currentOption.Name}'");
            }

            if (commandContext.Tokens.Separated.Any() && UsingEndOfOptions(currentCommand))
            {
                commandContext.Tokens.Separated.ForEach(ParseNonOptionValue);
            }

            commandContext.ParseResult = new ParseResult(
                currentCommand,
                remainingOperands,
                commandContext.Tokens.Separated);
        }

        private enum ParseOperandResult
        {
            Succeeded,
            UnexpectedArgument,
            NewSubCommand
        }

        private void ParseOptionValue(Token token,
            Command command, Option currentOption, Token currentOptionToken)
        {
            ThrowIfValueNotAllowed(command, currentOption, token);
            if (TryAddValue(currentOption, token, currentOptionToken))
            {
                return;
            }

            throw new CommandParsingException(command,
                $"Unexpected value '{token.RawValue}' for option '{currentOption.Name}'");
        }

        private ParseOperandResult ParseArgumentValue(Token token, ref Command command, OperandEnumerator operands)
        {
            if (!operands.Any && command.FindArgumentNode(token.Value) is Command subcommand)
            {
                command = subcommand;
                return ParseOperandResult.NewSubCommand;
            }

            if (operands.MoveNext())
            {
                var current = operands.Current;
                ThrowIfValueNotAllowed(command, current, token);
                GetArgumentParsedValues(current).Add(new ValueFromToken(token.Value, token, null));
            }
            else
            {
                if (command.GetIgnoreUnexpectedOperands(_appSettings))
                {
                    return ParseOperandResult.UnexpectedArgument;
                }

                // use the term "argument" for messages displayed to users
                throw new UnrecognizedArgumentCommandParsingException(command, token, $"Unrecognized command or argument '{token.RawValue}'");
            }

            return ParseOperandResult.Succeeded;
        }

        private Option? ParseOption(Token token, Command command)
        {
            var optionTokenType = token.OptionTokenType!;

            var option = command.FindOption(optionTokenType.GetName());
            if (option is null)
            {
                throw new UnrecognizedArgumentCommandParsingException(command, token, $"Unrecognized option '{token.RawValue}'");
            }

            if (optionTokenType.IsClubbed)
            {
                throw new AppRunnerException($"ExpandClubbedOptions transformation should have expanded all clubbed tokens: {token}");
            }
            if (optionTokenType.HasValue)
            {
                throw new AppRunnerException($"SplitOptionAssignments transformation should have split values from all option tokens: {token}");
            }
            if (option.Arity.AllowsNone())
            {
                TryAddValue(option, null, token);
                return null;
            }

            return option;
        }

        private static ICollection<ValueFromToken> GetArgumentParsedValues(IArgument argument)
        {
            // in most cases, this will be the first or only InputValues
            var source = Constants.InputValueSources.Argument;
            var parserValues = argument.InputValues.FirstOrDefault(iv => iv.Source == source);
            
            if (parserValues is null)
            {
                parserValues = new InputValue(source, false, new List<ValueFromToken>());
                argument.InputValues.Add(parserValues);
            }
            parserValues.ValuesFromTokens ??= new List<ValueFromToken>();

            return (List<ValueFromToken>)parserValues.ValuesFromTokens!;
        }

        private void ThrowIfValueNotAllowed(Command command, IArgument argument, Token token)
        {
            if (argument.AllowedValues.IsNullOrEmpty() || argument.AllowedValues.Contains(token.Value))
            {
                return;
            }

            // help displays the AllowedValues
            // TypoSuggestions can hide the help
            _commandContext.ShowHelpOnExit = true;
            throw new UnrecognizedValueCommandParsingException(
                command, argument, token, $"Unrecognized value '{token.RawValue}' for {(argument is Option ? "option" : "argument")}: {argument.Name}");
        }

        private bool TryAddValue(Option option, Token? valueToken, Token optionToken)
        {
            var values = GetArgumentParsedValues(option);

            if (option.Arity.AllowsMany())
            {
                values.Add(new ValueFromToken(valueToken!.Value, valueToken, optionToken));
            }
            else if (option.Arity.AllowsNone())
            {
                // only possible for flags which are boolean type
                // Add a value to indicate that this option was specified
                values.Add(new ValueFromToken("true", valueToken, optionToken));
            }
            else
            {
                if (values.Any())
                {
                    return false;
                }
                values.Add(new ValueFromToken(valueToken!.Value, valueToken, optionToken));
            }
            return true;
        }

        bool UsingEndOfOptions(Command command)
        {
            var separatorStrategy = command.GetArgumentSeparatorStrategy(_appSettings);
            return separatorStrategy == ArgumentSeparatorStrategy.EndOfOptions;
        }

        private class OperandEnumerator : IEnumerator<Operand>
        {
            private readonly IEnumerator<Operand> _enumerator;
            private int _count;

            public OperandEnumerator(IEnumerable<Operand> enumerable)
            {
                _enumerator = enumerable.GetEnumerator();
            }

            public bool Any => _count > 0;

            public Operand Current => _enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                _count++;

                if (Current is null || !Current.Arity.AllowsMany())
                {
                    return _enumerator.MoveNext();
                }

                // If current operand allows multiple values, we don't move forward and
                // all later values will be added to current IOperand.Values
                return true;
            }

            public void Reset()
            {
                _enumerator.Reset();
            }
        }
    }
}