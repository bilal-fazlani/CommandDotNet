using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class NestedArgModel : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public NestedArgModel(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void NestedModel_BasicHelp_IncludesNestedOperandsAndOptions()
        {
            Verify(new Given<NestedModelApp>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "Do -h",
                Then = { Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  Operand1
  Operand2

Options:
  --Option1
  --Option2
  -h | --help  Show help information" }
            });
        }

        [Fact]
        public void NestedModel_DetailedHelp_IncludesNestedOperandsAndOptions()
        {
            Verify(new Given<NestedModelApp>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "Do -h",
                Then = { Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  Operand1    <TEXT>

  Operand2    <TEXT>


Options:

  --Option1      <TEXT>

  --Option2      <TEXT>

  -h | --help
  Show help information" }
            });
        }

        [Fact]
        public void NestedModel_Exec_MapsNestedOperandsAndOptions()
        {
            Verify(new Given<NestedModelApp>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "Do --Option1 aaa --Option2 bbb ccc ddd",
                Then =
                {
                    Outputs =
                    {
                        new ParentModel
                        {
                            Option1 = "aaa", Operand1 = "ccc",
                            NestedModel = new NestedModel {Option2 = "bbb", Operand2 = "ddd"}
                        }
                    }
                }
            });
        }

        public class NestedModelApp
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do(ParentModel parameterModel)
            {
                TestOutputs.Capture(parameterModel);
            }
        }

        public class ParentModel: IArgumentModel
        {
            [Option]
            public string Option1 { get; set; }

            [Operand]
            public string Operand1 { get; set; }

            public NestedModel NestedModel { get; set; }
        }

        public class NestedModel : IArgumentModel
        {
            [Option]
            public string Option2 { get; set; }

            [Operand]
            public string Operand2 { get; set; }
        }
    }
}