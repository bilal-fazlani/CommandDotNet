using System.Threading.Tasks;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class InterceptorExecutionWithInheritedOptionsTests
    {
        public InterceptorExecutionWithInheritedOptionsTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void DeclaringCommands_InheritedOptions_NotShown_InHelp()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "-h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll [command] [options]

Options:

  --interceptorOpt  <TEXT>

Commands:

  ChildApp
  Do

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                    }
                });
        }

        [Fact]
        public void DeclaringCommands_InheritedOptions_NotAccepted()
        {
            // TODO: Does this really make sense?  Should inherited options be specified in either location?  It seems confusing. 
            //       What's the purpose of this feature?
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "--interceptorOpt lala --inheritedOpt fishies Do --opt1 5 10"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { "Unrecognized option '--inheritedOpt'" }
                    }
                });
        }

        [Fact]
        public void ExecutableLocalSubcommands_InheritedOptions_AreShown_InHelp()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Do [options] <arg1>

Arguments:

  arg1  <NUMBER>

Options:

  --opt1          <NUMBER>

  --inheritedOpt  <TEXT>
"
                    }
                });
        }

        [Fact]
        public void ExecutableLocalSubcommands_InheritedOptions_AreAccepted()
        {
            new AppRunner<App>()
                .TrackingInvocations()
                .Verify(new Scenario
                {
                    When = {Args = "--interceptorOpt lala Do --inheritedOpt fishies --opt1 5 10"},
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            ctx.GetInterceptorInvocationInfo<App>().WasInvoked.Should().BeTrue();
                            ctx.ParamValuesShouldBe<App>("lala", "fishies");
                            ctx.GetCommandInvocationInfo().WasInvoked.Should().BeTrue();
                            ctx.ParamValuesShouldBe(10,5);
                        }
                    }
                });
        }

        [Fact]
        public void ExecutableNestedSubcommands_InheritedOptions_AreShown_InHelp()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "ChildApp Do -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll ChildApp Do [options]

Options:

  --inheritedOpt  <TEXT>
"
                    }
                });
        }

        [Fact]
        public void ExecutableNestedSubcommands_InheritedOptions_AreAccepted()
        {
            new AppRunner<App>()
                .TrackingInvocations()
                .Verify(new Scenario
                {
                    When = {Args = "--interceptorOpt lala ChildApp Do --inheritedOpt fishies"},
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            ctx.GetInterceptorInvocationInfo<App>().WasInvoked.Should().BeTrue();
                            ctx.ParamValuesShouldBe<App>("lala", "fishies");
                            ctx.GetCommandInvocationInfo().WasInvoked.Should().BeTrue();
                            ctx.ParamValuesShouldBeEmpty();
                        }
                    }
                });
        }

        [Fact]
        public void NonExecutableSubcommands_InheritedOptions_NotShown_InHelp()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "ChildApp -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll ChildApp [command]

Commands:

  Do

Use ""dotnet testhost.dll ChildApp [command] --help"" for more information about a command.
"
                    }
                });
        }

        [Fact]
        public void NonExecutableSubcommands_InheritedOptions_NotAccepted()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "--interceptorOpt lala ChildApp --inheritedOpt fishies"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { "Unrecognized option '--inheritedOpt'" }
                    }
                });
        }

        class App
        {
            [SubCommand]
            public ChildApp ChildApp { get; set; } = null!;

            public Task<int> Intercept(InterceptorExecutionDelegate next,
                string interceptorOpt,
                [Option(AssignToExecutableSubcommands = true)] string inheritedOpt)
            {
                return next();
            }

            public void Do(int arg1, [Option]int opt1)
            {
            }

            public class InterceptResult
            {
                public string? InterceptorOpt { get; set; }
                public string? InheritedOpt { get; set; }
            }

            public class DoResult
            {
                public int Arg1 { get; set; }
                public int Opt1 { get; set; }
            }
        }

        class ChildApp
        {
            public void Do()
            {
            }

            public class DoResult
            {
                public bool Executed { get; set; }
            }
        }
    }
}
