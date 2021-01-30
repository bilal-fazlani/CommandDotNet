using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Suggestions
{
    public class AllowedValuesTypoSuggestionsTests
    {
        public AllowedValuesTypoSuggestionsTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void Given_OperandValueTypo_SuggestAllowedValues()
        {
            new AppRunner<CafeApp>()
                .UseTypoSuggestions()
                .Verify(new Scenario
                {
                    When = { Args = "Eat linner" },
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts =
                        {
                            @"'linner' is not a valid Meal

Did you mean ...
   Dinner

See 'dotnet testhost.dll Eat --help'"
                        }
                    }
                });
        }

        [Fact]
        public void Given_OperandValueTypo_OfEmptyString_DoesNotSuggest_AndHelpIsShown()
        {
            new AppRunner<CafeApp>()
                .UseTypoSuggestions()
                .Verify(new Scenario
                {
                    When = { Args = "Eat \"\"" },
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts =
                        {
                            @"Unrecognized value '' for argument: meal

Usage: dotnet testhost.dll Eat [options] <meal>"
                        }
                    }
                });
        }

        [Fact]
        public void Given_OptionValueTypo_SuggestAllowedValues()
        {
            new AppRunner<CafeApp>()
                .UseTypoSuggestions()
                .Verify(new Scenario
                {
                    When = { Args = "Eat Dinner --fruit cerry" },
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts =
                        {
                            @"'cerry' is not a valid Fruit

Did you mean ...
   Cherry

See 'dotnet testhost.dll Eat --help'"
                        }
                    }
                });
        }

        [Fact]
        public void Given_OptionValueTypo_OfEmptyString_DoesNotSuggest_AndHelpIsShown()
        {
            new AppRunner<CafeApp>()
                .UseTypoSuggestions()
                .Verify(new Scenario
                {
                    When = { Args = "Eat Dinner --fruit \"\"" },
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts =
                        {
                            @"Unrecognized value '' for option: fruit

Usage: dotnet testhost.dll Eat [options] <meal>"
                        }
                    }
                });
        }
    }
}