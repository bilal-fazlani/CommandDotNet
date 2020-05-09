using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.EnabledMiddlewareScenarios
{
    public class OverridingDefaultValuesTests
    {
        public OverridingDefaultValuesTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void DefaultValuesShouldBeOverrideable()
        {
            var overrides = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["stringArg"] = "green",
                ["structArg"] = 1,
                ["structNArg"] = 2,
                ["enumArg"] = DayOfWeek.Monday,
                ["objectArg"] = new Uri("http://google.com"),
                ["stringListArg"] = new List<string> { "yellow", "orange" }
            };
            new AppRunner<App>()
                .SetDefaults(overrides)
                .Verify(new Scenario
                {
                    When = {Args = "ArgsDefaults"},
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(
                            true, "green", 1, 2, DayOfWeek.Monday, 
                            new Uri("http://google.com"), new List<string> {"yellow", "orange"})
                    }
                });
            new AppRunner<App>()
                .SetDefaults(overrides)
                .Verify(new Scenario
                {
                    When = {Args = "OperandsDefaultsModel"},
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(
                            new OperandsDefaultsSampleTypesModel
                            {
                                BoolArg = true,
                                StringArg = "green",
                                StructArg = 1,
                                StructNArg = 2,
                                EnumArg = DayOfWeek.Monday,
                                ObjectArg = new Uri("http://google.com"),
                                StringListArg = new List<string> {"yellow", "orange"}
                            })
                    }
                });
        }

        [Fact]
        public void DefaultValuesCanBeParsedFromString()
        {
            var overrides = new Dictionary<string, object>()
            {
                ["stringArg"] = "green",
                ["structArg"] = 1,
                ["structNArg"] = 2,
                ["enumArg"] = "Monday",
                ["objectArg"] = "http://google.com",
                ["stringListArg"] = new List<string>{ "yellow","orange" },
                ["enumListArg"] = new List<string>{ "Monday", "Friday" },
                ["objectListArg"] = new List<string> { "http://www.google.com", "http://www.apple.com" }
            };
            new AppRunner<App>()
                .SetDefaults(overrides)
                .Verify(
                    new Scenario
                    {
                        When = {Args = "ArgsDefaults"},
                        Then =
                        {
                            AssertContext = ctx => ctx.ParamValuesShouldBe(
                                true, "green", 1, 2, DayOfWeek.Monday,
                                new Uri("http://google.com"), new List<string> {"yellow", "orange"})
                        }
                    });
            new AppRunner<App>()
                .SetDefaults(overrides)
                .Verify(new Scenario
                {
                    When = {Args = "EnumListDefaults"},
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(
                            new List<DayOfWeek> {DayOfWeek.Monday, DayOfWeek.Friday})
                    }
                });
            new AppRunner<App>()
                .SetDefaults(overrides)
                .Verify(new Scenario
                {
                    When = {Args = "ObjectListDefaults"},
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(
                            new List<Uri>
                            {
                                new Uri("http://www.google.com"),
                                new Uri("http://www.apple.com")
                            }),
                    }
                });
        }

        [Fact]
        public void DefaultValuesCanBeCoerced()
        {
            // for cases where an int is provided but the argument type is actually a long
            var overrides = new Dictionary<string, object>
            {
                ["StructArg"] = (long)1,
            };
            new AppRunner<App>()
                .SetDefaults(overrides)
                .Verify(new Scenario
                {
                    When = {Args = "OptionsDefaultsModel"},
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new OptionsDefaultsSampleTypesModel{StructArg = 1})
                    }
                });
        }

        [Fact]
        public void DefaultValuesFailureIncludesArgumentInfo()
        {
            // for cases where an int is provided but the argument type is actually a long
            var overrides = new Dictionary<string, object>
            {
                ["StructNArg"] = (long)1,
            };
            new AppRunner<App>()
                .SetDefaults(overrides)
                .Verify(new Scenario
                {
                    When = {Args = "OptionsDefaultsModel"},
                    Then =
                    {
                        ExitCode = 2,
                        OutputContainsTexts =
                        {
                            "Failure assigning value to Option: StructNArg",
                            "CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels.OptionsDefaultsSampleTypesModel.StructNArg",
                            "Invalid cast from 'System.Int64' to 'System.Nullable`1"
                        }
                    }
                });
        }

        class App : IArgsDefaultsSampleTypesMethod
        {
            public void ArgsDefaults(
                [Option] bool boolArg = true,
                [Option] string stringArg = "lala",
                [Option] int structArg = 3,
                [Option] int? structNArg = 4,
                [Option] DayOfWeek enumArg = DayOfWeek.Wednesday,
                [Option] Uri? objectArg = null,
                [Option] List<string>? stringListArg = null)
            {
            }

            public void StructListDefaults(
                [Option] List<int>? structListArg = null)
            {
            }

            public void EnumListDefaults(
                [Option] List<DayOfWeek>? enumListArg = null)
            {
            }

            public void ObjectListDefaults(
                [Option] List<Uri>? objectListArg = null)
            {
            }

            public void OperandsDefaultsModel(OperandsDefaultsSampleTypesModel model)
            {
            }

            public void OptionsDefaultsModel(OptionsDefaultsSampleTypesModel model)
            {
            }
        }
    }
}
