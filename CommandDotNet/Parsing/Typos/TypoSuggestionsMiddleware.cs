using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing.Typos
{
    internal static class TypoSuggestionsMiddleware
    {
        internal static AppRunner UseTypoSuggestions(AppRunner appRunner, int maxSuggestionCount)
        {
            if (maxSuggestionCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSuggestionCount), $"{maxSuggestionCount} must be > 0");
            }

            return appRunner.Configure(c =>
            {  
                c.UseMiddleware(TypoSuggest, MiddlewareSteps.TypoSuggest);
                c.Services.Add(new Config {MaxSuggestionCount = maxSuggestionCount}); 
            });
        }

        private class Config
        {
            public int MaxSuggestionCount;
        }

        private static Task<int> TypoSuggest(CommandContext ctx, ExecutionDelegate next)
        {
            if (ctx.ParseResult!.ParseError != null)
            {
                switch (ctx.ParseResult.ParseError)
                {
                    case UnrecognizedArgumentCommandParsingException unrecognizedArgument:
                        if (TrySuggestArgumentNames(ctx, unrecognizedArgument.Command, unrecognizedArgument.Token))
                        {
                            // in case help was requested by CommandParser
                            ctx.ShowHelpOnExit = false;
                            return ExitCodes.Error;
                        }

                        // TODO: suggest other directives? We'd need a list of names which we don't collect atm.
                        break;
                    case UnrecognizedValueCommandParsingException unrecognizedValue:
                        if (TrySuggestValues(ctx, unrecognizedValue))
                        {
                            // in case help was requested by CommandParser
                            ctx.ShowHelpOnExit = false;
                            return ExitCodes.Error;
                        }

                        break;
                }
            }

            return next(ctx);
        }

        private static bool TrySuggestValues(CommandContext ctx, UnrecognizedValueCommandParsingException unrecognizedValue)
        {
            return TrySuggest(ctx, unrecognizedValue.Command, unrecognizedValue.Token.Value,
                unrecognizedValue.Argument.AllowedValues.ToCollection(),
                $"is not a valid {unrecognizedValue.Argument.TypeInfo.DisplayName}", 
                null);
        }

        private static bool TrySuggestArgumentNames(CommandContext ctx, Command command, Token token)
        {
            ICollection<string> GetOptionNames()
            {
                // skip short names
                return command.Options
                    .Where(o => !o.Hidden)
                    .SelectMany(o => o.Aliases.Where(a => a.Length > 1))
                    .ToList();
            }

            ICollection<string> GetSubcommandNames()
            {
                return command.Subcommands.SelectMany(o => o.Aliases).ToList();
            }

            return token.TokenType switch
            {
                TokenType.Option => TrySuggest(ctx, command, token.Value, 
                    GetOptionNames(), "is not a valid option", "--"),
                TokenType.Value => TrySuggest(ctx, command, token.Value, 
                    GetSubcommandNames(), "is not a valid subcommand", null),
                _ => false
            };
        }

        private static bool TrySuggest(CommandContext ctx,
            Command command, string typo,
            ICollection<string> candidates,
            string message, string? prefix)
        {
            if (!candidates.Any())
            {
                return false;
            }

            var config = ctx.AppConfig.Services.GetOrThrow<Config>();

            // TODO: allowed values
            //   if the next value should be a value for an option, include allowed values for the option.
            //   else include allowed values of the next expected operand
            //      fyi: the next expected operand could be a list operand that already has values.
            //           logic should remain the same, just be sure to include this test case

            var suggestions = candidates
                .RankAndTrimSuggestions(typo, config.MaxSuggestionCount)
                .ToList();

            if (!suggestions.Any())
            {
                return false;
            }

            var usage = command.GetAppName(ctx.AppConfig.AppSettings.Help) + " " + command.GetPath();

            var @out = ctx.Console.Out;
            @out.WriteLine($"'{typo}' {message}");
            @out.WriteLine();
            @out.WriteLine("Did you mean ...");
            suggestions.ForEach(name => @out.WriteLine($"   {prefix}{name}"));
            @out.WriteLine();
            @out.WriteLine($"See '{usage} --help'");

            return true;
        }
    }
}