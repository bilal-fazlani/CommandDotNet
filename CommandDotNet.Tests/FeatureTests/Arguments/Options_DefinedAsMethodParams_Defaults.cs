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
    public class Options_DefinedAsMethodParams_Defaults : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Options_DefinedAsMethodParams_Defaults(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            Verify(new Given<OptionsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "ArgsDefaults -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:
  --boolArg
  --stringArg
  --structArg
  --structNArg
  --enumArg
  --objectArg
  --stringListArg
  -h | --help      Show help information"
                }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp()
        {
            Verify(new Given<OptionsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "ArgsDefaults -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:

  --boolArg                                    [True]

  --stringArg                   <TEXT>         [lala]

  --structArg                   <NUMBER>       [3]

  --structNArg                  <NUMBER>       [4]

  --enumArg                     <DAYOFWEEK>    [Wednesday]
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --objectArg                   <URI>

  --stringListArg (Multiple)    <TEXT>

  -h | --help
  Show help information"
                }
            });
        }

        [Fact]
        public void StructList_BasicHelp()
        {
            Verify(new Given<OptionsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "StructListDefaults -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll StructListDefaults [options]

Options:
  --structListArg
  -h | --help      Show help information"
                }
            });
        }

        [Fact]
        public void StructList_DetailedHelp()
        {
            Verify(new Given<OptionsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "StructListDefaults -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll StructListDefaults [options]

Options:

  --structListArg (Multiple)    <NUMBER>

  -h | --help
  Show help information"
                }
            });
        }

        [Fact]
        public void EnumList_BasicHelp()
        {
            Verify(new Given<OptionsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "EnumListDefaults -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll EnumListDefaults [options]

Options:
  --enumListArg
  -h | --help    Show help information"
                }
            });
        }

        [Fact]
        public void EnumList_DetailedHelp()
        {
            Verify(new Given<OptionsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "EnumListDefaults -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll EnumListDefaults [options]

Options:

  --enumListArg (Multiple)    <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  -h | --help
  Show help information"
                }
            });
        }

        [Fact]
        public void ObjectList_BasicHelp()
        {
            Verify(new Given<OptionsDefaults>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "ObjectListDefaults -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ObjectListDefaults [options]

Options:
  --objectListArg
  -h | --help      Show help information"
                }
            });
        }

        [Fact]
        public void ObjectList_DetailedHelp()
        {
            Verify(new Given<OptionsDefaults>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "ObjectListDefaults -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ObjectListDefaults [options]

Options:

  --objectListArg (Multiple)    <URI>

  -h | --help
  Show help information"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Named()
        {
            Verify(new Given<OptionsDefaults>
            {
                WhenArgs = "ArgsDefaults --stringArg green --structArg 1 --structNArg 2 --enumArg Monday " +
                           "--objectArg http://google.com --stringListArg yellow --stringListArg orange",
                Then =
                {
                    Outputs =
                    {
                        new ParametersSampleTypesResults
                        {
                            BoolArg = true,
                            StringArg = "green",
                            StructArg = 1,
                            StructNArg = 2,
                            EnumArg = DayOfWeek.Monday,
                            ObjectArg = new Uri("http://google.com"),
                            StringListArg = new List<string> {"yellow", "orange"}
                        }
                    }
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OptionsNotRequired_UsesDefaults()
        {
            Verify(new Given<OptionsDefaults>
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

        [Fact]
        public void StructList_Exec_Named()
        {
            Verify(new Given<OptionsDefaults>
            {
                WhenArgs = "StructListDefaults --structListArg 23 --structListArg 5 --structListArg 7",
                Then =
                {
                    Outputs =
                    {
                        new ParametersSampleTypesResults
                        {
                            StructListArg = new List<int>{23,5,7}
                        }
                    }
                }
            });
        }

        [Fact]
        public void EnumList_Exec_Named()
        {
            Verify(new Given<OptionsDefaults>
            {
                WhenArgs = "EnumListDefaults --enumListArg Friday --enumListArg Tuesday --enumListArg Thursday",
                Then =
                {
                    Outputs =
                    {
                        new ParametersSampleTypesResults
                        {
                            EnumListArg = new List<DayOfWeek>{DayOfWeek.Friday, DayOfWeek.Tuesday, DayOfWeek.Thursday}
                        }
                    }
                }
            });
        }

        [Fact]
        public void ObjectList_Exec_Named()
        {
            Verify(new Given<OptionsDefaults>
            {
                WhenArgs = "ObjectListDefaults --objectListArg http://google.com --objectListArg http://apple.com --objectListArg http://github.com",
                Then =
                {
                    Outputs =
                    {
                        new ParametersSampleTypesResults
                        {
                            ObjectListArg = new List<Uri>
                            {
                                new Uri("http://google.com"),
                                new Uri("http://apple.com"),
                                new Uri("http://github.com"),
                            }
                        }
                    }
                }
            });
        }

        private class OptionsDefaults : IArgsDefaultsSampleTypesMethod
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void ArgsDefaults(
                [Option] bool boolArg = true,
                [Option] string stringArg = "lala", 
                [Option] int structArg = 3, 
                [Option] int? structNArg = 4,
                [Option] DayOfWeek enumArg = DayOfWeek.Wednesday, 
                [Option] Uri objectArg = null,
                [Option] List<string> stringListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(
                    boolArg, stringArg, structArg, structNArg, enumArg, objectArg, stringListArg));
            }

            public void StructListDefaults(
                [Option] List<int> structListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(structListArg));
            }

            public void EnumListDefaults(
                [Option] List<DayOfWeek> enumListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(enumListArg));
            }

            public void ObjectListDefaults(
                [Option] List<Uri> objectListArg = null)
            {
                TestOutputs.Capture(new ParametersSampleTypesResults(objectListArg));
            }
        }
    }
}