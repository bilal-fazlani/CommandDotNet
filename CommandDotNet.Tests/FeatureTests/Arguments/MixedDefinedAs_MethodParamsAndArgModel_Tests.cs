using System.Collections.Generic;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class MixedDefinedAs_MethodParamsAndArgModel_Tests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public MixedDefinedAs_MethodParamsAndArgModel_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void BasicHelp_IncludesModelAndParamDefinedArgs()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options] <ModelArg> <paramArg> <paramArgList>

Arguments:
  ModelArg
  paramArg
  paramArgList

Options:
  --ModelOption
  --ModelOptionList
  --paramOption
  --paramOptionList
"
                }
            });
        }

        [Fact]
        public void DetailedHelp_IncludesModelAndParamDefinedArgs()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then = { Output = @"Usage: dotnet testhost.dll Do [options] <ModelArg> <paramArg> <paramArgList>

Arguments:

  ModelArg                 <TEXT>

  paramArg                 <TEXT>

  paramArgList (Multiple)  <TEXT>

Options:

  --ModelOption                 <TEXT>

  --ModelOptionList (Multiple)  <TEXT>

  --paramOption                 <TEXT>

  --paramOptionList (Multiple)  <TEXT>
" }
            });
        }

        [Fact]
        public void Exec_MapsToAllArgs()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "Do --ModelOption moA --ModelOptionList moB --ModelOptionList moC " +
                           "--paramOption poA --paramOptionList poB --paramOptionList poC " +
                           "red green blue orange"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new Model
                        {
                            ModelOption = "moA",
                            ModelOptionList = new List<string>{"moB", "moC"},
                            ModelArg = "red"
                        },
                        "green",
                        "poA",
                        new List<string>{"poB", "poC"},
                        new List<string>{"blue", "orange"})
                }
            });
        }

        [Fact]
        public void Exec_OptionsCanBeIncludedAfterArguments()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "Do --paramOptionList poB --paramOptionList poC " +
                           "red --paramOptionList poD green --paramOptionList poE " +
                           "blue --paramOptionList poF orange --paramOptionList poG"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new Model {ModelArg = "red"},
                        "green",
                        null,
                        new List<string>{"poB", "poC", "poD", "poE", "poF", "poG"},
                        new List<string>{"blue", "orange"})
                }
            });
        }

        private class App
        {
            public void Do(
                Model model,
                [Operand] string paramArg,
                [Option] string paramOption,
                [Option] List<string> paramOptionList,
                [Operand] List<string> paramArgList)
            {
            }
        }

        private class Model : IArgumentModel
        {
            [Operand]
            public string ModelArg { get; set; } = null!;

            [Option]
            public string ModelOption { get; set; } = null!;

            [Option]
            public List<string> ModelOptionList { get; set; } = null!;
        }
    }
}
