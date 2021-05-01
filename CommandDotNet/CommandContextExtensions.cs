using CommandDotNet.ConsoleOnly;

namespace CommandDotNet
{
    public static class CommandContextExtensions
    {
        /// <summary>
        /// Prints help for the provided command if provided,
        /// else the target command if exists,
        /// else the root command.
        /// </summary>
        public static void PrintHelp(this CommandContext commandContext, Command? command = null)
        {
            command ??= commandContext.ParseResult?.TargetCommand ?? commandContext.RootCommand!;
            var helpText = commandContext.AppConfig.HelpProvider.GetHelpText(command);
            commandContext.Console.Out.WriteLine(helpText);
        }
    }
}