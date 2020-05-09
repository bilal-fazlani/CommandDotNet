using System;
using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.Tests
{
    public static class AppRunnerScenarioExtensions
    {
        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string args,
            Func<TestConsole, string>? onReadLine = null,
            IEnumerable<string>? pipedInput = null)
        {
            return runner.RunInMem(args, Ambient.WriteLine, onReadLine, pipedInput);
        }

        public static AppRunnerResult RunInMem(
            this AppRunner runner,
            string[] args,
            Func<TestConsole, string>? onReadLine = null,
            IEnumerable<string>? pipedInput = null)
        {
            return runner.RunInMem(args, Ambient.WriteLine, onReadLine, pipedInput);
        }

        public static AppRunnerResult Verify(this AppRunner appRunner, IScenario scenario)
        {
            // use Test.Default to force testing of TestConfig.GetDefaultFromSubClass()
            return appRunner.Verify(Ambient.WriteLine, TestConfig.Default, scenario);
        }
    }
}