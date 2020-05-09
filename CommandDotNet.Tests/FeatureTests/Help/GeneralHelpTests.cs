using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class GeneralHelpTests
    {
        public GeneralHelpTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void QuestionMark_ShowsHelp()
        {
            var result = new AppRunner<App>().RunInMem("-?".SplitArgs());
            result.CommandContext.ShowHelpOnExit.Should().BeTrue();
        }

        [Fact]
        public void ShortName_ShowsHelp()
        {
            var result = new AppRunner<App>().RunInMem("-h".SplitArgs());
            result.CommandContext.ShowHelpOnExit.Should().BeTrue();
        }

        [Fact]
        public void LongName_ShowsHelp()
        {
            var result = new AppRunner<App>().RunInMem("--help".SplitArgs());
            result.CommandContext.ShowHelpOnExit.Should().BeTrue();
        }

        public class App
        {
            public void Do(){ }
        }
    }
}