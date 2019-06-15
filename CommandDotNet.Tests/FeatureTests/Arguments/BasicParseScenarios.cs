using CommandDotNet.Attributes;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class BasicParseScenarios : ScenarioTestBase<BasicParseScenarios>
    {
        public BasicParseScenarios(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<SingleCommandApp>("method is called with expected values")
                {
                    WhenArgs = "Add -o * 2 3",
                    Then = {Outputs = {new SingleCommandApp.AddResults {X = 2, Y = 3, Op = "*"}}}
                },
                new Given<SingleCommandApp>("option can be specified after positional arg")
                {
                    WhenArgs = "Add 2 3 -o *",
                    Then = {Outputs = {new SingleCommandApp.AddResults {X = 2, Y = 3, Op = "*"}}}
                },
                new Given<SingleCommandApp>("option can be colon separated: --option:value")
                {
                    WhenArgs = "Add 2 3 -o:*",
                    Then = {Outputs = {new SingleCommandApp.AddResults {X = 2, Y = 3, Op = "*"}}}
                },
                new Given<SingleCommandApp>("option can be equals separated: --option=value")
                {
                    WhenArgs = "Add 2 3 -o=*",
                    Then = {Outputs = {new SingleCommandApp.AddResults {X = 2, Y = 3, Op = "*"}}}
                },
                new Given<SingleCommandApp>("error when extra value provided for option")
                {
                    WhenArgs = "Add 2 3 -o * %",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = {"Unrecognized command or argument '%'"}
                    }
                },
                new Given<SingleCommandApp>("extra arguments not allowed")
                {
                    WhenArgs = "Add 2 3 4",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = {"Unrecognized command or argument '4'"}
                    }
                }
            };

        [Fact(Skip = "Method params cannot be marked as required yet.  Requiredness is only possible via FluentValidator")]
        public void PositionalArgumentsAreRequired()
        {
            Verify(new Given<SingleCommandApp>
            {
                WhenArgs = "Add 2",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = {"missing positional argument 'Y'"}
                }
            });
        }

        public class SingleCommandApp
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Add(
                [Argument(Description = "the first operand")]
                int x,
                [Argument(Description = "the second operand")]
                int y,
                [Option(ShortName = "o", LongName = "operator", Description = "the operation to apply")]
                string operation = "+")
            {
                TestOutputs.Capture(new AddResults { X = x, Y = y, Op = operation });
            }

            public class AddResults
            {
                public int X { get; set; }
                public int Y { get; set; }
                public string Op { get; set; }
            }
        }
    }
}