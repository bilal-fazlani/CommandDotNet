using System;
using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.Tokens;

namespace CommandDotNet.Example
{
    public class InteractiveSession
    {
        private readonly AppRunner _appRunner;
        private readonly string _appName;
        private readonly CommandContext _context;

        public InteractiveSession(AppRunner appRunner, string appName, CommandContext context)
        {
            _appRunner = appRunner;
            _appName = appName;
            _context = context;
        }

        public void Start()
        {
            var console = _context.Console;
            var cancellationToken = _context.CancellationToken;

            bool pressedCtrlC = false;
            Console.CancelKeyPress += (sender, args) =>
            {
                pressedCtrlC = true;
            };

            PrintSessionInit();

            bool pendingNewLine = false;
            void Write(string? value = null)
            {
                console!.Write(value);
                pendingNewLine = true;
            }

            void WriteLine(string? value = null)
            {
                console!.WriteLine(value);
                pendingNewLine = false;
            }

            void EnsureNewLine()
            {
                if (pendingNewLine)
                {
                    WriteLine();
                }
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                EnsureNewLine();
                Write(">>>");
                var input = console.In.ReadLine();
                if (input is null || pressedCtrlC)
                {
                    pressedCtrlC = false;
                    WriteLine();
                    return;
                }

                var args = new CommandLineStringSplitter().Split(input).ToArray();
                if (args.Length == 0)
                {
                    WriteLine();
                    continue;
                }
                if (args.Length == 1)
                {
                    var singleArg = args[0];
                    switch (singleArg)
                    {
                        case "exit":
                        case "quit":
                            return;
                        case "help":
                            PrintSessionHelp();
                            continue;
                    }
                    if (singleArg == Environment.NewLine)
                    {
                        WriteLine();
                        continue;
                    }
                }
                EnsureNewLine();
                _appRunner.Run(args);
            }
            EnsureNewLine();
        }

        private void PrintSessionInit()
        {
            var appInfo = AppInfo.GetAppInfo(_context);
            var console = _context.Console;
            console.WriteLine($"{_appName} {appInfo.Version}");
            console.WriteLine("Type 'help' to see interactive options");
            console.WriteLine("Type '-h' or '--help' to options for commands");
            console.WriteLine("Type 'exit', 'quit' or 'Ctrl+C' to exit.");
        }

        private void PrintSessionHelp()
        {
            var console = _context.Console;
            console.WriteLine("Type '-h' or '--help' to options for commands");
            console.WriteLine("Type 'exit', 'quit' or 'Ctrl+C' to exit.");
        }
    }
}