using System;
using System.Linq;
using System.Text;

namespace CommandDotNet.TestTools.Scenarios
{
    public static class AppRunnerScenarioExtensions
    {
        /// <summary>Run and verify the scenario expectations, output results to <see cref="Console"/></summary>
        public static AppRunnerResult Verify(this AppRunner appRunner, IScenario scenario, 
            Action<string?>? logLine = null, TestConfig? config = null)
        {
            return appRunner.Verify(logLine, config, scenario);
        }

        /// <summary>Run and verify the scenario expectations using the given logger for output.</summary>
        public static AppRunnerResult Verify(this AppRunner appRunner, Action<string?>? logLine, TestConfig? config, IScenario scenario)
        {
            if (scenario.When.Args != null && scenario.When.ArgsArray != null)
            {
                throw new InvalidOperationException($"Both {nameof(scenario.When)}.{nameof(scenario.When.Args)} and " +
                                                    $"{nameof(scenario.When)}.{nameof(scenario.When.ArgsArray)} were specified. " +
                                                    "Only one can be specified.");
            }

            logLine ??= Console.WriteLine;
            config ??= TestConfig.Default;

            var args = scenario.When.ArgsArray ?? scenario.When.Args!.SplitArgs();

            var origOnSuccess = config.OnSuccess;
            if (!config.OnError.CaptureAndReturnResult)
            {
                config = config.Where(c =>
                {
                    // silence success in RunInMem so results are not printed
                    // twice when asserts fail below.
                    // success will be replaced and printed again.
                    c.OnSuccess = TestConfig.Silent.OnSuccess;
                    c.OnError.CaptureAndReturnResult = true;
                });
            }
            var results = appRunner.RunInMem(
                args,
                logLine,
                scenario.When.OnReadLine,
                scenario.When.PipedInput,
                scenario.When.OnPrompt,
                config);

            config.OnSuccess = origOnSuccess;

            try
            {
                AssertExitCodeAndErrorMessage(scenario, results);

                scenario.Then.AssertOutput?.Invoke(results.Console.AllText());
                scenario.Then.AssertContext?.Invoke(results.CommandContext);

                if (scenario.Then.Output != null)
                {
                    results.Console.AllText().ShouldBe(scenario.Then.Output, "output");
                }

                results.LogResult(logLine);
            }
            catch (Exception)
            {
                results.LogResult(logLine, onError: true);
                throw;
            }

            return results;
        }

        private static void AssertExitCodeAndErrorMessage(IScenario scenario, AppRunnerResult result)
        {
            var sb = new StringBuilder();
            AssertExitCode(scenario, result, sb);
            AssertMissingOutputTexts(scenario, result, sb);
            AssertUnexpectedOutputTexts(scenario, result, sb);
            if (sb.Length > 0)
            {
                throw new AssertFailedException(sb.ToString());
            }
        }

        private static void AssertExitCode(IScenario scenario, AppRunnerResult result, StringBuilder sb)
        {
            var expectedExitCode = scenario.Then.ExitCode.GetValueOrDefault();
            if (expectedExitCode != result.ExitCode)
            {
                sb.AppendLine($"ExitCode: expected={expectedExitCode} actual={result.ExitCode}");
            }
        }

        private static void AssertMissingOutputTexts(IScenario scenario, AppRunnerResult result, StringBuilder sb)
        {
            var missingHelpTexts = scenario.Then.OutputContainsTexts
                .Where(t => !result.Console.AllText().Contains(t))
                .ToList();
            if (missingHelpTexts.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Missing text in output:");
                foreach (var text in missingHelpTexts)
                {
                    sb.AppendLine();
                    sb.AppendLine($"  {text}");
                }
            }
        }

        private static void AssertUnexpectedOutputTexts(IScenario scenario, AppRunnerResult result, StringBuilder sb)
        {
            var unexpectedHelpTexts = scenario.Then.OutputNotContainsTexts
                .Where(result.Console.AllText().Contains)
                .ToList();
            if (unexpectedHelpTexts.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Unexpected text in output:");
                foreach (var text in unexpectedHelpTexts)
                {
                    sb.AppendLine();
                    sb.AppendLine($"  {text}");
                }
            }
        }
    }
}