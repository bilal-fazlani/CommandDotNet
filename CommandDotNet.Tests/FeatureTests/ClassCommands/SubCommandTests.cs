using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class SubCommandTests
    {
        public SubCommandTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void CanDiscoverAppsAtAllLevels()
        {
            var classNames = new AppRunner<ThreeLevelsApp>()
                .GetCommandClassTypes()
                .Select(t => t.Name)
                .ToList();
            classNames.Count.Should().Be(3);
            classNames.Should().ContainEquivalentOf(nameof(ThreeLevelsApp), nameof(Second), nameof(Third));

            classNames = new AppRunner<NestedThreeLevelsApp>()
                .GetCommandClassTypes()
                .Select(t => t.Name)
                .ToList();
            classNames.Count.Should().Be(3);
            classNames.Should().ContainEquivalentOf(nameof(NestedThreeLevelsApp), nameof(Second), nameof(Third));
        }
        
        public static IEnumerable<object[]> AppTypes => new[]
        {
            typeof(ThreeLevelsApp), 
            typeof(NestedThreeLevelsApp)
        }.ToObjectArrays();

        [Theory]
        [MemberData(nameof(AppTypes))]
        public void Help_NoArgs_Includes_1stLevelCommands_And_2ndLevelApp(Type type)
        {
            // implicit help

            new AppRunner(type)
                .Verify(new Scenario
                {
                    When = { Args = null },
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll [command]

Commands:

  Do1
  Second

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                    }
                });
        }

        [Theory]
        [MemberData(nameof(AppTypes))]
        public void Help_1stLevelApp_Includes_1stLevelCommands_And_2ndLevelApp(Type type)
        {
            new AppRunner(type)
                .Verify(new Scenario
                {
                    When = { Args = "-h" },
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll [command]

Commands:

  Do1
  Second

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                    }
                });
        }

        [Theory]
        [MemberData(nameof(AppTypes))]
        public void Help_2rdLevelApp_Includes_2ndLevelCommands_And_3rdLevelApp(Type type)
        {
            new AppRunner(type)
                .Verify(new Scenario
                {
                    When = {Args = "Second -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Second [command]

Commands:

  Do2
  Third

Use ""dotnet testhost.dll Second [command] --help"" for more information about a command.
"
                    }
                });
        }

        [Theory]
        [MemberData(nameof(AppTypes))]
        public void Help_3rdLevelApp_Includes_3rdLevelCommands(Type type)
        {
            new AppRunner(type)
                .Verify(new Scenario
                {
                    When = {Args = "Second Third -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Second Third [command]

Commands:

  Do3

Use ""dotnet testhost.dll Second Third [command] --help"" for more information about a command.
"
                    }
                });
        }

        [Theory]
        [MemberData(nameof(AppTypes))]
        public void CanExecute_1stLevel_LocalCommand(Type type)
        {
            new AppRunner(type)
                .Verify(new Scenario
                {
                    When = {Args = "Do1 --Opt1 1111 somearg"},
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new ArgModel1 {Opt1 = "1111", Arg1 = "somearg"})
                    }
                });
        }

        [Theory]
        [MemberData(nameof(AppTypes))]
        public void CanExecute_2ndLevel_LocalCommand(Type type)
        {
            new AppRunner(type)
                .Verify(new Scenario
                {
                    When = {Args = "Second Do2 --Opt2 1111 somearg"},
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new ArgModel2 {Opt2 = "1111", Arg2 = "somearg"})
                    }
                });
        }

        [Theory]
        [MemberData(nameof(AppTypes))]
        public void CanExecute_3rdLevel_LocalCommand(Type type)
        {
            new AppRunner(type)
                .Verify(new Scenario
                {
                    When = {Args = "Second Third Do3 --Opt3 1111 somearg"},
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new ArgModel3 {Opt3 = "1111", Arg3 = "somearg"})
                    }
                });
        }

        private class ThreeLevelsApp
        {
            [SubCommand]
            public Second Second { get; set; } = null!;

            public void Do1(ArgModel1 model)
            {
            }
        }

        private class Second
        {
            [SubCommand]
            public Third Third { get; set; } = null!;

            public void Do2(ArgModel2 model)
            {
            }
        }

        private class Third
        {
            public void Do3(ArgModel3 model)
            {
            }
        }

        private class NestedThreeLevelsApp
        {
            public void Do1(ArgModel1 model)
            {
            }

            [SubCommand]
            public class Second
            {
                public void Do2(ArgModel2 model)
                {
                }

                [SubCommand]
                public class Third
                {
                    public void Do3(ArgModel3 model)
                    {
                    }
                }
            }
        }

        private class ArgModel1 : IArgumentModel
        {
            [Option]
            public string? Opt1 { get; set; }
            [Operand]
            public string? Arg1 { get; set; }
        }

        private class ArgModel2 : IArgumentModel
        {
            [Option]
            public string? Opt2 { get; set; }
            [Operand]
            public string? Arg2 { get; set; }
        }

        private class ArgModel3 : IArgumentModel
        {
            [Option]
            public string? Opt3 { get; set; }
            [Operand]
            public string? Arg3 { get; set; }
        }
    }
}