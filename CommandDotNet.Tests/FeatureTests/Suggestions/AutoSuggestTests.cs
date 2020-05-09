using CommandDotNet.Parsing;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Suggestions
{
    public class AutoSuggestTests
    {
        public AutoSuggestTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void AutoSuggest_Commands()
        {
            new AppRunner<CafeApp>()
                .UseTypoSuggestions()
                .UseAutoSuggestDirective()
                .Verify(new Scenario
                {
                    When = { Args = "[suggest]"},
                    Then =
                    {
                        Output = "Eat"
                    }
                });
        }

        [Fact]
        public void AutoSuggest_Options()
        {
            new AppRunner<CafeApp>()
                .UseTypoSuggestions()
                .UseAutoSuggestDirective()
                .Verify(new Scenario
                {
                    When = { Args = "[suggest] Eat"},
                    Then =
                    {
                        Output = @"--fruit
--vegetable
"
                    }
                });
        }
    }
}