using System;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class AutoSuggestTests
    {
        private readonly ITestOutputHelper _output;

        public AutoSuggestTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void AutoSuggest_Commands()
        {
            new AppRunner<App>()
                .UseTypoSuggestions()
                .UseAutoSuggestDirective()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "[suggest]",
                    Then =
                    {
                        Result = "Eat"
                    }
                });
        }

        [Fact]
        public void AutoSuggest_Options()
        {
            new AppRunner<App>()
                .UseTypoSuggestions()
                .UseAutoSuggestDirective()
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "[suggest] Eat",
                    Then =
                    {
                        Result = @"--fruit
--vegetable"
                    }
                });
        }

        public enum Fruit { Apple, Banana, Cherry }
        public enum Vegetable { Asparagus, Broccoli, Carrot }
        public enum Meal { Breakfast, Lunch, Dinner }

        public class App
        {
            public void Eat(IConsole console, 
                [Operand] Meal meal, 
                [Option] Vegetable vegetable, 
                [Option] Fruit fruit)
            {
                console.Out.WriteLine($"{nameof(meal)}     :{meal}");
                console.Out.WriteLine($"{nameof(fruit)}    :{fruit}");
                console.Out.WriteLine($"{nameof(vegetable)}:{vegetable}");
            }
        }
    }
}