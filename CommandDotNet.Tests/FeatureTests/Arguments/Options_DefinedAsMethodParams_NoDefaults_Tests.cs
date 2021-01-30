using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_DefinedAsMethodParams_NoDefaults_Tests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Options_DefinedAsMethodParams_NoDefaults_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            new AppRunner<OptionsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsNoDefault [options]

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
            new AppRunner<OptionsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsNoDefault [options]

Options:

  --boolArg

  --stringArg                 <TEXT>

  --structArg                 <NUMBER>

  --structNArg                <NUMBER>

  --enumArg                   <DAYOFWEEK>
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
            new AppRunner<OptionsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "StructListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListNoDefault [options]

Options:
  --structListArg
"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            new AppRunner<OptionsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "StructListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListNoDefault [options]

Options:

  --structListArg (Multiple)  <NUMBER>
"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            new AppRunner<OptionsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "EnumListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListNoDefault [options]

Options:
  --enumListArg
"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            new AppRunner<OptionsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "EnumListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListNoDefault [options]

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
            new AppRunner<OptionsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListNoDefault [options]

Options:
  --objectListArg
"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            new AppRunner<OptionsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListNoDefault [options]

Options:

  --objectListArg (Multiple)  <URI>
"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Named()
        {
            new AppRunner<OptionsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault --boolArg --stringArg green --structArg 1 --structNArg 2 --enumArg Monday " +
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
        public void SampleTypes_Exec_OperandsNotRequired()
        {
            new AppRunner<OptionsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(null, null, null, null, null, null, null)
                }
            });
        }

        [Fact]
        public void StructList_Exec_Named()
        {
            new AppRunner<OptionsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "StructListNoDefault --structListArg 23 --structListArg 5 --structListArg 7"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(new List<int>{23,5,7})
                }
            });
        }

        [Fact]
        public void EnumList_Exec_Named()
        {
            new AppRunner<OptionsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "EnumListNoDefault --enumListArg Friday --enumListArg Tuesday --enumListArg Thursday"},
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
            new AppRunner<OptionsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ObjectListNoDefault --objectListArg http://google.com --objectListArg http://apple.com --objectListArg http://github.com"},
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

        private class OptionsNoDefaults : IArgsNoDefaultsSampleTypesMethod
        {
            public void ArgsNoDefault(
                [Option] bool boolArg,
                [Option] string stringArg,
                [Option] int structArg,
                [Option] int? structNArg,
                [Option] DayOfWeek enumArg,
                [Option] Uri objectArg,
                [Option] List<string> stringListArg)
            {
            }

            public void StructListNoDefault(
                [Option] List<int> structListArg)
            {
            }

            public void EnumListNoDefault(
                [Option] List<DayOfWeek> enumListArg)
            {
            }

            public void ObjectListNoDefault(
                [Option] List<Uri> objectListArg)
            {
            }
        }
    }
}
