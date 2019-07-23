﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;

namespace CommandDotNet.Parsing
{
    internal class CommandParser
    {
        private readonly AppSettings _appSettings;

        private CommandParser(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        internal static Task<int> ParseMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            try
            {
                new CommandParser(commandContext.AppSettings).ParseCommand(commandContext);
            }
            catch (CommandParsingException ex)
            {
                var console = commandContext.AppSettings.Console;
                console.Error.WriteLine(ex.Message);
                ex.PrintStackTrace(console);
                console.Error.WriteLine();
                HelpMiddleware.Print(commandContext.AppSettings, ex.Command);
                return Task.FromResult(1);
            }
            return next(commandContext);
        }

        private void ParseCommand(CommandContext commandContext)
        {
            bool ignoreRemainingArguments = false;
            var remainingArguments = new List<Token>();

            Command currentCommand = commandContext.RootCommand;
            Option currentOption = null;
            IEnumerator<Operand> operands = new OperandEnumerator(currentCommand.Operands);

            var argumentValues = new ArgumentValues();

            foreach (var token in commandContext.Tokens.Arguments)
            {
                switch (token.TokenType)
                {
                    case TokenType.Option:
                        var optionResult = ParseOption(
                            token, currentCommand, out currentOption, argumentValues.GetOrAdd);

                        switch (optionResult)
                        {
                            case ParseOptionResult.Succeeded:
                                break;
                            case ParseOptionResult.UnknownOption:
                                remainingArguments.Add(token);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(optionResult.ToString());
                        }
                        break;
                    case TokenType.Value:
                        if (ignoreRemainingArguments && currentOption == null)
                        {
                            remainingArguments.Add(token);
                        }
                        else
                        {
                            var operandResult = ParseArgumentValue(
                                token, ref currentCommand, ref currentOption, operands, argumentValues.GetOrAdd);

                            switch (operandResult)
                            {
                                case ParseOperandResult.Succeeded:
                                    break;
                                case ParseOperandResult.UnexpectedArgument:
                                    ignoreRemainingArguments = true;
                                    break;
                                case ParseOperandResult.NewSubCommand:
                                    operands = new OperandEnumerator(currentCommand.Operands);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(operandResult.ToString());
                            }
                        }
                        break;
                    case TokenType.Separator:
                        throw new ArgumentOutOfRangeException($"The argument list should have already had the separator removed: {token.RawValue}");
                    case TokenType.Directive:
                        throw new ArgumentOutOfRangeException($"Directives should have already been processed and removed: {token.RawValue}");
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown {nameof(TokenType)}: {token.TokenType}");
                }
            }

            if (currentOption != null) // an option was left without a value
            {
                throw new CommandParsingException(currentCommand, $"Missing value for option '{currentOption.Name}'");
            }

            commandContext.ParseResult = new ParseResult(
                currentCommand, 
                remainingArguments,
                commandContext.Tokens.Separated,
                argumentValues);
        }

        private enum ParseOperandResult
        {
            Succeeded,
            UnexpectedArgument,
            NewSubCommand
        }

        private enum ParseOptionResult
        {
            Succeeded,
            UnknownOption
        }

        private ParseOperandResult ParseArgumentValue(Token token,
            ref Command command,
            ref Option option,
            IEnumerator<Operand> operands, 
            Func<IArgument,List<string>> getArgValues)
        {
            if (option != null)
            {
                if (TryAddValue(option, token.Value, getArgValues))
                {
                    option = null;
                    return ParseOperandResult.Succeeded;
                }

                throw new CommandParsingException(command, $"Unexpected value '{token.RawValue}' for option '{option.Name}'");
            }

            var subCommand = command.Commands
                .FirstOrDefault(c => c.Name.Equals(token.Value, StringComparison.OrdinalIgnoreCase));
            if (subCommand != null)
            {
                command = subCommand;
                option = null;
                return ParseOperandResult.NewSubCommand;
            }

            if (operands.MoveNext())
            {
                getArgValues(operands.Current).Add(token.Value);
            }
            else
            {
                if (_appSettings.ThrowOnUnexpectedArgument)
                {
                    // use the term "argument" for messages displayed to users
                    throw new CommandParsingException(command, $"Unrecognized command or argument '{token.RawValue}'");
                }
                return ParseOperandResult.UnexpectedArgument;
            }

            return ParseOperandResult.Succeeded;
        }

        private ParseOptionResult ParseOption(Token token, 
            Command command, 
            out Option option,
            Func<IArgument, List<string>> getArgValues)
        {
            var optionTokenType = token.OptionTokenType;

            string optionName = optionTokenType.GetName();

            // TODO: use IOption for param
            option = command.FindOption(optionName);

            if (option == null)
            {
                if (_appSettings.ThrowOnUnexpectedArgument)
                {
                    throw new CommandParsingException(command, $"Unrecognized option '{token.RawValue}'");
                }
                return ParseOptionResult.UnknownOption;
            }

            if (optionTokenType.IsClubbed)
            {
                throw new AppRunnerException($"ExpandClubbedOptions transformation should have expanded all clubbed tokens: {token}");
            }
            if (optionTokenType.HasValue)
            {
                throw new AppRunnerException($"SplitOptionAssignments transformation should have split values from all option tokens: {token}");
            }
            if(option.Arity.AllowsNone())
            {
                // No value is needed for this option
                TryAddValue(option, null, getArgValues);
                option = null;
            }

            return ParseOptionResult.Succeeded;
        }
        private static bool TryAddValue(Option option, string value, Func<IArgument, List<string>> getArgValues)
        {
            if (option.Arity.AllowsZeroOrMore())
            {
                getArgValues(option).Add(value);
            }
            else if (option.Arity.AllowsZeroOrOne())
            {
                if (getArgValues(option).Any())
                {
                    return false;
                }
                getArgValues(option).Add(value);
            }
            else if (option.Arity.AllowsNone())
            {
                if (value != null)
                {
                    return false;
                }
                // Add a value to indicate that this option was specified
                getArgValues(option).Add("true");
            }
            return true;
        }

        private class OperandEnumerator : IEnumerator<Operand>
        {
            private readonly IEnumerator<Operand> _enumerator;

            public OperandEnumerator(IEnumerable<Operand> enumerable)
            {
                _enumerator = enumerable.GetEnumerator();
            }

            public Operand Current => _enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (Current == null || !Current.Arity.AllowsZeroOrMore())
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