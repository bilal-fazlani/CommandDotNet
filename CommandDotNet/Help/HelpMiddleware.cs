using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Execution;

namespace CommandDotNet.Help
{
    internal static class HelpMiddleware
    {
        internal static AppRunner UseHelpMiddleware(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(CheckIfShouldShowHelp, MiddlewareSteps.Help.CheckIfShouldShowHelp);
                c.UseMiddleware(PrintHelp, MiddlewareSteps.Help.PrintHelpOnExit);
                c.BuildEvents.OnCommandCreated += AddHelpOption;
            });
        }

        private static void AddHelpOption(BuildEvents.CommandCreatedEventArgs args)
        {
            if (args.CommandBuilder.Command.ContainsArgumentNode(Constants.HelpOptionName))
            {
                return;
            }

            var appSettingsHelp = args.CommandContext.AppConfig.AppSettings.Help;

            var option = new Option(Constants.HelpOptionName, 'h',
                TypeInfo.Flag, ArgumentArity.Zero,
                aliases: new[] { "?" },
                definitionSource: typeof(HelpMiddleware).FullName)
            {
                Description = "Show help information",
                IsMiddlewareOption = true,
                Hidden = !appSettingsHelp.PrintHelpOption
            };

            args.CommandBuilder.AddArgument(option);
        }

        private static Task<int> CheckIfShouldShowHelp(CommandContext ctx, ExecutionDelegate next)
        {
            var parseResult = ctx.ParseResult!;
            var targetCommand = parseResult.TargetCommand;

            if (parseResult.ParseError != null)
            {
                var console = ctx.Console;
                console.Error.WriteLine(parseResult.ParseError.Message);
                console.Error.WriteLine();
                ctx.ShowHelpOnExit = true;
                return ExitCodes.Error;
            }

            if (parseResult.HelpWasRequested())
            {
                ctx.ShowHelpOnExit = true;
                return ExitCodes.Success;
            }

            if (!targetCommand.IsExecutable)
            {
                ctx.ShowHelpOnExit = true;
                return ExitCodes.Success;
            }

            return next(ctx);
        }

        private static Task<int> PrintHelp(CommandContext ctx, ExecutionDelegate next)
        {
            var result = next(ctx);

            if (ctx.ShowHelpOnExit)
            {
                ctx.PrintHelp();
            }

            return result;
        }
    }
}