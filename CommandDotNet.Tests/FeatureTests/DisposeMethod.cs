using System;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{

    public class DisposeMethod : TestBase
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public DisposeMethod(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void When_IDisposable_BasicHelp_DoesNotInclude_DisposeMethod()
        {
            Verify(new Given<DisposableApp>
            {
                And = {AppSettings = BasicHelp},
                WhenArgs = "-h",
                Then = {ResultsNotContainsTexts = {"Dispose"}}
            });
        }

        [Fact]
        public void When_IDisposable_DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            Verify(new Given<DisposableApp>
            {
                And = {AppSettings = DetailedHelp},
                WhenArgs = "-h",
                Then = {ResultsNotContainsTexts = {"Dispose"}}
            });
        }

        [Fact]
        public void When_NotIDisposable_BasicHelp_DoesNotInclude_DisposeMethod()
        {
            Verify(new Given<NotDisposableApp>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { @"Commands:
  Dispose  " } }
            });
        }

        [Fact]
        public void When_NotIDisposable_DetailedHelp_DoesNotInclude_DisposeMethod()
        {
            Verify(new Given<NotDisposableApp>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { @"Commands:

  Dispose  " } }
            });
        }

        [Fact]
        public void When_IDisposable_CallsDisposeMethod()
        {
            Verify(new Given<DisposableApp>
            {
                WhenArgs = "Do",
                Then = {Outputs = {true}}
            });
        }

        [Fact]
        public void When_NotIDisposable_CallsDisposeMethod()
        {
            Verify(new Given<NotDisposableApp>
            {
                WhenArgs = "Dispose",
                Then = { Outputs = { true } }
            });
        }

        public class DisposableApp : IDisposable
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do()
            {
            }

            public void Dispose()
            {
                TestOutputs.Capture(true);
            }
        }

        public class NotDisposableApp
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Dispose()
            {
                TestOutputs.Capture(true);
            }
        }

    }
}