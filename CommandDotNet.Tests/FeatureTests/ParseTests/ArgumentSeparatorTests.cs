using CommandDotNet.ConsoleOnly;
using CommandDotNet.Execution;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseTests
{
    public class ArgumentSeparatorTests
    {
        private readonly AppSettings _endOfOptionsSettings = new AppSettings
        {
            DefaultArgumentSeparatorStrategy = ArgumentSeparatorStrategy.EndOfOptions
        };

        private readonly AppSettings _passThruSettings = new AppSettings
        {
            DefaultArgumentSeparatorStrategy = ArgumentSeparatorStrategy.PassThru
        };

        public ArgumentSeparatorTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [InlineData(ArgumentSeparatorStrategy.EndOfOptions, "UseAppSettings", ArgumentSeparatorStrategy.EndOfOptions)]
        [InlineData(ArgumentSeparatorStrategy.EndOfOptions, "EndOfOptions", ArgumentSeparatorStrategy.EndOfOptions)]
        [InlineData(ArgumentSeparatorStrategy.EndOfOptions, "PassThru", ArgumentSeparatorStrategy.PassThru)]
        [InlineData(ArgumentSeparatorStrategy.PassThru, "UseAppSettings", ArgumentSeparatorStrategy.PassThru)]
        [InlineData(ArgumentSeparatorStrategy.PassThru, "EndOfOptions", ArgumentSeparatorStrategy.EndOfOptions)]
        [InlineData(ArgumentSeparatorStrategy.PassThru, "PassThru", ArgumentSeparatorStrategy.PassThru)]
        [Theory]
        public void ArgumentSeparatorStrategy_UseCommandAttribute_DefaultFromAppSettings(
            ArgumentSeparatorStrategy appSettingsStrategy, string command, ArgumentSeparatorStrategy expectedStrategy)
        {
            var appSettings = new AppSettings {DefaultArgumentSeparatorStrategy = appSettingsStrategy};
            new AppRunner<SettingsApp>(appSettings)
                .StopAfter(MiddlewareStages.ParseInput)
                .RunInMem(command)
                .CommandContext.ParseResult!.TargetCommand.ArgumentSeparatorStrategy!.Should().Be(expectedStrategy);
        }
        
        [Fact]
        public void Given_PassThru_When_OperandValueWithDash_FailsWithUnrecognizedOption()
        {
            new AppRunner<Math>(_passThruSettings)
                .Verify(new Scenario
                {
                    When = {Args = "Concat -a -b"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { "Unrecognized option '-a'" }
                    }
                });
        }

        [Fact]
        public void Given_EndOfOptions_When_OperandValueWithDash_FailsWithUnrecognizedOption()
        {
            new AppRunner<Math>(_endOfOptionsSettings)
                .Verify(new Scenario
                {
                    When = {Args = "Concat -a -b"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { "Unrecognized option '-a'" }
                    }
                });
        }
        
        [Fact]
        public void Given_PassThru_When_Separator_OperandValueWithDash_OperandsAreIgnored()
        {
            var result = new AppRunner<Math>(_passThruSettings)
                .Verify(new Scenario
                {
                    When = {Args = "Concat -- -a -b"},
                    Then = { Output = "" }
                });

            result.CommandContext.ParseResult!.SeparatedArguments
                .Should().BeEquivalentTo("-a", "-b");
        }

        [Fact]
        public void Given_EndOfOptions_When_Separator_OperandValueWithDash_OperandsAreParsed()
        {
            var result = new AppRunner<Math>(_endOfOptionsSettings)
                .Verify(new Scenario
                {
                    When = {Args = "Concat -- -a -b"},
                    Then = { Output = "-a-b" }
                });

            result.CommandContext.ParseResult!.SeparatedArguments
                .Should().BeEquivalentTo("-a", "-b");
        }

        [Fact]
        public void Given_EndOfOptions_And_IgnoreUnexpected_Enabled_When_Separator_OperandValueWithDash_OperandsAreParsed_And_ExtraArgsAreIgnoredAndCaptured()
        {
            var appSettings = _endOfOptionsSettings.Clone(s => s.IgnoreUnexpectedOperands = true);

            var result = new AppRunner<Math>(appSettings)
                .Verify(new Scenario
                {
                    When = {Args = "Concat -- -a -b -c -d"},
                    Then = { Output = "-a-b" }
                });

            result.CommandContext.ParseResult!.RemainingOperands
                .Should().BeEquivalentTo("-c", "-d");

            result.CommandContext.ParseResult!.SeparatedArguments
                .Should().BeEquivalentTo("-a", "-b", "-c", "-d");
        }

        [Fact]
        public void CmdDotNet_Examples_Alternative2_IsValid()
        {
            // test examples in the help documentation

            var appSettings = _endOfOptionsSettings.Clone(s => s.IgnoreUnexpectedOperands = true);

            var result = new AppRunner<Math>(appSettings)
                .Verify(new Scenario
                {
                    When = {Args = "Concat -- -a -b -- -c -d"},
                    Then = { Output = "-a-b" }
                });

            result.CommandContext.ParseResult!.RemainingOperands
                .Should().BeEquivalentTo("--", "-c", "-d");

            result.CommandContext.ParseResult!.SeparatedArguments
                .Should().BeEquivalentTo("-a", "-b", "--", "-c", "-d");
        }

        [Fact]
        public void CmdDotNet_Examples_Alternative3_IsValid()
        {
            // test examples in the help documentation

            var appSettings = _endOfOptionsSettings.Clone(s => s.IgnoreUnexpectedOperands = true);

            var result = new AppRunner<Math>(appSettings)
                .Verify(new Scenario
                {
                    When = {Args = "Concat -- -a -b __ -c -d" },
                    Then = { Output = "-a-b" }
                });

            result.CommandContext.ParseResult!.RemainingOperands
                .Should().BeEquivalentTo("__", "-c", "-d");

            result.CommandContext.ParseResult!.SeparatedArguments
                .Should().BeEquivalentTo("-a", "-b", "__", "-c", "-d");
        }

        public class Math
        {
            public void Concat(IConsole console, string x, string y)
            {
                console.Write(x + y);
            }
        }

        public class SettingsApp
        {
            public void UseAppSettings()
            {
            }

            [Command(ArgumentSeparatorStrategy = ArgumentSeparatorStrategy.EndOfOptions)]
            public void EndOfOptions()
            {
            }

            [Command(ArgumentSeparatorStrategy = ArgumentSeparatorStrategy.PassThru)]
            public void PassThru()
            {
            }
        }
    }
}
