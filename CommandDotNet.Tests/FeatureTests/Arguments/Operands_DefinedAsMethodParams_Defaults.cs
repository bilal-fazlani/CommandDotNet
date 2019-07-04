using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.FeatureTests.Arguments.Models;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_DefinedAsMethodParams_Defaults : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsMethodParams_Defaults(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "ArgsDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [arguments] [options]

Arguments:
  boolArg
  stringArg
  structArg
  structNArg
  enumArg
  objectArg
  stringListArg

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "ArgsDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [arguments] [options]

Arguments:

  boolArg                     <BOOLEAN>      [True]
  Allowed values: true, false

  stringArg                   <TEXT>         [lala]

  structArg                   <NUMBER>       [3]

  structNArg                  <NUMBER>       [4]

  enumArg                     <DAYOFWEEK>    [Wednesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  objectArg                   <URI>

  stringListArg (Multiple)    <TEXT>


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void StructList_BasicHelp()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "StructListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll StructListDefaults [arguments] [options]

Arguments:
  structListArg

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "StructListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll StructListDefaults [arguments] [options]

Arguments:

  structListArg (Multiple)    <NUMBER>


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "EnumListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll EnumListDefaults [arguments] [options]

Arguments:
  enumListArg

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "EnumListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll EnumListDefaults [arguments] [options]

Arguments:

  enumListArg (Multiple)    <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "ObjectListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments] [options]

Arguments:
  objectListArg

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "ObjectListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments] [options]

Arguments:

  objectListArg (Multiple)    <URI>


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Positional()
        {
            Verify(new Given<OperandsDefaults>
            {
                WhenArgs = "ArgsDefaults true green 1 2 Monday http://google.com yellow orange",
                Then =
                {
                    Outputs = { new ParametersSampleTypesResults
                    {
                        BoolArg = true,
                        StringArg = "green",
                        StructArg = 1,
                        StructNArg = 2,
                        EnumArg = DayOfWeek.Monday,
                        ObjectArg = new Uri("http://google.com"),
                        StringListArg = new List<string>{"yellow", "orange"}
                    } }
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OperandsNotRequired_UsesDefaults()
        {
            Verify(new Given<OperandsDefaults>
            {
                WhenArgs = "ArgsDefaults",
                Then =
                {
                    Outputs =
                    {
                        new ParametersSampleTypesResults
                        {
                            BoolArg = true,
                            StringArg = "lala",
                            StructArg = 3,
                            StructNArg = 4,
                            EnumArg = DayOfWeek.Wednesday,
                        }
                    }
                }
            });
        }

        private class OperandsDefaults : IArgsDefaultsSampleTypesMethod
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void ArgsDefaults(
                [Operand] bool boolArg = true,
                [Operand] string stringArg = "lala", 
                [Operand] int structArg = 3, 
                [Operand] int? structNArg = 4,
                [Operand] DayOfWeek enumArg = DayOfWeek.Wednesday, 
                [Operand] Uri objectArg = null,
                [Operand] List<string> stringListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(
                    boolArg, stringArg, structArg, structNArg, enumArg, objectArg, stringListArg));
            }

            public void StructListDefaults(
                [Operand] List<int> structListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(structListArg));
            }

            public void EnumListDefaults(
                [Operand] List<DayOfWeek> enumListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(enumListArg));
            }

            public void ObjectListDefaults(
                [Operand] List<Uri> objectListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(objectListArg));
            }
        }
    }
}