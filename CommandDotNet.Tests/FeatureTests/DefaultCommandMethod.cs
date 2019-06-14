using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class DefaultCommandMethod : TestBase
    {
        private static AppSettings BasicHelp = new AppSettings { Help = { TextStyle = HelpTextStyle.Basic }, EnableVersionOption = false };
        private static AppSettings DetailedHelp = new AppSettings { Help = { TextStyle = HelpTextStyle.Detailed }, EnableVersionOption = false };
        
        public DefaultCommandMethod(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void WithoutParams_BasicHelp_IncludesOtherCommands()
        {
            Verify(new Given<WithoutParamsApp>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "-h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll [options] [command]

Options:
  -h | --help  Show help information

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void WithoutParams_DetailedHelp_IncludesOtherCommands()
        {
            Verify(new Given<WithoutParamsApp>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "-h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll [options] [command]

Options:

  -h | --help
  Show help information


Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void WithParams_BasicHelp_IncludesArgsOtherCommands()
        {
            Verify(new Given<WithParamsApp>
            {
                And = {AppSettings = BasicHelp},
                WhenArgs = "-h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll [arguments] [options] [command]

Arguments:
  text  some text

Options:
  -h | --help  Show help information

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void WithParams_DetailedHelp_IncludesArgsOtherCommands()
        {
            Verify(new Given<WithParamsApp>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "-h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll [arguments] [options] [command]

Arguments:

  text    <TEXT>
  some text


Options:

  -h | --help
  Show help information


Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void WithoutParams_Execute_works()
        {
            Verify(new Given<WithoutParamsApp>
            {
                WhenArgs = "",
                Then =
                {
                    Outputs = { WithoutParamsApp.DefaultMethodExecuted }
                }
            });
        }

        [Fact]
        public void WithParams_Execute_works()
        {
            Verify(new Given<WithParamsApp>
            {
                WhenArgs = "abcde",
                Then =
                {
                    Outputs = { "abcde" }
                }
            });
        }

        public class WithoutParamsApp
        {
            public const string DefaultMethodExecuted = "default executed";

            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            [DefaultMethod]
            public void DefaultMethod()
            {
                TestOutputs.Capture(DefaultMethodExecuted);
            }

            public void AnotherCommand()
            {
                TestOutputs.Capture(false);
            }
        }

        public class WithParamsApp
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            [DefaultMethod]
            public void DefaultMethod(
                [Argument(Description = "some text")]
                string text)
            {
                TestOutputs.Capture(text);
            }

            public void AnotherCommand()
            {
                TestOutputs.Capture(false);
            }
        }
    }
}