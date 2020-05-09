using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.EnabledMiddlewareScenarios
{
    public class NameTransformationTests
    {
        public NameTransformationTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void TransformNameForAllTypesShouldWork()
        {
            new AppRunner<App>()
                .Configure(b => b.NameTransformation = (attributes, memberName, nameOverride, commandNodeType)
                    => $"prefix-{memberName}")
                .Verify(new Scenario
                {
                    When = {Args = "prefix-Do -h"},
                    Then =
                    {
                        OutputContainsTexts =
                        {
                            "prefix-Do", "prefix-option1", "prefix-operand1"
                        }
                    }
                });
        }

        [Fact]
        public void TransformNameForOnlyCommandTypesShouldWork()
        {
            new AppRunner<App>()
                .Configure(b => b.NameTransformation = (attributes, memberName, nameOverride, commandNodeType)
                    => commandNodeType.IsCommand ? $"prefix-{memberName}" : memberName)
                .Verify(new Scenario
                {
                    When = {Args = "prefix-Do -h"},
                    Then =
                    {
                        OutputContainsTexts =
                        {
                            "prefix-Do", "option1", "operand1"
                        },
                        OutputNotContainsTexts =
                        {
                            "prefix-option1", "prefix-operand1"
                        }
                    }
                });
        }

        [Fact]
        public void TransformNameForOnlyOperandTypesShouldWork()
        {
            new AppRunner<App>()
                .Configure(b => b.NameTransformation = (attributes, memberName, nameOverride, commandNodeType)
                    => commandNodeType.IsOperand ? $"prefix-{memberName}" : memberName)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        OutputContainsTexts =
                        {
                            "Do", "option1", "prefix-operand1"
                        },
                        OutputNotContainsTexts =
                        {
                            "prefix-option1"
                        }
                    }
                });
        }

        class App
        {
            public void Do(
                [Option] string option1,
                [Operand] string operand1)
            {

            }
        }
    }
}
