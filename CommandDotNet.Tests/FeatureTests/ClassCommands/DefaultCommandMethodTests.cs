using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class DefaultCommandMethodTests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;
        
        public DefaultCommandMethodTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void WithoutDefaultArgs_BasicHelp_IncludesOtherCommands()
        {
            new AppRunner<WithoutDefaultArgsApp>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll [command]

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                }
            });
        }

        [Fact]
        public void WithoutDefaultArgs_DetailedHelp_IncludesOtherCommands()
        {
            new AppRunner<WithoutDefaultArgsApp>().Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll [command]

Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_BasicHelp_IncludesArgsAndOtherCommands()
        {
            new AppRunner<WithDefaultArgsApp>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll [command] <text>

Arguments:
  text  some text

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_DetailedHelp_IncludesArgsAndOtherCommands()
        {
            new AppRunner<WithDefaultArgsApp>().Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll [command] <text>

Arguments:

  text  <TEXT>
  some text

Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                }
            });
        }


        [Fact]
        public void WithDefaultArgs_Help_ForAnotherCommand_DoesNotIncludeDefaultArgs()
        {
            new AppRunner<WithDefaultArgsApp>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "AnotherCommand -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll AnotherCommand
"
                }
            });
        }

        [Fact]
        public void WithoutDefaultArgs_Execute_works()
        {
            new AppRunner<WithoutDefaultArgsApp>().Verify(new Scenario
            {
                When = {Args = null},
                Then = {AssertContext = ctx => ctx.GetCommandInvocationInfo()
                    .MethodInfo!.Name.Should().Be(nameof(WithoutDefaultArgsApp.DefaultMethod))}
            });
        }

        [Fact]
        public void WithDefaultArgs_Execute_works()
        {
            new AppRunner<WithDefaultArgsApp>().Verify(new Scenario
            {
                When = {Args = "abcde"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe("abcde")}
            });
        }

        [Fact]
        public void WithDefaultArgs_Execute_AnotherCommand_WorksWithoutParams()
        {
            new AppRunner<WithDefaultArgsApp>().Verify(new Scenario
            {
                When = {Args = "AnotherCommand"},
                Then = { AssertContext = ctx => ctx.ParamValuesShouldBeEmpty() }
            });
        }

        [Fact]
        public void WithDefaultArgs_Execute_AnotherCommand_FailsWithDefaultParams()
        {
            new AppRunner<WithDefaultArgsApp>().Verify(new Scenario
            {
                When = {Args = "AnotherCommand abcde"},
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = { "Unrecognized command or argument 'abcde'"}
                }
            });
        }

        private class WithoutDefaultArgsApp
        {
            [DefaultMethod]
            public void DefaultMethod()
            {
            }

            public void AnotherCommand()
            {
            }
        }

        private class WithDefaultArgsApp
        {
            [DefaultMethod]
            public void DefaultMethod(
                [Operand(Description = "some text")]
                string text)
            {
            }

            public void AnotherCommand()
            {
            }
        }
    }
}
