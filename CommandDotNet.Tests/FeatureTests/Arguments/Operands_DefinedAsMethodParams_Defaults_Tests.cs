using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_DefinedAsMethodParams_Defaults_Tests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsMethodParams_Defaults_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ArgsDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [<boolArg> <stringArg> <structArg> <structNArg> <enumArg> <objectArg> <stringListArg>]

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
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ArgsDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [<boolArg> <stringArg> <structArg> <structNArg> <enumArg> <objectArg> <stringListArg>]

Arguments:

  boolArg                   <BOOLEAN>    [True]
  Allowed values: true, false

  stringArg                 <TEXT>       [lala]

  structArg                 <NUMBER>     [3]

  structNArg                <NUMBER>     [4]

  enumArg                   <DAYOFWEEK>  [Wednesday]
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
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "StructListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListDefaults [<structListArg>]

Arguments:
  structListArg
"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "StructListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll StructListDefaults [<structListArg>]

Arguments:

  structListArg (Multiple)  <NUMBER>
"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "EnumListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListDefaults [<enumListArg>]

Arguments:
  enumListArg
"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "EnumListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll EnumListDefaults [<enumListArg>]

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
            new AppRunner<OperandsDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListDefaults [<objectListArg>]

Arguments:
  objectListArg
"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            new AppRunner<OperandsDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ObjectListDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ObjectListDefaults [<objectListArg>]

Arguments:

  objectListArg (Multiple)  <URI>
"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Positional()
        {
            new AppRunner<OperandsDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsDefaults true green 1 2 Monday http://google.com yellow orange"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        true, "green", 1, 2, DayOfWeek.Monday,
                        new Uri("http://google.com"), new List<string> {"yellow", "orange"})
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OperandsNotRequired_UsesDefaults()
        {
            new AppRunner<OperandsDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsDefaults"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                    true, "lala", 3, 4, DayOfWeek.Wednesday, null, null)
                }
            });
        }

        private class OperandsDefaults : IArgsDefaultsSampleTypesMethod
        {
            public void ArgsDefaults(
                [Operand] bool boolArg = true,
                [Operand] string stringArg = "lala", 
                [Operand] int structArg = 3, 
                [Operand] int? structNArg = 4,
                [Operand] DayOfWeek enumArg = DayOfWeek.Wednesday, 
                [Operand] Uri? objectArg = null,
                [Operand] List<string>? stringListArg = null)
            {
            }

            public void StructListDefaults(
                [Operand] List<int>? structListArg = null)
            {
            }

            public void EnumListDefaults(
                [Operand] List<DayOfWeek>? enumListArg = null)
            {
            }

            public void ObjectListDefaults(
                [Operand] List<Uri>? objectListArg = null)
            {
            }
        }
    }
}
