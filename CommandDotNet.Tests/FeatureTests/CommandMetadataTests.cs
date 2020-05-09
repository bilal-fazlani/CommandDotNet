using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CommandMetadataTests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public CommandMetadataTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void App_BasicHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    Output = @"app description

Usage: some usage examples

Commands:
  somecommand  cmd description
  SubApp       sub-app description

Use ""dotnet testhost.dll [command] --help"" for more information about a command.

app extended help
"
                }
            });
        }

        [Fact]
        public void App_DetailedHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    Output = @"app description

Usage: some usage examples

Commands:

  somecommand  cmd description
  SubApp       sub-app description

Use ""dotnet testhost.dll [command] --help"" for more information about a command.

app extended help
"
                }
            });
        }

        [Fact]
        public void NestedApp_BasicHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "SubApp -h"},
                Then =
                {
                    Output = @"sub-app description

Usage: dotnet testhost.dll SubApp [command]

Commands:
  subdo

Use ""dotnet testhost.dll SubApp [command] --help"" for more information about a command.

sub-app extended help
"
                }
            });
        }

        [Fact]
        public void NestedApp_DetailedHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "SubApp -h"},
                Then =
                {
                    Output = @"sub-app description

Usage: dotnet testhost.dll SubApp [command]

Commands:

  subdo

Use ""dotnet testhost.dll SubApp [command] --help"" for more information about a command.

sub-app extended help
"
                }
            });
        }

        [Fact]
        public void Command_BasicHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "somecommand -h"},
                Then =
                {
                    Output = @"cmd description

Usage: dotnet testhost.dll somecommand <value>

Arguments:
  value

cmd extended help
"
                }
            });
        }

        [Fact]
        public void Command_DetailedHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "somecommand -h"},
                Then =
                {
                    Output = @"cmd description

Usage: dotnet testhost.dll somecommand <value>

Arguments:

  value  <NUMBER>

cmd extended help
"
                }
            });
        }

        [Fact]
        public void Command_Exec_UsesNameFromCommandAttrData()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "somecommand 5"},
                Then = { ExitCode = 5 }
            });
        }

        [Fact]
        public void NestedApp_Command_Exec_UsesNameFromCommandAttrData()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "SubApp subdo 5"},
                Then = { ExitCode = 5 }
            });
        }

        // sanity check for ApplicationMetadata until it has been removed 
        [Command(
            Description = "app description",
            Usage = "some usage examples",
            Name = "SomeApp",
            ExtendedHelpText = "app extended help")]
        private class App
        {
            [Command(
                Description = "cmd description",
                Name = "somecommand",
                ExtendedHelpText = "cmd extended help")]
            public int Do(int value)
            {
                return value;
            }

            [SubCommand]
            [Command(
                Description = "sub-app description",
                Name = "SubApp",
                ExtendedHelpText = "sub-app extended help")]
            public class SubApp
            {
                [Command(Name = "subdo")]
                public int Do(int value)
                {
                    return value;
                }
            }
        }
    }
}
