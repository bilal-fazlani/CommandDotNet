using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class OnRunCompletedTests
    {
        public OnRunCompletedTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void OnRunCompleted_IsCalled_AfterPipelineIsExecuted()
        {
            bool wasCalled = false;
            new AppRunner<App>()
                .Configure(b => b.OnRunCompleted += args =>
                {
                    wasCalled = true;
                })
                .RunInMem("");

            wasCalled.Should().BeTrue();
        }

        public class App
        {
            public void Do() { }
        }
    }
}