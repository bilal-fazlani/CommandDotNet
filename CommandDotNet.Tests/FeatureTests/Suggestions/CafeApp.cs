using CommandDotNet.Rendering;

namespace CommandDotNet.Tests.FeatureTests.Suggestions
{
    // example borrowed from
    // https://github.com/dotnet/command-line-api/blob/master/src/System.CommandLine.Tests/SuggestionTests.cs

    public enum Fruit { Apple, Banana, Cherry }
    public enum Vegetable { Asparagus, Broccoli, Carrot }
    public enum Meal { Breakfast, Lunch, Dinner }

    public class CafeApp
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