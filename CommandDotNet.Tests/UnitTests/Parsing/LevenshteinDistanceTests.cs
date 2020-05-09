using CommandDotNet.Parsing.Typos;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests.Parsing
{
    public class LevenshteinDistanceTests
    {
        // https://www.eximiaco.tech/en/2019/11/17/computing-the-levenshtein-edit-distance-of-two-strings-using-c/

        [Theory]
        [InlineData("ant", "aunt", 1)]
        [InlineData("fast", "cats", 3)]
        [InlineData("Elemar", "Vilmar", 3)]
        [InlineData("kitten", "sitting", 3)]
        public void ComputeTheDistanceBetween(string s1, string s2, int expectedDistance)
        {
            Levenshtein.ComputeDistance(s1, s2).Should().Be(expectedDistance);
        }
    }
}