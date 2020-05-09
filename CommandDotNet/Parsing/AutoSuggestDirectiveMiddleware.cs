using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Diagnostics;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    internal static class AutoSuggestDirectiveMiddleware
    {
        internal static AppRunner UseAutoSuggestDirective(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(AutoSuggestDirective, 
                    MiddlewareSteps.AutoSuggest.Directive);
                c.UseMiddleware(RegisterWithDotNetSuggest,
                    MiddlewareSteps.AutoSuggest.RegisterWithDotNetSuggest);
            });
        }
        
        private static Task<int> AutoSuggestDirective(CommandContext ctx, ExecutionDelegate next)
        {
            if (!ctx.Tokens.TryGetDirective("suggest", out string? value))
            {
                return next(ctx);
            }

            var parts = value!.Split(':');
            int position = parts.Length == 1
                ? 0
                : int.Parse(parts[1]);

            /* Scenarios:
             * - no command/command/subcommand: show root subcommands & options & first argument AllowedValues
             * - parse error: show subcommands, options, next argument AllowedValues
             *                starting with same prefix as unrecognized argument
             * - no parse error: show subcommands, options, next argument AllowedValues
             */

            if (ctx.ParseResult!.ParseError == null)
            {
                // TODO: include suggestions from first operand

                IEnumerable<string> AddOptionPrefix(Option o)
                {
                    // TODO: extension method for IArgumentNode
                    return o.Aliases.Select(a => a.Length == 1 ? $"-{a}" : $"--{a}");
                }

                var command = ctx.ParseResult.TargetCommand;

                // maybe... to get the next unpopulated argument 
                // ctx.ParseResult.TargetCommand.Operands.FirstOrDefault(o => o.Value == null);

                var optionAliases = command.Options
                    .Where(o => !o.Hidden)
                    .SelectMany(AddOptionPrefix)
                    .OrderBy(a => a);

                command.Subcommands
                    .SelectMany(c => c.Aliases)
                    .OrderBy(a => a)
                    .Union(optionAliases)
                    .ForEach(n => ctx.Console.Out.WriteLine(n));
                
                return ExitCodes.Success;
            }
            
            switch (ctx.ParseResult.ParseError)
            {
                case UnrecognizedArgumentCommandParsingException unrecognizedArgument:
                    // TODO: include suggestions from first operand

                    var command = unrecognizedArgument.Command;
                    var tokenType = unrecognizedArgument.Token.TokenType;
                    break;
                case UnrecognizedValueCommandParsingException unrecognizedValue:
                    // TODO: include suggestions from the allowed values

                    var allowedValues = unrecognizedValue.Argument.AllowedValues;
                    break;
            }

            return ExitCodes.Success;
        }

        private static Task<int> RegisterWithDotNetSuggest(CommandContext ctx, ExecutionDelegate next)
        {
            if (ctx.Tokens.TryGetDirective("dotnet-suggest:register", out string? value))
            {
                // not needed if we're already here
                return next(ctx);
            }

            var parts = value!.Split(':');

            // TODO: cache registration for support
            // TODO: if dotnet-suggest is not installed.
            //       "To enable tab suggestions, run dotnet tool install -g dotnet-suggest."
            //       as suggested here: https://github.com/dotnet/command-line-api/issues/211
            //       & link to this documentation: https://github.com/dotnet/command-line-api/wiki/dotnet-suggest

            var appInfo = AppInfo.GetAppInfo(ctx);
            var fileName = Path.GetFileNameWithoutExtension(appInfo.FilePath);

            var action = parts.Last();

            if (!ExeCmd.TryExecute(
                "dotnet-suggest",
                $"{action} --command-path \"{appInfo.FilePath}\" --suggestion-command \"{fileName}\"",
                out var exeCmd, 
                ctx.Console))
            {
                ctx.Console.Error.WriteLine($"Failed to {action} with dotnet-suggest");
                ctx.Console.Error.WriteLine($"{exeCmd.FileName} exited with {exeCmd.ExitCode}");
                exeCmd.Error?.Print(ctx.Console);
                return ExitCodes.Error;
            }

            return ExitCodes.Success;
        }
    }
}