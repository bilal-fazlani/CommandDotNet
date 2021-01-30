using System;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class CommandTemplatesTests
    {
        private const string AppName = "dotnet testhost.dll";

        public CommandTemplatesTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [InlineData("%AppName% lala", "-h", "Usage: " + AppName + " lala")]
        [InlineData("%CmdPath% lala", "-h", "Usage:  lala")]
        [InlineData("%CmdPath% lala", "L1 -h", "Usage: L1 lala")]
        [InlineData("%CmdPath% lala", "L1 L2 -h", "Usage: L1 L2 lala")]
        [InlineData("%AppName% %CmdPath% lala", "L1 L2 -h", "Usage: " + AppName + " L1 L2 lala")]
        [Theory]
        public void InUsage(string usage, string input, string contains)
        {
            Run(usage, input, contains, cmd => { cmd.Usage = usage; });
        }

        [InlineData("%AppName% lala", "-h", AppName + " lala")]
        [InlineData("%CmdPath% lala", "-h", " lala")]
        [InlineData("%CmdPath% lala", "L1 -h", "L1 lala")]
        [InlineData("%CmdPath% lala", "L1 L2 -h", "L1 L2 lala")]
        [InlineData("%AppName% %CmdPath% lala", "L1 L2 -h", AppName + " L1 L2 lala")]
        [Theory]
        public void InDescription(string usage, string input, string contains)
        {
            Run(usage, input, contains, cmd => { cmd.Description = usage; });
        }

        [InlineData("%AppName% lala", "-h", AppName + " lala")]
        [InlineData("%CmdPath% lala", "-h", " lala")]
        [InlineData("%CmdPath% lala", "L1 -h", "L1 lala")]
        [InlineData("%CmdPath% lala", "L1 L2 -h", "L1 L2 lala")]
        [InlineData("%AppName% %CmdPath% lala", "L1 L2 -h", AppName + " L1 L2 lala")]
        [Theory]
        public void InExtendedHelpText(string usage, string input, string contains)
        {
            Run(usage, input, contains, cmd => { cmd.ExtendedHelpText = usage; });
        }

        private static void Run(string usage, string input, string contains, Action<Command> onCommandCreated)
        {
            new AppRunner<TemplateApp>()
                .OnCommandCreated(onCommandCreated)
                .Verify(new Scenario
                {
                    When = {Args = input},
                    Then =
                    {
                        OutputContainsTexts = {contains}
                    }
                });
        }

        private class TemplateApp
        {
            [SubCommand]
            public class L1
            {
                [SubCommand]
                public class L2
                {

                }
            }
        }
    }
}