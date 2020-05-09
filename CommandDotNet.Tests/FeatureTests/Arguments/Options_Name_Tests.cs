using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_Name_Tests
    {
        private readonly Dictionary<string, Option> _options;

        public Options_Name_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;

            _options = new AppRunner<App>()
                .GetFromContext("Do".SplitArgs(),
                    ctx => ctx.ParseResult!.TargetCommand.Options,
                    middlewareStage: MiddlewareStages.PostParseInputPreBindValues)
                .ToDictionary(o => o.DefinitionSource!.Split('.').Last());
        }

        [InlineData(DefaultName, DefaultName)]
        [InlineData(LongNameOverride, "longName1")]
        [InlineData(ShortNameOverride, ShortNameOverride)]
        [InlineData(ShortAndLongNameOverride, "longName2")]
        [InlineData(LongNameNull, "c")]
        [InlineData(LongNameEmpty, "d")]
        [Theory]
        public void NameShouldBe(string propertyName, string name)
        {
            var option = _options[propertyName];
            option.Name.Should().Be(name);
        }

        [InlineData(DefaultName, null)]
        [InlineData(LongNameOverride, null)]
        [InlineData(ShortNameOverride, 'a')]
        [InlineData(ShortAndLongNameOverride, 'b')]
        [InlineData(LongNameNull, 'c')]
        [InlineData(LongNameEmpty, 'd')]
        [Theory]
        public void ShortNameShouldBe(string propertyName, char? shortName)
        {
            var option = _options[propertyName];
            option.ShortName.Should().Be(shortName);
        }

        [InlineData(DefaultName, DefaultName)]
        [InlineData(LongNameOverride, "longName1")]
        [InlineData(ShortNameOverride, ShortNameOverride)]
        [InlineData(ShortAndLongNameOverride, "longName2")]
        [InlineData(LongNameNull, null)]
        [InlineData(LongNameEmpty, null)]
        [Theory]
        public void LongNameShouldBe(string propertyName, string longName)
        {
            var option = _options[propertyName];
            option.LongName.Should().Be(longName);
        }

        private const string DefaultName = "defaultName";
        private const string LongNameOverride = "longNameOverride";
        private const string ShortNameOverride = "shortNameOverride";
        private const string ShortAndLongNameOverride = "shortAndLongNameOverride";
        private const string LongNameNull = "longNameNull";
        private const string LongNameEmpty = "longNameEmpty";

        class App
        {
            public void Do(
                [Option] string defaultName,
                [Option(LongName = "longName1")] string longNameOverride,
                [Option(ShortName = "a")] string shortNameOverride,
                [Option(ShortName = "b", LongName = "longName2")] string shortAndLongNameOverride,
                [Option(ShortName = "c", LongName = null)] string longNameNull,
                [Option(ShortName = "d", LongName = "")] string longNameEmpty)
            {
            }
        }
    }
}