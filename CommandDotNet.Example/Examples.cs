using CommandDotNet.Example.Commands;
using CommandDotNet.Example.DocExamples;
using Git = CommandDotNet.Example.Commands.Git;

namespace CommandDotNet.Example
{
    [Command(
        ExtendedHelpText = "Directives:\n" +
                           "  [debug] to attach a debugger to the app\n" +
                           "  [parse] to output how the inputs were tokenized\n" +
                           "  [log] to output framework logs to the console\n" +
                           "  [log:debug|info|warn|error|fatal] to output framework logs for the given level or above\n" +
                           "\n" +
                           "directives must be specified before any commands and arguments.\n" +
                           "\n" +
                           "Example: %UsageAppName% [debug] [parse] [log:info] cancel-me")]
    internal class Examples
    {
        [SubCommand]
        public Git Git { get; set; }

        [SubCommand]
        public Math Math { get; set; }

        [SubCommand]
        public Models Models { get; set; }

        [SubCommand]
        public Pipes Pipes { get; set; }

        [SubCommand]
        public CancelMe CancelMe { get; set; }

        [SubCommand]
        public Commands.Prompts Prompts { get; set; }

        [SubCommand]
        public RocketLauncher RocketLauncher { get; set; }
    }
}
