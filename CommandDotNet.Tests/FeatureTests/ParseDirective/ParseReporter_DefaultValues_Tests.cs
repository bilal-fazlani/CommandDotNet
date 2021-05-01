using System.Collections.Generic;
using System.Collections.Specialized;
using CommandDotNet.ConsoleOnly;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseReporter_DefaultValues_Tests
    {
        public ParseReporter_DefaultValues_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void Defaults_FromExternalSources_IncludeSourceName()
        {
            var appSettings = new NameValueCollection
            {
                { "opd", "woo" },
                { "--opt", "hoo" }
            };

            var envVars = new Dictionary<string, string>
            {
                { "opdList", "a,b,c" },
                { "optList", "four,five,six" }
            };

            new AppRunner<App>()
                .UseDefaultsFromEnvVar(envVars)
                .UseDefaultsFromAppSetting(appSettings, includeNamingConventions: true)
                .UseParseDirective()
                .Verify(new Scenario
                {
                    When = {Args = "[parse] Do"},
                    Then =
                    {
                        Output = @"command: Do

arguments:

  opd <Text>
    value: woo
    inputs:
    default: source=AppSetting key=opd: woo

  opdList <Text>
    value: a, b, c
    inputs:
    default: source=EnvVar key=opdList: a, b, c

options:

  opt <Text>
    value: hoo
    inputs:
    default: source=AppSetting key=--opt: hoo

  optList <Text>
    value: four, five, six
    inputs:
    default: source=EnvVar key=optList: four, five, six

Parse usage: [parse:t:raw] to include token transformations.
 't' to include token transformations.
 'raw' to include command line as passed to this process.
"
                    }
                });
        }

        [Fact]
        public void Defaults_AreShown_And_ListsAreCsvJoined()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .Verify(new Scenario
                {
                    When = {Args = "[parse] Do"},
                    Then =
                    {
                        Output = @"command: Do

arguments:

  opd <Text>
    value: lala
    inputs:
    default: lala

  opdList <Text>
    value: one, two, three
    inputs:
    default: one, two, three

options:

  opt <Text>
    value: fishies
    inputs:
    default: fishies

  optList <Text>
    value: one, two, three
    inputs:
    default: one, two, three

Parse usage: [parse:t:raw] to include token transformations.
 't' to include token transformations.
 'raw' to include command line as passed to this process.
"
                    }
                });
        }

        public class App
        {

            public void Do(IConsole console,
                [Operand] string opd = "lala",
                [Option] string opt = "fishies",
                ListsWithDefaults? listsWithDefaults = null)
            {

            }
        }

        public class ListsWithDefaults : IArgumentModel
        {
            [EnvVar("opdList")]
            [Operand]
            public List<string>? opdList { get; set; } = new List<string> { "one", "two", "three" };

            [EnvVar("optList")]
            [Option(ShortName = "l")]
            public List<string>? optList { get; set; } = new List<string> { "one", "two", "three" };
        }
    }
}
