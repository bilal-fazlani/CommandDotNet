using System.Threading.Tasks;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class InterceptorExecutionWithDefaultMethodTests
    {
        public InterceptorExecutionWithDefaultMethodTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_Help4Parent_NoImpact()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .Verify(new Scenario
                {
                    When = {Args = "-h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll [command] <defaultArg>

Arguments:

  defaultArg  <NUMBER>

Commands:

  Do

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_Help4Child_NoImpact()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Do <arg1>

Arguments:

  arg1  <NUMBER>
"
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_Help4Parent_ContainsInterceptorOptions()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .Verify(new Scenario
                {
                    When = {Args = "-h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll [command] [options] <defaultArg>

Arguments:

  defaultArg  <NUMBER>

Options:

  --stringOpt  <TEXT>

Options also available for subcommands:

  --skipCmd

  --useReturnCode  <NUMBER>

Commands:

  Do

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_Help4Child_NoImpact()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Do [options] <arg1>

Arguments:

  arg1  <NUMBER>

Options:

  --stringOpt  <TEXT>
"
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_IsDetectedAndUsed()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .TrackingInvocations()
                .Verify(new Scenario
                {
                    When = {Args = "Do 1"},
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            ctx.GetInterceptorInvocationInfo<AppWithNoInterceptorOptions>().WasInvoked.Should().BeTrue();
                            ctx.ParamValuesShouldBeEmpty<AppWithNoInterceptorOptions>();
                            var command = ctx.GetCommandInvocationInfo();
                            command.WasInvoked.Should().BeTrue();
                            command.MethodInfo!.Name.Should().Be(nameof(AppWithNoInterceptorOptions.Do));
                            ctx.ParamValuesShouldBe(1);
                        }
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_AndDefaultMethod_IsDetectedAndUsed()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .TrackingInvocations()
                .Verify(new Scenario
                {
                    When = {Args = "1"},
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            ctx.GetInterceptorInvocationInfo<AppWithNoInterceptorOptions>().WasInvoked.Should().BeTrue();
                            ctx.ParamValuesShouldBeEmpty<AppWithNoInterceptorOptions>();
                            var command = ctx.GetCommandInvocationInfo();
                            command.WasInvoked.Should().BeTrue();
                            command.MethodInfo!.Name.Should().Be(nameof(AppWithNoInterceptorOptions.Default));
                            ctx.ParamValuesShouldBe(1);
                        }
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_IsDetectedAndUsed()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .TrackingInvocations()
                .Verify(new Scenario
                {
                    When = {Args = "--stringOpt lala Do 1"},
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            ctx.GetInterceptorInvocationInfo<AppWithInteceptorOptions>().WasInvoked.Should().BeTrue();
                            ctx.ParamValuesShouldBe<AppWithInteceptorOptions>(new InterceptOptions{stringOpt = "lala"});
                            var command = ctx.GetCommandInvocationInfo();
                            command.WasInvoked.Should().BeTrue();
                            command.MethodInfo!.Name.Should().Be(nameof(AppWithInteceptorOptions.Do));
                            ctx.ParamValuesShouldBe(1);
                        }
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_AndDefaultMethod_IsDetectedAndUsed()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .TrackingInvocations()
                .Verify(new Scenario
                {
                    When = {Args = "--stringOpt lala 1"},
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            ctx.GetInterceptorInvocationInfo<AppWithInteceptorOptions>().WasInvoked.Should().BeTrue();
                            ctx.ParamValuesShouldBe<AppWithInteceptorOptions>(new InterceptOptions{stringOpt = "lala"});
                            var command = ctx.GetCommandInvocationInfo();
                            command.WasInvoked.Should().BeTrue();
                            command.MethodInfo!.Name.Should().Be(nameof(AppWithInteceptorOptions.Default));
                            ctx.ParamValuesShouldBe(1);
                        }
                    }
                });
        }

        class AppWithNoInterceptorOptions
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next)
            {
                return next();
            }

            [DefaultMethod]
            public void Default(int defaultArg)
            {
            }

            public void Do(int arg1)
            {
            }
        }

        class AppWithInteceptorOptions
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next,
                InterceptOptions interceptOptions)
            {
                if (interceptOptions.skipCmd)
                {
                    return ExitCodes.Success;
                }

                var returnCode = next();
                return interceptOptions.useReturnCode.HasValue
                    ? Task.FromResult(interceptOptions.useReturnCode.Value)
                    : returnCode;
            }

            [DefaultMethod]
            public void Default(int defaultArg)
            {
            }

            public void Do(int arg1)
            {
            }
        }

        public class InterceptOptions : IArgumentModel
        {
            [Option(AssignToExecutableSubcommands = true)]
            public string? stringOpt { get; set; }
            public bool skipCmd { get; set; } = false;
            public int? useReturnCode { get; set; }
        }
    }
}
