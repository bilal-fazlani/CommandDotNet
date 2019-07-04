using System.Collections.Generic;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class MixedDefinedAs_MethodParamsAndArgModel : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public MixedDefinedAs_MethodParamsAndArgModel(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void BasicHelp_IncludesModelAndParamDefinedArgs()
        {
            Verify(new Given<App>
            {
                And = {AppSettings = BasicHelp},
                WhenArgs = "Do -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  ModelArg
  paramArg
  paramArgList

Options:
  --ModelOption
  --ModelOptionList
  --paramOption
  --paramOptionList
  -h | --help        Show help information"
                }
            });
        }

        [Fact]
        public void DetailedHelp_IncludesModelAndParamDefinedArgs()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "Do -h",
                Then = { Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  ModelArg                   <TEXT>

  paramArg                   <TEXT>

  paramArgList (Multiple)    <TEXT>


Options:

  --ModelOption                   <TEXT>

  --ModelOptionList (Multiple)    <TEXT>

  --paramOption                   <TEXT>

  --paramOptionList (Multiple)    <TEXT>

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void Exec_MapsToAllArgs()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "Do --ModelOption moA --ModelOptionList moB --ModelOptionList moC " +
                           "--paramOption poA --paramOptionList poB --paramOptionList poC " +
                           "red green blue orange",
                Then =
                {
                    Outputs =
                    {
                        new Model
                        {
                            ModelOption = "moA",
                            ModelOptionList = new List<string>{"moB", "moC"},
                            ModelArg = "red"
                        },
                        new Params
                        {
                            ParamOption = "poA",
                            ParamOptionList = new List<string>{"poB", "poC"},
                            ParamArg = "green",
                            ParamArgList = new List<string>{"blue", "orange"}
                        }
                    }
                }
            });
        }

        [Fact]
        public void Exec_OptionsCanBeIncludedAfterArguments()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "Do --paramOptionList poB --paramOptionList poC " +
                           "red --paramOptionList poD green --paramOptionList poE " +
                           "blue --paramOptionList poF orange --paramOptionList poG",
                Then =
                {
                    Outputs =
                    {
                        new Model
                        {
                            ModelArg = "red"
                        },
                        new Params
                        {
                            ParamOptionList = new List<string>{"poB", "poC", "poD", "poE", "poF", "poG"},
                            ParamArg = "green",
                            ParamArgList = new List<string>{"blue", "orange"}
                        }
                    }
                }
            });
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do(
                Model model,
                [Operand] string paramArg,
                [Option] string paramOption,
                [Option] List<string> paramOptionList,
                [Operand] List<string> paramArgList)
            {
                TestOutputs.Capture(model);
                TestOutputs.Capture(new Params(paramArg, paramOption, paramOptionList, paramArgList));
            }
        }

        public class Model : IArgumentModel
        {
            [Operand]
            public string ModelArg { get; set; }

            [Option]
            public string ModelOption { get; set; }

            [Option]
            public List<string> ModelOptionList { get; set; }
        }

        public class Params
        {
            public string ParamArg { get; set; }

            public string ParamOption { get; set; }

            public List<string> ParamOptionList { get; set; }

            public List<string> ParamArgList { get; set; }

            public Params(string paramArg, string paramOption, List<string> paramOptionList, List<string> paramArgList)
            {
                ParamArg = paramArg;
                ParamOption = paramOption;
                ParamOptionList = paramOptionList;
                ParamArgList = paramArgList;
            }

            public Params()
            {
            }
        }

    }
}