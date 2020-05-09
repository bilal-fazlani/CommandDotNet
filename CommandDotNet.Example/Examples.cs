using CommandDotNet.Example.Commands;

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
        private static bool _inSession;

        [DefaultMethod]
        public void StartSession(
            CommandContext context,
            InteractiveSession interactiveSession, 
            [Option(ShortName = "i")] bool interactive)
        {
            if (interactive && !_inSession)
            {
                context.Console.WriteLine("start session");
                _inSession = true;
                interactiveSession.Start();
            }
            else
            {
                context.Console.WriteLine($"no session {interactive} {_inSession}");
                context.ShowHelpOnExit = true;
            }
        }
        
        [SubCommand]
        public Git Git { get; set; } = null!;

        [SubCommand]
        public Math Math { get; set; } = null!;

        [SubCommand]
        public Models Models { get; set; } = null!;

        [SubCommand]
        public Pipes Pipes { get; set; } = null!;

        [SubCommand]
        public CancelMe CancelMe { get; set; } = null!;

        [SubCommand]
        public Commands.Prompts Prompts { get; set; } = null!;
    }
}
