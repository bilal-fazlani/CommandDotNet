using System.Collections.Generic;
using System.Collections.Specialized;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ArgumentDefaults
{
    public class DefaultFromEnvVarsTests
    {
        public DefaultFromEnvVarsTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }
        
        [Theory]
        // argument names are not used
        [InlineData("ByAttribute.-o", false, "--option1")]
        [InlineData("-o", false, "--option1")]
        [InlineData("ByAttribute.--option1", false, "--option1")]
        [InlineData("--option1", false, "--option1")]
        [InlineData("ByAttribute.operand1", false, "operand1")]
        [InlineData("operand1", false, "operand1")]

        // command names are not used
        [InlineData("ByAttribute", false, "--option1")]
        [InlineData("ByAttribute.opt1", false, "opt1")]

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
                .UseDefaultsFromEnvVar(
                    new Dictionary<string,string> { { key, "red" } })
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
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(new object[] {expectedParamValues})}
                });
        }

        public class App
        {
            public void ByAttribute(
                [EnvVar("opt1")] [Option(LongName = "option1", ShortName = "o")]
                string option1,
                [EnvVar("oper1")] [Operand] string operand1,
                [EnvVar("opt2")] [Option(LongName = "option2", ShortName = "t")]
                string option2 = "lala",
                [EnvVar("oper2")] [Operand] string operand2 = "fishies"
            )
            {

            }

            public void List(string[] planets)
            {
            }
        }
    }
}
