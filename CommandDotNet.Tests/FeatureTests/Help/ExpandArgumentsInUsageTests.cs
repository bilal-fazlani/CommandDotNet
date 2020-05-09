using System.IO;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class ExpandArgumentsInUsageTests
    {
        public ExpandArgumentsInUsageTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void Given_Not_ExpandArgumentsInUsage_Then_ArgumentsAreListedByName()
        {
            new AppRunner<App>(new AppSettings { Help = { ExpandArgumentsInUsage = false } })
                .Verify(new Scenario
                {
                    When = { Args = "Do -h" },
                    Then = { OutputContainsTexts = { "Usage: dotnet testhost.dll Do [arguments]" } }
                });
        }

        [Fact]
        public void Given_ExpandArgumentsInUsage_Then_ArgumentsAreListedByName()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then = { OutputContainsTexts = { "Usage: dotnet testhost.dll Do <arg1> <arg2> [<optional>]" } }
                });
        }

        [Fact]
        public void Given_ExpandArgumentsInUsage_Then_ArgumentsAreListedAfterOptions()
        {
            new AppRunner<App>(new AppSettings { Help = { ExpandArgumentsInUsage = true } })
                .Verify(new Scenario
                {
                    When = {Args = "Do2 -h"},
                    Then = { OutputContainsTexts = { "Usage: dotnet testhost.dll Do2 [options] <arg1> <arg2> [<optional>]" } }
                });
        }

        [Fact]
        public void Given_DefaultsInMixedOrder_Then_HelpShowsLastContiguousDefaultsAsOptional()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = { Args = "DefaultsInMixedOrder -h" },
                    Then = { OutputContainsTexts = { "Usage: dotnet testhost.dll DefaultsInMixedOrder <arg1> <Arg2> <arg3> [<Arg4> <arg5>]" } }
                });
        }

        public class App
        {
            public void Do(string arg1, FileInfo arg2, string optional = "lala")
            {

            }

            public void Do2(
                string arg1, FileInfo arg2, [Option] string option1, 
                string optional = "lala", [Option] string optionalOption = "fishies")
            {

            }

            public void DefaultsInMixedOrder(int arg1, Model1 model1, string arg3, Model2 model2, string arg5 = "default")
            {

            }
        }

        public class Model1 : IArgumentModel
        {
            [Operand]
            public string Arg2 { get; set; } = "default";
        }

        public class Model2 : IArgumentModel
        {
            [Operand]
            public string Arg4 { get; set; } = "default";
        }
    }
}
