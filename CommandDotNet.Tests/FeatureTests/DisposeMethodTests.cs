using System;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class DisposeMethodTests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public DisposeMethodTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void When_IDisposable_BasicHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<DisposableApp>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then = {OutputNotContainsTexts = {"Dispose"}}
            });
        }

        [Fact]
        public void When_IDisposable_DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<DisposableApp>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then = {OutputNotContainsTexts = {"Dispose"}}
            });
        }

        [Fact]
        public void When_NotIDisposable_BasicHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<NotDisposableApp>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then = { OutputContainsTexts = { @"Commands:
  Dispose" } }
            });
        }

        [Fact]
        public void When_NotIDisposable_DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            new AppRunner<NotDisposableApp>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then = { OutputContainsTexts = { @"Commands:

  Dispose" } }
            });
        }

        [Fact]
        public void When_IDisposable_CallsDisposeMethod()
        {
            new AppRunner<DisposableApp>().Verify(new Scenario
            {
                When = {Args = "Do"},
                Then =
                {
                    AssertContext = ctx =>
                        ctx.GetCommandInvocationInfo<DisposableApp>().Instance!.WasDisposed.Should().BeTrue()
                }
            });
        }

        [Fact]
        public void When_NotIDisposable_CallsDisposeMethod()
        {
            new AppRunner<NotDisposableApp>().Verify(new Scenario
            {
                When = {Args = "Dispose"},
                Then =
                {
                    AssertContext = ctx =>
                        ctx.GetCommandInvocationInfo<NotDisposableApp>().Instance!.WasDispose.Should().BeTrue()
                }
            });

            // sanity check that Dispose wasn't executed by some weird name-matching logic.
            new AppRunner<NotDisposableApp>().Verify(new Scenario

            {
                When = {Args = "NotDispose"},
                Then =
                {
                    AssertContext = ctx =>
                        ctx.GetCommandInvocationInfo<NotDisposableApp>().Instance!.WasDispose.Should().BeFalse()
                }
            });
        }

        private class DisposableApp : IDisposable
        {
            public bool WasDisposed;

            public void Do()
            {
            }

            public void Dispose()
            {
                WasDisposed = true;
            }
        }

        private class NotDisposableApp
        {
            public bool WasDispose;

            // use the name Dispose to prove it can be a command name
            // and that the Dispose name is filtered out only for IDisposable's
            public void Dispose()
            {
                WasDispose = true;
            }

            public void NotDispose()
            {
            }
        }

    }
}
