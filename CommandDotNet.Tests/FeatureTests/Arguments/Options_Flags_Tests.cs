using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_Flags_Tests
    {
        public Options_Flags_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void Help_DoesNotInclude_BoolTypeOrAllowedArgumentValues()
        {
            new AppRunner<FlagApp>().Verify(new Scenario
            {
                // because the value should not be provided
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options]

Options:

  --flag
"
                }
            });
        }

        [Fact]
        public void WhenFlagIsSpecified_ValueIsTrue()
        {
            new AppRunner<FlagApp>().Verify(new Scenario
            {
                When = {Args = "Do --flag"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(true)}
            });
        }

        [Fact]
        public void WhenFlagIsNotSpecified_ValueIsFalse()
        {
            new AppRunner<FlagApp>().Verify(new Scenario
            {
                When = {Args = "Do"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(new object?[]{ null })}
            });
        }

        [Fact]
        public void FlagsCanBeClubbed()
        {
            new AppRunner<FlagApp>().Verify(new Scenario
            {
                When = {Args = "Club -ab"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(true, true)}
            });
        }

        private class FlagApp
        {
            public void Do([Option] bool flag)
            {
            }

            public void Club(
                [Option(ShortName = "a")] bool flagA, 
                [Option(ShortName = "b")] bool flagB)
            {
            }
        }
    }
}
