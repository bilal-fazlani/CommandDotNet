using System;
using CommandDotNet.ConsoleOnly;

namespace CommandDotNet.TestTools
{
    public class AppRunnerResult
    {
        internal AppRunner Runner { get; }
        internal TestConfig Config { get; }

        public int ExitCode { get; }

        public TestConsole Console { get; }

        /// <summary>The <see cref="CommandContext"/> used during the run</summary>
        public CommandContext CommandContext { get; }

        /// <summary>The exception that escaped from <see cref="AppRunner.Run"/><br/></summary>
        public Exception? EscapedException { get; }

        public AppRunnerResult(int exitCode, AppRunner runner,
            CommandContext commandContext, TestConsole testConsole,
            TestConfig config, Exception? escapedException = null)
        {
            ExitCode = exitCode;
            Runner = runner;
            CommandContext = commandContext;
            Console = testConsole;
            Config = config;
            EscapedException = escapedException;
        }
    }
}