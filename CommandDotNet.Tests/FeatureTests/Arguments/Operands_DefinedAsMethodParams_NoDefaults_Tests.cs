using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_DefinedAsMethodParams_NoDefaults_Tests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsMethodParams_NoDefaults_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsNoDefault <boolArg> <stringArg> <structArg> <structNArg> <enumArg> <objectArg> <stringListArg>

Arguments:
  boolArg
  stringArg
  structArg
  structNArg
  enumArg
  objectArg
  stringListArg
"
                }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsNoDefault <boolArg> <stringArg> <structArg> <structNArg> <enumArg> <objectArg> <stringListArg>

Arguments:

  boolArg                   <BOOLEAN>
  Allowed values: true, false

  stringArg                 <TEXT>

  structArg                 <NUMBER>

  structNArg                <NUMBER>

  enumArg                   <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  objectArg                 <URI>

  stringListArg (Multiple)  <TEXT>
"
                }
            });
        }

        [Fact]
        public void StructList_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "StructListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListNoDefault <structListArg>

Arguments:
  structListArg
"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "StructListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListNoDefault <structListArg>

Arguments:

  structListArg (Multiple)  <NUMBER>
"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "EnumListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListNoDefault <enumListArg>

Arguments:
  enumListArg
"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "EnumListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListNoDefault <enumListArg>

Arguments:

  enumListArg (Multiple)  <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
"
                }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp()
        {
            new AppRunner<OperandsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListNoDefault <objectListArg>

Arguments:
  objectListArg
"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            new AppRunner<OperandsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListNoDefault -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListNoDefault <objectListArg>

Arguments:

  objectListArg (Multiple)  <URI>
"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault true green 1 2 Monday http://google.com yellow orange"},
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
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsNoDefault"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(null, null, null, null, null, null, null)
                }
            });
        }

        [Fact]
        public void StructList_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "StructListNoDefault 23 5 7"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(new List<int>{23,5,7})
                }
            });
        }

        [Fact]
        public void EnumList_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "EnumListNoDefault Friday Tuesday Thursday"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new List<DayOfWeek>{DayOfWeek.Friday, DayOfWeek.Tuesday, DayOfWeek.Thursday})
                }
            });
        }

        [Fact]
        public void ObjectList_Exec_Positional()
        {
            new AppRunner<OperandsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ObjectListNoDefault http://google.com http://apple.com http://github.com"},
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

        private class OperandsNoDefaults: IArgsNoDefaultsSampleTypesMethod
        {
            public void ArgsNoDefault(
                [Operand] bool boolArg,
                [Operand] string stringArg,
                [Operand] int structArg,
                [Operand] int? structNArg,
                [Operand] DayOfWeek enumArg,
                [Operand] Uri objectArg,
                [Operand] List<string> stringListArg)
            {
            }

            public void StructListNoDefault(
                [Operand] List<int> structListArg)
            {
            }

            public void EnumListNoDefault(
                [Operand] List<DayOfWeek> enumListArg)
            {
            }

            public void ObjectListNoDefault(
                [Operand] List<Uri> objectListArg)
            {
            }
        }
    }
}
