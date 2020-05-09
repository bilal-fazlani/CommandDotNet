using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseDirective_Transforms_Tests
    {
        public ParseDirective_Transforms_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void When_ParseT_ShowsWhenTransformDoesNotMakeChanges()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .Verify(new Scenario
                {
                    When = {Args = "[parse:t] Do"},
                    Then =
                    {
                        OutputContainsTexts = { @"token transformations:

>>> from shell
  Directive: [parse:t]
  Value    : Do
>>> after: expand-clubbed-flags (no changes)
>>> after: split-option-assignments (no changes)" }
                    }
                });
        }

        [Fact]
        public void When_ParseT_ShowsResultsOfEveryTransform()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .Verify(new Scenario
                {
                    When = {Args = "[parse:t] Do -abc --one two --three:four --five=six seven"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { @"token transformations:

>>> from shell
  Directive: [parse:t]
  Value    : Do
  Option   : -abc
  Option   : --one
  Value    : two
  Option   : --three:four
  Option   : --five=six
  Value    : seven
>>> after: expand-clubbed-flags
  Directive: [parse:t]
  Value    : Do
  Option   : -a
  Option   : -b
  Option   : -c
  Option   : --one
  Value    : two
  Option   : --three:four
  Option   : --five=six
  Value    : seven
>>> after: split-option-assignments
  Directive: [parse:t]
  Value    : Do
  Option   : -a
  Option   : -b
  Option   : -c
  Option   : --one
  Value    : two
  Option   : --three
  Value    : four
  Option   : --five
  Value    : six
  Value    : seven" }
                    }
                });
        }

        public class App
        {
            public void Do() { }
        }
    }
}
