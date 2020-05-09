using System;
using CommandDotNet.Parsing.Typos;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class SuggestionRankingExtensionsTests: IDisposable
    {
        private readonly IDisposable _cleanup;

        public SuggestionRankingExtensionsTests(ITestOutputHelper output)
        {
            _cleanup = TestToolsLogProvider.InitLogProvider(output.WriteLine);
        }

        public void Dispose()
        {
            _cleanup.Dispose();
        }

        [Theory]
        [InlineData("o", "one", "two")]
        [InlineData("n", "one")]
        [InlineData("e", "one")]
        [InlineData("t", "two")]
        [InlineData("w", "two")]
        public void AtLeastOneLetterHasToBeInCommon(string typo, params string[] expectedSuggestions)
        {
            new[] {"one", "two"}
                .RankAndTrimSuggestions(typo, 3)
                .Should()
                .BeEquivalentSequenceTo(expectedSuggestions);
        }

        [Theory]
        [InlineData("get", "git")]
        [InlineData("mote", "models", "git")]
        [InlineData("sipes", "pipes")]
        [InlineData("qend-af", "send-after")]
        [InlineData("rety0count", "retry-count")]
        [InlineData("cryrun", "dryrun")]
        public void GivenMixedNames(string typo, params string[] expectedSuggestions)
        {
            new[] { "cancel-me", "git", "models", "pipes", "prompts", "send-after", "retry-count", "dryrun" }
                .RankAndTrimSuggestions(typo, 3)
                .Should()
                .BeEquivalentSequenceTo(expectedSuggestions);
        }

        [Theory]
        [InlineData("ain", "drain", "grain")]
        [InlineData("apes", "drapes", "grapes")]
        [InlineData("gra", "grain", "grapes", "drain")]
        [InlineData("dra", "drain", "drapes", "grain")]
        public void GivenSimilarNames(string typo, params string[] expectedSuggestions)
        {
            new[] { "grain", "grapes", "drapes", "drain" }
                .RankAndTrimSuggestions(typo, 3)
                .Should()
                .BeEquivalentSequenceTo(expectedSuggestions);
        }

        [Theory]
        [InlineData("appl", 1, "apple1")]
        [InlineData("appl", 2, "apple1", "apple2")]
        [InlineData("appl", 3, "apple1", "apple2", "apple3")]
        public void LimitSuggestionCount(string typo, int count, params string[] expectedSuggestions)
        {
            new[] { "apple1", "apple2", "apple3", "apple4", "apple5" }
                .RankAndTrimSuggestions(typo, count)
                .Should()
                .BeEquivalentSequenceTo(expectedSuggestions);
        }

        [Theory]
        [InlineData("nameuser", "username,password", "username")]
        [InlineData("treework", "worktree,trek", "worktree,trek")]
        public void GivenWordsInWrongOrder(string typo, string options, string expectedSuggestions)
        {
            options.Split(",")
                .RankAndTrimSuggestions(typo, 5)
                .Should()
                .BeEquivalentSequenceTo(expectedSuggestions.Split(","));
        }
    }
}