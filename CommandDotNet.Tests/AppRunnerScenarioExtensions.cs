using System;
using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public static class AppRunnerScenarioExtensions
    {
        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string args,
            ITestOutputHelper output,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            return runner.RunInMem(args.SplitArgs(), output?.AsLogger(), onReadLine, pipedInput);
        }

        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string[] args,
            ITestOutputHelper output,
            Func<TestConsole, string> onReadLine = null,
            IEnumerable<string> pipedInput = null)
        {
            return runner.RunInMem(args, output?.AsLogger(), onReadLine, pipedInput);
        }

        public static AppRunnerResult VerifyScenario(this AppRunner appRunner, ITestOutputHelper output, IScenario scenario)
        {
            return appRunner.VerifyScenario(output.AsLogger(), scenario);
        }

        public static ILogger AsLogger(this ITestOutputHelper testOutputHelper)
        {
            return new Logger(testOutputHelper.WriteLine);
        }

    }
}