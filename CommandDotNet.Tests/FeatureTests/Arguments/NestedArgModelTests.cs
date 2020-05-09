using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class NestedArgModelTests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public NestedArgModelTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void NestedModel_BasicHelp_IncludesNestedOperandsAndOptions()
        {
            new AppRunner<NestedModelApp>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then = { Output = @"Usage: dotnet testhost.dll Do [options] <Operand1> <Operand2>

Arguments:
  Operand1
  Operand2

Options:
  --Option1
  --Option2
" }
            });
        }

        [Fact]
        public void NestedModel_DetailedHelp_IncludesNestedOperandsAndOptions()
        {
            new AppRunner<NestedModelApp>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then = { Output = @"Usage: dotnet testhost.dll Do [options] <Operand1> <Operand2>

Arguments:

  Operand1  <TEXT>

  Operand2  <TEXT>

Options:

  --Option1  <TEXT>

  --Option2  <TEXT>
" }
            });
        }

        [Fact]
        public void NestedModel_Exec_MapsNestedOperandsAndOptions()
        {
            new AppRunner<NestedModelApp>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "Do --Option1 aaa --Option2 bbb ccc ddd"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new ParentModel
                        {
                            Option1 = "aaa", Operand1 = "ccc",
                            NestedModel = new NestedModel {Option2 = "bbb", Operand2 = "ddd"}
                        })
                }
            });
        }

        private class NestedModelApp
        {
            public void Do(ParentModel parameterModel)
            {
            }
        }

        private class ParentModel: IArgumentModel
        {
            [Option]
            public string Option1 { get; set; } = null!;

            [Operand]
            public string Operand1 { get; set; } = null!;

            [OrderByPositionInClass]
            public NestedModel NestedModel { get; set; } = null!;
        }

        private class NestedModel : IArgumentModel
        {
            [Option]
            public string Option2 { get; set; } = null!;

            [Operand]
            public string Operand2 { get; set; } = null!;
        }
    }
}
