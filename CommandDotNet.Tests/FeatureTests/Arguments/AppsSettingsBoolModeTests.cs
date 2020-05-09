using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class AppsSettingsBoolModeTests
    {
        private static readonly AppSettings ImplicitBasicHelp = TestAppSettings.BasicHelp.Clone(a => a.BooleanMode = BooleanMode.Implicit);
        private static readonly AppSettings ImplicitDetailedHelp = TestAppSettings.DetailedHelp.Clone(a => a.BooleanMode = BooleanMode.Implicit);

        private static readonly AppSettings ExplicitBasicHelp = TestAppSettings.BasicHelp.Clone(a => a.BooleanMode = BooleanMode.Explicit);
        private static readonly AppSettings ExplicitDetailedHelp = TestAppSettings.DetailedHelp.Clone(a => a.BooleanMode = BooleanMode.Explicit);

        public AppsSettingsBoolModeTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void WhenExplicit_BasicHelp_DoesNotIncludeAllowedValues()
        {
            new AppRunner<App>(ExplicitBasicHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options] <operand>

Arguments:
  operand

Options:
  --option
"
                }
            });
        }

        [Fact]
        public void WhenExplicit_DetailedHelp_IncludesAllowedValues()
        {
            new AppRunner<App>(ExplicitDetailedHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options] <operand>

Arguments:

  operand  <BOOLEAN>
  Allowed values: true, false

Options:

  --option  <BOOLEAN>
  Allowed values: true, false
"
                }
            });
        }

        [Fact]
        public void WhenImplicit_BasicHelp_DoesNotIncludeAllowedValues()
        {
            new AppRunner<App>(ImplicitBasicHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options] <operand>

Arguments:
  operand

Options:
  --option
"
                }
            });
        }

        [Fact]
        public void WhenImplicit_DetailedHelp_DoesNotIncludeAllowedValuesForOption()
        {
            new AppRunner<App>(ImplicitDetailedHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options] <operand>

Arguments:

  operand  <BOOLEAN>
  Allowed values: true, false

Options:

  --option
"
                }
            });
        }

        [Fact]
        public void WhenImplicit_Exec_OptionsIsFalseIfNotSpecified()
        {
            new AppRunner<App>(ImplicitBasicHelp).Verify(new Scenario
            {
                // bool value 'true' is operand
                When = {Args = "Do true"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(null, true)
                }
            });
        }

        [Fact]
        public void WhenImplicit_Exec_OptionsIsTrueIfSpecified()
        {
            new AppRunner<App>(ImplicitBasicHelp).Verify(new Scenario
            {
                // bool value 'false' is operand
                When = {Args = "Do --option false"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(true, false)
                }
            });
        }

        [Fact]
        public void WhenExplicit_Exec_OptionValueMustFollowTheArgument()
        {
            new AppRunner<App>(ExplicitBasicHelp).Verify(new Scenario
            {
                When = {Args = "Do2 --option 2"},
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = { "Unrecognized value '2' for option: option" }
                }
            });
        }

        [Fact]
        public void WhenExplicit_Exec_OptionValueIsRequired()
        {
            new AppRunner<App>(ExplicitBasicHelp).Verify(new Scenario
            {
                When = {Args = "Do --option"},
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = { "Missing value for option 'option'" }
                }
            });
        }

        [Fact]
        public void WhenExplicit_Exec_SpecifiedOptionValueIsUsed()
        {
            var result = new AppRunner<App>(ExplicitBasicHelp).Verify(new Scenario
            {
                When = {Args = "Do --option false true"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(false, true)
                }
            });
        }

        private class App
        {
            public void Do(
                [Option] bool option, 
                [Operand] bool operand)
            {
            }

            public void Do2(
                [Option] bool option,
                [Operand] int number)
            {
            }
        }
    }
}
