using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;
using CommandDotNet.Builders;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.Parsing
{
    internal static class AutoSuggestDirectiveMiddleware
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        internal static AppRunner UseAutoSuggestDirective(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(AutoSuggestDirective, 
                    MiddlewareSteps.AutoSuggest.Stage, MiddlewareSteps.AutoSuggest.Order);
                c.UseMiddleware(RegisterWithDotNetSuggest,
                    MiddlewareSteps.AutoSuggest.Stage, MiddlewareSteps.AutoSuggest.Order-1);
            });
        }
        
        private static Task<int> AutoSuggestDirective(CommandContext ctx, ExecutionDelegate next)
        {
            if (!ctx.Tokens.TryGetDirective("suggest", out string value))
            {
                return next(ctx);
            }

            var parts = value.Split(':');
            int position = parts.Length == 1
                ? 0
                : int.Parse(parts[1]);

            /* Scenarios:
             * - no command/command/subcommand: show subcommands & options
             * - parse error: show subcommands or options starting with same prefix as unrecognized argument
             * - enum support: show allowed values
             *   - update TypoSuggestions middleware to benefit
             *
             */


            if (ctx.ParseResult.ParseError == null)
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
                    .Where(o => o.ShowInHelp)
                    .SelectMany(AddOptionPrefix)
                    .OrderBy(a => a);

                command.Subcommands
                    .SelectMany(c => c.Aliases)
                    .OrderBy(a => a)
                    .Union(optionAliases)
                    .ForEach(n => ctx.Console.Out.WriteLine(n));
                return Task.FromResult(0);
            }
            
            if(ctx.ParseResult.ParseError is CommandParsingException cpe
               && cpe.UnrecognizedArgument != null)
            {
                // TODO: include suggestions from first operand

                var command = cpe.Command;
                var tokenType = cpe.UnrecognizedArgument.TokenType;
            }

            return Task.FromResult(1);
        }

        private static Task<int> RegisterWithDotNetSuggest(CommandContext ctx, ExecutionDelegate next)
        {
            if (ctx.Tokens.TryGetDirective("suggest", out string value))
            {
                // not needed if we're already here
                return next(ctx);
            }

            // TODO: cache registration for support

            var appInfo = AppInfo.GetAppInfo(ctx);

            var stdOut = new StringBuilder();
            var stdErr = new StringBuilder();

            var fileName = Path.GetFileNameWithoutExtension(appInfo.FilePath);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet-suggest",
                    Arguments = $"register --command-path \"{appInfo.FilePath}\" --suggestion-command \"{fileName}\"",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            process.OutputDataReceived += (sender, args) => stdOut.Append(args.Data);
            process.ErrorDataReceived += (sender, args) => stdErr.Append(args.Data);

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // TODO: honor CancellationToken
                process.WaitForExit();

                var log = $"{process.StartInfo.FileName} exited with {process.ExitCode}{NewLine}" +
                          $"out:{NewLine}{stdOut}{NewLine}error:{NewLine}{stdErr}";
                if (process.ExitCode == 0)
                {
                    Log.Debug(log);
                }
                else
                {
                    Log.Error(log);
                }
            }
            catch (Exception e)
            {
                Log.Error($"error during dotnet-suggest registration: {e}{NewLine}");
                throw;
            }

            return next(ctx);
        }
    }
}