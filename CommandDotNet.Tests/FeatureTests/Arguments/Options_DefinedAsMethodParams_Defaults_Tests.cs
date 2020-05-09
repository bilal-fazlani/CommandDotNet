using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_DefinedAsMethodParams_Defaults_Tests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Options_DefinedAsMethodParams_Defaults_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            new AppRunner<OptionsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ArgsDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:
  --boolArg
  --stringArg
  --structArg
  --structNArg
  --enumArg
  --objectArg
  --stringListArg
"
                }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp()
        {
            new AppRunner<OptionsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ArgsDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:

  --boolArg                                [True]

  --stringArg                 <TEXT>       [lala]

  --structArg                 <NUMBER>     [3]

  --structNArg                <NUMBER>     [4]

  --enumArg                   <DAYOFWEEK>  [Wednesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --objectArg                 <URI>

  --stringListArg (Multiple)  <TEXT>
"
                }
            });
        }

        [Fact]
        public void StructList_BasicHelp()
        {
            new AppRunner<OptionsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "StructListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListDefaults [options]

Options:
  --structListArg
"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            new AppRunner<OptionsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "StructListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListDefaults [options]

Options:

  --structListArg (Multiple)  <NUMBER>
"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            new AppRunner<OptionsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "EnumListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListDefaults [options]

Options:
  --enumListArg
"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            new AppRunner<OptionsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "EnumListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListDefaults [options]

Options:

  --enumListArg (Multiple)  <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
"
                }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp()
        {
            new AppRunner<OptionsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListDefaults [options]

Options:
  --objectListArg
"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            new AppRunner<OptionsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListDefaults [options]

Options:

  --objectListArg (Multiple)  <URI>
"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Named()
        {
            new AppRunner<OptionsDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsDefaults --stringArg green --structArg 1 --structNArg 2 --enumArg Monday " +
                           "--objectArg http://google.com --stringListArg yellow --stringListArg orange"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        true, "green", 1, 2, DayOfWeek.Monday,
                        new Uri("http://google.com"), new List<string> {"yellow", "orange"})
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OptionsNotRequired_UsesDefaults()
        {
            new AppRunner<OptionsDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsDefaults"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        true, "lala", 3, 4, DayOfWeek.Wednesday, null, null)
                }
            });
        }

        [Fact]
        public void StructList_Exec_Named()
        {
            new AppRunner<OptionsDefaults>().Verify(new Scenario
            {
                When = {Args = "StructListDefaults --structListArg 23 --structListArg 5 --structListArg 7"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(new List<int>{23,5,7})
                }
            });
        }

        [Fact]
        public void EnumList_Exec_Named()
        {
            new AppRunner<OptionsDefaults>().Verify(new Scenario
            {
                When = {Args = "EnumListDefaults --enumListArg Friday --enumListArg Tuesday --enumListArg Thursday"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new List<DayOfWeek>{DayOfWeek.Friday, DayOfWeek.Tuesday, DayOfWeek.Thursday})
                }
            });
        }

        [Fact]
        public void ObjectList_Exec_Named()
        {
            new AppRunner<OptionsDefaults>().Verify(new Scenario
            {
                When = {Args = "ObjectListDefaults --objectListArg http://google.com --objectListArg http://apple.com --objectListArg http://github.com"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new List<Uri>
                        {
                            new Uri("http://google.com"),
                            new Uri("http://apple.com"),
                            new Uri("http://github.com"),
                        })
                }
            });
        }

        private class OptionsDefaults : IArgsDefaultsSampleTypesMethod
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
        }
    }
}
