using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_DefinedAsArgModel_Defaults : TestBase
    {
        private static AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Operands_DefinedAsArgModel_Defaults(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SampleTypes_BasicHelp_IncludesAll()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "ArgsDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [arguments] [options]

Arguments:
  BoolArg
  StringArg
  StructArg
  StructNArg
  EnumArg
  ObjectArg
  StringListArg

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp_IncludesAll()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "ArgsDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ArgsDefaults [arguments] [options]

Arguments:

  BoolArg                     <BOOLEAN>      [True]
  Allowed values: true, false

  StringArg                   <TEXT>         [lala]

  StructArg                   <NUMBER>       [3]

  StructNArg                  <NUMBER>       [4]

  EnumArg                     <DAYOFWEEK>    [Tuesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  ObjectArg                   <URI>          [http://google.com/]

  StringListArg (Multiple)    <TEXT>         [blue,red]


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void StructList_BasicHelp_IncludesList()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "StructListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll StructListDefaults [arguments] [options]

Arguments:
  StructListArg

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void StructList_DetailedHelp_IncludesList()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "StructListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll StructListDefaults [arguments] [options]

Arguments:

  StructListArg (Multiple)    <NUMBER>    [3,4]


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void EnumList_BasicHelp_IncludesList()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "EnumListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll EnumListDefaults [arguments] [options]

Arguments:
  EnumListArg

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp_IncludesList()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "EnumListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll EnumListDefaults [arguments] [options]

Arguments:

  EnumListArg (Multiple)    <DAYOFWEEK>    [Monday,Tuesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp_IncludesList()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "ObjectListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments] [options]

Arguments:
  ObjectListArg

Options:
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp_IncludesList()
        {
            Verify(new Given<OperandsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "ObjectListDefaults -h",
                Then = { Result = @"Usage: dotnet testhost.dll ObjectListDefaults [arguments] [options]

Arguments:

  ObjectListArg (Multiple)    <URI>    [http://github.com/,http://google.com/]


Options:

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OperandsAreAssignedByPosition()
        {
            Verify(new Given<OperandsDefaults>
            {
                WhenArgs = "ArgsDefaults true green 1 2 Monday http://google.com yellow orange",
                Then =
                {
                    Outputs = { new OperandsDefaultsSampleTypesModel
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
        public void SampleType_Exec_OperandsAreNotRequired_UsesDefaults()
        {
            Verify(new Given<OperandsDefaults>
            {
                WhenArgs = "ArgsDefaults",
                Then =
                {
                    Outputs =
                    {
                        new OperandsDefaultsSampleTypesModel
                        {
                            StringArg = "lala",
                            StructArg = 3,
                            StructNArg = 4,
                            EnumArg = DayOfWeek.Tuesday,
                        }
                    }
                }
            });
        }

        private class OperandsDefaults
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void ArgsDefaults(OperandsDefaultsSampleTypesModel model)
            {
                TestOutputs.Capture(model);
            }

            public void StructListDefaults(OperandsDefaultsStructListArgumentModel model)
            {
                TestOutputs.Capture(model);
            }

            public void EnumListDefaults(OperandsDefaultsEnumListArgumentModel model)
            {
                TestOutputs.Capture(model);
            }

            public void ObjectListDefaults(OperandsDefaultsObjectListArgumentModel model)
            {
                TestOutputs.Capture(model);
            }
        }
    }
}