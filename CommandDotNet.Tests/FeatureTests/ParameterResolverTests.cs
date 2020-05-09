using System.Threading;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Rendering;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ParameterResolverTests
    {
        public ParameterResolverTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }
        
        [Fact]
        public void ParameterServices_AreNotIncludedInBasicHelp()
        {
            new AppRunner<App>(TestAppSettings.BasicHelp)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Do [options] <intOperand>

Arguments:
  intOperand

Options:
  --stringOption
"
                    }
                });
        }

        [Fact]
        public void ParameterServices_AreNotIncludedInDetailedHelp()
        {
            new AppRunner<App>(TestAppSettings.DetailedHelp)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Do [options] <intOperand>

Arguments:

  intOperand  <NUMBER>

Options:

  --stringOption  <TEXT>
"
                    }
                });
        }

        [Fact]
        public void ParameterServices_ArePassedToCommandAndInterceptorMethod()
        {
            new AppRunner<App>()
                .Verify(new Scenario
            {
                When = {Args = "Do 7 --stringOption optValue"},
                Then =
                {
                    AssertContext = ctx =>
                    {
                        ctx.ParamValuesShouldBe(7, "optValue");
                        ctx.ParamValuesShouldBeEmpty<App>();

                        var invocation = ctx.GetCommandInvocationInfo();
                        invocation.ParameterValues![0].Should().BeOfType<CommandContext>().And.Should().NotBeNull();
                        invocation.ParameterValues[1].Should().BeAssignableTo<IConsole>().And.Should().NotBeNull();
                        invocation.ParameterValues[2].Should().BeOfType<CancellationToken>().And.Should().NotBeNull();

                        invocation = ctx.GetInterceptorInvocationInfo<App>();
                        invocation.ParameterValues![0].Should().BeOfType<InterceptorExecutionDelegate>().And.Should().NotBeNull();
                        invocation.ParameterValues[1].Should().BeOfType<CommandContext>().And.Should().NotBeNull();
                        invocation.ParameterValues[2].Should().BeAssignableTo<IConsole>().And.Should().NotBeNull();
                        invocation.ParameterValues[3].Should().BeOfType<CancellationToken>().And.Should().NotBeNull();
                    }
                }
            });
        }

        [Fact]
        public void ExternalParameterService_WhenNotRegistered_ResultContainsActionableErrorMessage()
        {
            new AppRunner<SomeServiceApp>()
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts =
                        {
                            "CommandDotNet.Tests.FeatureTests.ParameterResolverTests+SomeService is not supported.",
                            "If it is a service and not an argument, register using AppRunner.Configure(b => b.UseParameterResolver(ctx => ...));"
                        }
                    }
                });
        }

        [Fact]
        public void ExternalParameterService_CanBeRegistered()
        {
            var someSvc = new SomeService();
            new AppRunner<SomeServiceApp>()
                .Configure(b => b.UseParameterResolver(ctx => someSvc))
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            var invocation = ctx.GetCommandInvocationInfo();
                            invocation.ParameterValues![0].Should().Be(someSvc);
                        }
                    }
                });
        }

        public class SomeServiceApp
        {
            public void Do(SomeService someService, [Operand] int intOperand, [Option] string? stringOption = null)
            {
            }
        }

        public class App
        {
            public Task<int> Interceptor(InterceptorExecutionDelegate next, CommandContext commandContext, IConsole console, CancellationToken cancellationToken)
            {
                return next();
            }

            public void Do(CommandContext commandContext, IConsole console, CancellationToken cancellationToken, [Operand] int intOperand, [Option] string? stringOption = null)
            {
            }
        }

        public class SomeService
        {

        }
    }
}
