using System.Collections.Specialized;
using CommandDotNet.Extensions;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ArgumentDefaults
{
    public class DefaultFromAppSettingsTests
    {
        public DefaultFromAppSettingsTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Theory]
        [InlineData("ByConvention", false, "--option1")]
        [InlineData("ByConvention --option1", true, "--option1")]
        [InlineData("ByConvention -o", true, "--option1")]
        [InlineData("--option1", true, "--option1")]
        [InlineData("-o", true, "--option1")]
        [InlineData("option1", false, "--option1")]
        [InlineData("o", false, "--option1")]
        [InlineData("ByConvention operand1", true, "operand1")]
        [InlineData("operand1", true, "operand1")]
        [InlineData("--operand1", false, "operand1")]
        // optional default values are overridden
        [InlineData("ByConvention --option2", true, "--option2")]
        [InlineData("ByConvention -t", true, "--option2")]
        [InlineData("--option2", true, "--option2")]
        [InlineData("-t", true, "--option2")]
        [InlineData("option2", false, "--option2")]
        [InlineData("t", false, "--option2")]
        [InlineData("ByConvention operand2", true, "operand2")]
        [InlineData("operand2", true, "operand2")]
        [InlineData("--operand2", false, "operand2")]
        public void ByConvention(string key, bool includes, string nameToInclude)
        {
            var scenario = includes
                ? new Scenario
                {
                    When = {Args = "ByConvention -h"},
                    Then = {OutputContainsTexts = {$"{nameToInclude}  <TEXT>  [red]"}}
                }
                : new Scenario
                {
                    When = {Args = "ByConvention -h"},
                    Then = {OutputNotContainsTexts = {$"{nameToInclude}  <TEXT>  [red]"}}
                };

            new AppRunner<App>()
                .UseDefaultsFromAppSetting(
                    new NameValueCollection {{key, "red"}},
                    includeNamingConventions: true)
                .Verify(scenario);
        }

        [Theory]
        // argument names are not used
        [InlineData("ByAttribute -o", false, "--option1")]
        [InlineData("-o", false, "--option1")]
        [InlineData("ByAttribute --option1", false, "--option1")]
        [InlineData("--option1", false, "--option1")]
        [InlineData("ByAttribute operand1", false, "operand1")]
        [InlineData("operand1", false, "operand1")]

        // command names are not used
        [InlineData("ByAttribute", false, "--option1")]
        [InlineData("ByAttribute opt1", false, "opt1")]

        // AppSettings attr keys are used
        [InlineData("opt1", true, "--option1")]
        [InlineData("oper1", true, "operand1")]
        // optional default values are overridden
        [InlineData("opt2", true, "--option2")]
        [InlineData("oper2", true, "operand2")]
        public void ByAttribute(string key, bool includes, string nameToInclude)
        {
            var scenario = includes
                ? new Scenario
                {
                    When = {Args = "ByAttribute -h"},
                    Then = { OutputContainsTexts = { $"{nameToInclude}  <TEXT>  [red]" } }
                }
                : new Scenario
                {
                    When = {Args = "ByAttribute -h"},
                    Then = { OutputNotContainsTexts = { $"{nameToInclude}  <TEXT>  [red]" } }
                };

            new AppRunner<App>()
                .UseDefaultsFromAppSetting(
                    new NameValueCollection { { key, "red" } })
                .Verify(scenario);
        }

        [Theory]
        [InlineData("ByAttribute", false, "--option1")]
        [InlineData("ByAttribute --option1", true, "--option1")]
        [InlineData("ByAttribute -o", true, "--option1")]
        [InlineData("--option1", true, "--option1")]
        [InlineData("-o", true, "--option1")]
        [InlineData("option1", false, "--option1")]
        [InlineData("o", false, "--option1")]
        [InlineData("ByAttribute operand1", true, "operand1")]
        [InlineData("operand1", true, "operand1")]
        [InlineData("--operand1", false, "operand1")]
        // optional default values are overridden
        [InlineData("ByAttribute --option2", true, "--option2")]
        [InlineData("ByAttribute -t", true, "--option2")]
        [InlineData("--option2", true, "--option2")]
        [InlineData("-t", true, "--option2")]
        [InlineData("option2", false, "--option2")]
        [InlineData("t", false, "--option2")]
        [InlineData("ByAttribute operand2", true, "operand2")]
        [InlineData("operand2", true, "operand2")]
        [InlineData("--operand2", false, "operand2")]
        // AppSettings attr keys are used
        [InlineData("opt1", true, "--option1")]
        [InlineData("oper1", true, "operand1")]
        // optional default values are overridden
        [InlineData("opt2", true, "--option2")]
        [InlineData("oper2", true, "operand2")]
        public void ByAttributeAndConvention(string key, bool includes, string nameToInclude)
        {
            var scenario = includes
                ? new Scenario
                {
                    When = {Args = "ByAttribute -h"},
                    Then = { OutputContainsTexts = { $"{nameToInclude}  <TEXT>  [red]" } }
                }
                : new Scenario
                {
                    When = {Args = "ByAttribute -h"},
                    Then = { OutputNotContainsTexts = { $"{nameToInclude}  <TEXT>  [red]" } }
                };

            new AppRunner<App>()
                .UseDefaultsFromAppSetting(
                    new NameValueCollection { { key, "red" } },
                    includeNamingConventions: true)
                .Verify(scenario);
        }

        [Theory]
        [InlineData("--option1=blue;opt1=red", true, "--option1", "red")]
        [InlineData("operand2=blue;oper2=red", true, "operand2", "red")]
        public void ByAttributeAndConvention_FavorsAttribute(string settings, bool includes, string nameToInclude, string color)
        {
            var scenario = includes
                ? new Scenario
                {
                    When = {Args = "ByAttribute -h"},
                    Then = { OutputContainsTexts = { $"{nameToInclude}  <TEXT>  [{color}]" } }
                }
                : new Scenario
                {
                    When = {Args = "ByAttribute -h"},
                    Then = { OutputNotContainsTexts = { $"{nameToInclude}  <TEXT>  [{color}]" } }
                };

            var nvc = new NameValueCollection();
            settings.Split(';').ForEach(s =>
            {
                var parts = s.Split('=');
                nvc.Add(parts[0], parts[1]);
            });

            new AppRunner<App>()
                .UseDefaultsFromAppSetting(nvc, includeNamingConventions: true)
                .Verify(scenario);
        }

        [Theory]
        [InlineData("List", "planets", "mars,pluto")]
        [InlineData("List", "planets", "mars")]
        public void CsvValues(string args, string key, string value)
        {
            var nvc = new NameValueCollection
            {
                {key, value}
            };

            var expectedParamValues = value.Split(',');
            new AppRunner<App>()
                .UseDefaultsFromAppSetting(nvc, includeNamingConventions: true)
                .Verify(new Scenario
                {
                    When = {Args = args},
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(new object[]{expectedParamValues})}
                });
        }

        public class App
        {
            public void ByConvention(
                [Option(LongName = "option1", ShortName = "o")] string option1,
                [Operand] string operand1,
                [Option(LongName = "option2", ShortName = "t")] string option2 = "lala",
                [Operand] string operand2 = "fishies")
            {

            }

            public void ByAttribute(
                [AppSetting("opt1")] [Option(LongName = "option1", ShortName = "o")]
                string option1,
                [AppSetting("oper1")] [Operand] string operand1,
                [AppSetting("opt2")] [Option(LongName = "option2", ShortName = "t")]
                string option2 = "lala",
                [AppSetting("oper2")] [Operand] string operand2 = "fishies"
            )
            {

            }

            public void List(string[] planets)
            {
            }
        }
    }
}
