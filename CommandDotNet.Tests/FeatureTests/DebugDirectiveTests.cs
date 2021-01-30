using System.Diagnostics;
using CommandDotNet.Diagnostics;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class DebugDirectiveTests
    {
        public DebugDirectiveTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
            // skip waiting for debugger to connect
            DebugDirective.InTestHarness = true;
        }

        [Fact]
        public void Directives_CanBeDisabled()
        {
            new AppRunner<App>(new AppSettings {DisableDirectives = true})
                .Verify(
                    new Scenario
                    {
                        When = {Args = "[debug] Do"},
                        Then =
                        {
                            ExitCode = 1, // method should have been called
                            OutputContainsTexts = { "Unrecognized command or argument '[debug]'" }
                        }
                    });
        }

        [Fact]
        public void DebugDirective_PrintsProcessId_And_WaitsForDebuggerToStart()
        {
            var processId = Process.GetCurrentProcess().Id;
            new AppRunner<App>()
                .UseDebugDirective()
                .Verify(
                    new Scenario
                    {
                        When = {Args = "[debug] Do"},
                        Then =
                        {
                            ExitCode = 5, // method should have been called
                            OutputContainsTexts = {$"Attach your debugger to process {processId} (dotnet)."}
                        }
                    });
        }

        private class App
        {
            public int Do()
            {
                return 5;
            }
        }
    }
}
