using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class BasicParseTests
    {
        public BasicParseTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void MethodIsCalledWithExpectedValues()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add -o * 2 3"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(2, 3, "*")}
            });
        }

        [Fact]
        public void OptionCanBeSpecifiedAfterPositionalArg()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o *"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(2, 3, "*")}
            });
        }

        [Fact]
        public void OptionCanBeColonSeparated()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o:*"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(2, 3, "*")}
            });
        }

        [Fact]
        public void OptionCanBeEqualsSeparated()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o=*"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(2, 3, "*")}
            });
        }

        [Fact]
        public void DoesNotModifySpecialCharactersInArguments()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {ArgsArray = new[] {"Do", "~!@#$%^&*()_= +[]\\{} |;':\",./<>?"}},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe("~!@#$%^&*()_= +[]\\{} |;':\",./<>?")
                }
            });
        }

        [Fact]
        public void BracketsShouldBeRetainedInText()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {ArgsArray = new[] {"Do", "[some (parenthesis) {curly} and [bracketed] text]"}},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe("[some (parenthesis) {curly} and [bracketed] text]")
                }
            });
        }

        [Fact(Skip = "Method params cannot be marked as required yet.  Requiredness is only possible via FluentValidator")]
        public void OperandsAreRequired()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2"},
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = {"missing argument 'Y'"}
                }
            });
        }

        [Fact]
        public void Given_OptionRequiresValue_When_NextTokenIsOption_ThrowsError()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = { Args = "Options --opt1 --opt2 go" },
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = {"Missing value for option 'opt1'"}
                }
            });
        }

        [Fact]
        public void ErrorWhenExtraValueProvidedForOption()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Add 2 3 -o * %"},
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = {"Unrecognized command or argument '%'"}
                }
            });
        }

        [Fact]
        public void Given_IgnoreExtraOperands_DisabledByAppSetting_Parse_ThrowsUnrecognized()
        {
            var results = new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "Add 2 3 4"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = {"Unrecognized command or argument '4'"}
                    }
                });

            results.CommandContext.ParseResult!.RemainingOperands.Should().BeEmpty();
        }

        [Fact]
        public void Given_IgnoreExtraOperands_DisabledByCommand_Parse_ThrowsUnrecognized()
        {
            var results = new AppRunner<App>(new AppSettings {IgnoreUnexpectedOperands = true})
                .Verify(new Scenario
                {
                    When = {Args = "Add_DisabledIgnore 2 3 4"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = {"Unrecognized command or argument '4'"}
                    }
                });

            results.CommandContext.ParseResult!.RemainingOperands.Should().BeEmpty();
        }

        [Fact]
        public void Given_IgnoreExtraOperands_EnabledByAppSettings_CollectsRemaining()
        {
            var results = new AppRunner<App>(new AppSettings {IgnoreUnexpectedOperands = true})
                .Verify(new Scenario
                {
                    When = {Args = "Add 2 3 4"},
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(2, 3, "+")}
                });

            results.CommandContext.ParseResult!.RemainingOperands.Should().BeEquivalentTo("4");
        }

        [Fact]
        public void Given_IgnoreExtraOperands_EnabledByCommand_CollectsRemaining()
        {
            var results = new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "Add_EnabledIgnore 2 3 4"},
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(2,3)}
                });

            results.CommandContext.ParseResult!.RemainingOperands.Should().BeEquivalentTo("4");
        }

        [Fact]
        public void Given_OperandValuesFollowedBySubcommands_ThrowsError()
        {
            var results = new AppRunner<DefaultApp>()
                .Verify(new Scenario
                {
                    When = {Args = "1 Do"},
                    Then =
                    {
                        AssertContext = ctx =>
                        {
                            var invocation = ctx.GetCommandInvocationInfo();
                            invocation.MethodInfo!.Name.Should().Be("Default");
                            invocation.ParameterValues.Should().BeEquivalentTo(new object?[] {"1", "Do", null, null});
                        }
                    }
                });
        }

        private class App
        {
            public void Add(
                [Operand(Description = "the first operand")] int x,
                [Operand(Description = "the second operand")] int y,
                [Option(ShortName = "o", LongName = "operator", Description = "the operation to apply")]
                string operation = "+")
            {
            }

            [Command(IgnoreUnexpectedOperands = true)]
            public void Add_EnabledIgnore(int x, int y)
            {
            }

            [Command(IgnoreUnexpectedOperands = false)]
            public void Add_DisabledIgnore(int x, int y)
            {
            }

            public void Do([Operand] string arg)
            {
            }

            public void Options([Option] string opt1, [Option] string opt2)
            {
            }

            public class AddResults
            {
                public int X { get; set; }
                public int Y { get; set; }
                public string Op { get; set; } = null!;
            }
        }

        private class DefaultApp
        {
            [DefaultMethod]
            public void Default(string opd1, string opd2, [Option] string opt1, [Option] string opt2)
            {
            }

            public void Do(string opd1, string opd2, [Option] string opt1, [Option] string opt2)
            {
            }
        }
    }
}