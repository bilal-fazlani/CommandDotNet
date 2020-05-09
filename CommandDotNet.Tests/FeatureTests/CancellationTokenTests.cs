using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CancellationTokenTests
    {
        public CancellationTokenTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }
        
        [Fact]
        public void Console_CancelKeyPress_ShouldStop_MiddlewarePipeline()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Configure(c =>
                {
                    c.UseMiddleware(MockCtrlC, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(ShouldNotReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                })
                .Verify(new Scenario
                {
                    When = { Args = "Do" },
                    Then = { ExitCode = 0 }
                });
        }

        [Fact]
        public void Console_CancelKeyPress_Should_ApplyOnlyToNestedCommandContext()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Verify(new Scenario
                {
                    When = { Args = nameof(App.NestMockCtrlC) },
                    Then =
                    {
                        ExitCode = 0,
                        AssertContext = ctx => ctx.CancellationToken.IsCancellationRequested.Should().BeFalse()
                    }
                });
        }

        [Fact]
        public void Console_CancelKeyPress_Disabled_During_IgnoreCtrlC()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Configure(c =>
                {
                    c.UseMiddleware(IgnoreCtrlC, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(MockCtrlC, MiddlewareStages.PostTokenizePreParseInput);
                })
                .Verify(new Scenario
                {
                    When = { Args = "Do" },
                    Then = { ExitCode = 5 }
                });
        }

        [Fact]
        public void Console_CancelKeyPress_Enabled_After_IgnoreCtrlC()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Configure(c =>
                {
                    c.UseMiddleware(IgnoreCtrlC, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(MockCtrlC, MiddlewareStages.PostTokenizePreParseInput, allowMultipleRegistrations:true);
                    c.UseMiddleware(ShouldReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(UnignoreCtrlC, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(MockCtrlC, MiddlewareStages.PostTokenizePreParseInput, allowMultipleRegistrations:true);
                    c.UseMiddleware(ShouldNotReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                })
                .Verify(new Scenario
                {
                    When = { Args = "Do" },
                    Then =
                    {
                        ExitCode = 0,
                        Output = @"reached expected step"
                    }
                });
        }

        [Fact]
        public void CurrentDomain_ProcessExit_ShouldStop_MiddlewarePipeline()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Configure(c =>
                {
                    c.UseMiddleware(MockCurrentDomainProcessExit, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(ShouldNotReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                })
                .Verify(new Scenario
                {
                    When = { Args = "Do" },
                    Then = { ExitCode = 0 }
                });
        }

        [Fact]
        public void CurrentDomain_ProcessExit_Ignores_IgnoreCtrlC()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Configure(c =>
                {
                    c.UseMiddleware(IgnoreCtrlC, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(MockCurrentDomainProcessExit, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(ShouldNotReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                })
                .Verify(new Scenario
                {
                    When = { Args = "Do" },
                    Then = { ExitCode = 0 }
                });
        }

        [Fact]
        public void CurrentDomain_ProcessExit_Should_CancelAllTokens()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Verify(new Scenario
                {
                    When = { Args = nameof(App.NestMockCurrentDomainProcessExit) },
                    Then =
                    {
                        ExitCode = 0,
                        AssertContext = ctx => ctx.CancellationToken.IsCancellationRequested.Should().BeTrue()
                    }
                });
        }

        [Fact]
        public void CurrentDomain_UnhandledException_NotTerminating_ShouldNotStop_MiddlewarePipeline()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Configure(c =>
                {
                    c.UseMiddleware(MockCurrentDomainUnhandledExceptionNonTerminating, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(ShouldReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                })
                .Verify(new Scenario
                {
                    When = { Args = "Do" },
                    Then =
                    {
                        ExitCode = 5,
                        Output = @"reached expected step"
                    }
                });
        }

        [Fact]
        public void CurrentDomain_UnhandledException_Terminating_ShouldStop_MiddlewarePipeline()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Configure(c =>
                {
                    c.UseMiddleware(MockCurrentDomainUnhandledExceptionTerminating, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(ShouldNotReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                })
                .Verify(new Scenario
                {
                    When = { Args = "Do" },
                    Then = { ExitCode = 0 }
                });
        }

        [Fact]
        public void CurrentDomain_UnhandledException_Terminating_Ignores_IgnoreCtrlC()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Configure(c =>
                {
                    c.UseMiddleware(IgnoreCtrlC, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(MockCurrentDomainUnhandledExceptionTerminating, MiddlewareStages.PostTokenizePreParseInput);
                    c.UseMiddleware(ShouldNotReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                })
                .Verify(new Scenario
                {
                    When = { Args = "Do" },
                    Then = { ExitCode = 0 }
                });
        }

        [Fact]
        public void CurrentDomain_UnhandledException_Should_CancelAllTokens()
        {
            new AppRunner<App>()
                .UseCancellationHandlers()
                .Verify(new Scenario
                {
                    When = { Args = nameof(App.NestMockCurrentDomainUnhandledExceptionTerminating) },
                    Then =
                    {
                        ExitCode = 0,
                        AssertContext = ctx => ctx.CancellationToken.IsCancellationRequested.Should().BeTrue()
                    }
                });
        }

        [Fact]
        public void ShouldNotReachThisStep_ShouldFailTheTests()
        {
            // this test ensures the previous test is not passing with a false positive
            var appRunner = new AppRunner<App>()
                .UseCancellationHandlers()
                .Configure(c =>
                {
                    c.UseMiddleware(ShouldNotReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                });

            Assert.Throws<Exception>(() => appRunner.RunInMem("Do"))
                .Message.Should().Be("This middleware should not have been called");
        }

        private static Task<int> ShouldNotReachThisStep(CommandContext context, ExecutionDelegate next)
        {
            throw new Exception("This middleware should not have been called");
        }

        private static Task<int> ShouldReachThisStep(CommandContext context, ExecutionDelegate next)
        {
            context.Console.Write("reached expected step");
            return next(context);
        }

        private static Task<int> MockCtrlC(CommandContext context, ExecutionDelegate next)
        {
            CancellationHandlers.TestAccess.Console_CancelKeyPress();
            return next(context);
        }

        private static Task<int> IgnoreCtrlC(CommandContext context, ExecutionDelegate next)
        {
            context.Services.Add(CancellationHandlers.IgnoreCtrlC());
            return next(context);
        }

        private static Task<int> UnignoreCtrlC(CommandContext context, ExecutionDelegate next)
        {
            context.Services.GetOrThrow<IDisposable>().Dispose();
            return next(context);
        }

        private static Task<int> MockCurrentDomainProcessExit(CommandContext context, ExecutionDelegate next)
        {
            CancellationHandlers.TestAccess.CurrentDomain_ProcessExit();
            return next(context);
        }

        private static Task<int> MockCurrentDomainUnhandledExceptionTerminating(CommandContext context, ExecutionDelegate next)
        {
            CancellationHandlers.TestAccess.CurrentDomain_UnhandledException(true);
            return next(context);
        }

        private static Task<int> MockCurrentDomainUnhandledExceptionNonTerminating(CommandContext context, ExecutionDelegate next)
        {
            CancellationHandlers.TestAccess.CurrentDomain_UnhandledException(false);
            return next(context);
        }

        public class App
        {
            public int Do()
            {
                return 5;
            }

            public int NestMockCtrlC()
            {
                return new AppRunner<App>()
                    .UseCancellationHandlers()
                    .Configure(c =>
                    {
                        c.UseMiddleware(MockCtrlC, MiddlewareStages.PostTokenizePreParseInput);
                        c.UseMiddleware(ShouldNotReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                    })
                    .Run("Do");
            }

            public int NestMockCurrentDomainProcessExit()
            {
                return new AppRunner<App>()
                    .UseCancellationHandlers()
                    .Configure(c =>
                    {
                        c.UseMiddleware(MockCurrentDomainProcessExit, MiddlewareStages.PostTokenizePreParseInput);
                        c.UseMiddleware(ShouldNotReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                    })
                    .Run("Do");
            }

            public int NestMockCurrentDomainUnhandledExceptionTerminating()
            {
                return new AppRunner<App>()
                    .UseCancellationHandlers()
                    .Configure(c =>
                    {
                        c.UseMiddleware(MockCurrentDomainUnhandledExceptionTerminating, MiddlewareStages.PostTokenizePreParseInput);
                        c.UseMiddleware(ShouldNotReachThisStep, MiddlewareStages.PostTokenizePreParseInput);
                    })
                    .Run("Do");
            }
        }
    }
}