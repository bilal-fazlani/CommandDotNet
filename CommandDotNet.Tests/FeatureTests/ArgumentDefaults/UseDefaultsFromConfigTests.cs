using System;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ArgumentDefaults
{
    public class UseDefaultsFromConfigTests
    {
        public UseDefaultsFromConfigTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void GivenDefaultValue_Should_DefaultForArgument()
        {
            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => Config("red"))
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe("red")}
                });
        }

        [Fact]
        public void GivenDefaultValue_Should_OverrideArgumentDefault()
        {
            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => Config("red"))
                .Verify(new Scenario
                {
                    When = {Args = "Default"},
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe("red")}
                });
        }

        [Fact]
        public void GivenCsvValue_Should_DefaultForArgument()
        {
            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => Config("red,blue,green"))
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe("red,blue,green")}
                });
        }

        [Fact]
        public void GivenNullValue_Should_Skip()
        {
            // null can indicate the key does not exist so default should not be overridden

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => null)
                .Verify(new Scenario
                {
                    When = {Args = "Default"},
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe("lala")}
                });
        }

        [Fact]
        public void GivenMultipleSources_AllAreUsed()
        {
            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => arg.Name == "first" ? new ArgumentDefault("1", "1", "one") : null)
                .UseDefaultsFromConfig(arg => arg.Name == "second" ? new ArgumentDefault("2", "2", "two") : null)
                .Verify(new Scenario
                {
                    When = {Args = "Multi"},
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe("one", "two") }
                });
        }

        [Fact]
        public void GivenMultipleSources_FirstRegisteredWins()
        {
            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => arg.Name == "first" ? new ArgumentDefault("1", "1", "right one") : null)
                .UseDefaultsFromConfig(arg => arg.Name == "first" ? new ArgumentDefault("1", "1", "wrong one") : null)
                .UseDefaultsFromConfig(arg => arg.Name == "second" ? new ArgumentDefault("2", "2", "two") : null)
                .Verify(new Scenario
                {
                    When = {Args = "Multi"},
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe("right one", "two") }
                });
        }

        private static ArgumentDefault Config(string value)
        {
            return new ArgumentDefault("test", "key", value);
        }

        public class App
        {
            public void Multi([Operand] string first, [Operand] string second)
            {
            }

            public void Do([Operand] string op1)
            {
            }

            public void List([Operand] string[] ops)
            {
            }

            public void Default([Operand] string op1 = "lala")
            {
            }
        }
    }
}
