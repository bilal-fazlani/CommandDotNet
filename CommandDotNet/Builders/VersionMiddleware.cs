using System.Threading.Tasks;
using CommandDotNet.ConsoleOnly;
using CommandDotNet.Execution;

namespace CommandDotNet.Builders
{
    internal static class VersionMiddleware
    {
        internal static readonly string VersionOptionName = Constants.VersionOptionName;

        internal static AppRunner UseVersionMiddleware(AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(DisplayVersionIfSpecified, MiddlewareSteps.Version);
                c.BuildEvents.OnCommandCreated += AddVersionOption;
            });
        }

        private static void AddVersionOption(BuildEvents.CommandCreatedEventArgs args)
        {
            if (!args.CommandBuilder.Command.IsRootCommand())
            {
                return;
            }

            if (args.CommandBuilder.Command.ContainsArgumentNode(VersionOptionName))
            {
                return;
            }

            var option = new Option(VersionOptionName, 'v', 
                TypeInfo.Flag, ArgumentArity.Zero, 
                definitionSource: typeof(VersionMiddleware).FullName)
            {
                Description = "Show version information",
                IsMiddlewareOption = true
            };

            args.CommandBuilder.AddArgument(option);
        }

        private static Task<int> DisplayVersionIfSpecified(CommandContext commandContext,
            ExecutionDelegate next)
        {
            if (commandContext.RootCommand!.HasInputValues(VersionOptionName))
            {
                Print(commandContext.Console);
                return ExitCodes.Success;
            }

            return next(commandContext);
        }

        private static void Print(IConsole console)
        {
            var appInfo = AppInfo.Instance;

            console.Out.WriteLine(appInfo.FileName);
            console.Out.WriteLine(appInfo.Version);
        }
    }
}