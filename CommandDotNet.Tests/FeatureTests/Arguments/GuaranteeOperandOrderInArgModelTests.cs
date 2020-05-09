using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;
using static System.Environment;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class GuaranteeOperandOrderInArgModelTests
    {
        private static readonly AppSettings OperandModeWithGuarantee = TestAppSettings.BasicHelp.Clone(a => a.DefaultArgumentMode = ArgumentMode.Operand);
        private static readonly AppSettings OptionModeWithGuarantee = TestAppSettings.BasicHelp.Clone(a => a.DefaultArgumentMode = ArgumentMode.Option);
        
        public GuaranteeOperandOrderInArgModelTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }
        
        [Fact]
        public void GivenOptionMode_WithGuarantee_UnattributedArgModel_Should_BeOk()
        {
            new AppRunner<UArgModelApp>(OptionModeWithGuarantee)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then = { ExitCode = 0 }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_UnattributedArgModel_Should_Detect_UnattributedOperand()
        {
            new AppRunner<UArgModelApp>(OperandModeWithGuarantee)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        ExitCode = 1,
                        Output = "Operand property must be attributed with OperandAttribute or " +
                                 "OrderByPositionInClassAttribute to guarantee consistent order. " +
                                 "Property: CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+UnattributedArgModel.Arg1" +
                                 $"{NewLine}"
                    }
                });
        }

        [Fact]
        public void GivenOptionMode_WithGuarantee_AttributedArgModel_Should_BeOk()
        {
            new AppRunner<AArgModelApp>(OptionModeWithGuarantee)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        ExitCode = 0,
                        OutputContainsTexts =
                        {
                            @"Arguments:
  Operand1",
                            @"Options:
  --Arg1"
                        }
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_AttributedArgModel_Should_ListOperandsInCorrectOrder()
        {
            new AppRunner<AArgModelApp>(OperandModeWithGuarantee)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        ExitCode = 0,
                        OutputContainsTexts =
                        {
                            @"Arguments:
  Arg1
  Operand1"
                        }
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_AttributedNestedModel_With_AttributedArgModel_Should_BeOk()
        {
            new AppRunner<ANestedModelAArgModelApp>(OperandModeWithGuarantee)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        ExitCode = 0,
                        OutputContainsTexts =
                        {
                            @"Arguments:
  OperandDefinedFirstReflectedLast
  Arg1
  Operand1
  Arg2
  Operand2"
                        }
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_UnattributedNestedModel_With_UnattributedArgModel_Should_Detect_UnattributedOperand()
        {
            new AppRunner<UNestedModelUArgModelApp>(OperandModeWithGuarantee)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        ExitCode = 1,
                        Output = "Operand property must be attributed with OperandAttribute or " +
                                 "OrderByPositionInClassAttribute to guarantee consistent order. " +
                                 "Property: CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+UnattributedArgModel.Arg1" +
                                 $"{NewLine}"
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_UnattributedNestedModel_With_AttributedArgModel_Should_Detect_UnattributedArgModel()
        {
            new AppRunner<UNestedModelAArgModelApp>(OperandModeWithGuarantee)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        ExitCode = 1,
                        Output = "Operand property must be attributed with OperandAttribute or " +
                                 "OrderByPositionInClassAttribute to guarantee consistent order. " +
                                 @"Properties:
  CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+UnattributedNestedModelAttributedArgModel.Model" +
                                 $"{NewLine}"
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_AttributedNestedModel_With_UnattributedArgModel_Should_Detect_UnattributedOperand()
        {
            new AppRunner<ANestedModelUArgModelApp>(OperandModeWithGuarantee)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        ExitCode = 1,
                        Output = "Operand property must be attributed with OperandAttribute or " +
                                 "OrderByPositionInClassAttribute to guarantee consistent order. " +
                                 "Property: CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+UnattributedArgModel.Arg1" +
                                 $"{NewLine}"
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_DeepNestedUnattributedArgModel_Should_Detect_All_UnattributedArgModels()
        {
            new AppRunner<DeepNestedUArgModelsApp>(OperandModeWithGuarantee)
                .Verify(new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        ExitCode = 1,
                        Output = "Operand property must be attributed with OperandAttribute or " +
                                 "OrderByPositionInClassAttribute to guarantee consistent order. " +
                                 @"Properties:
  CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+UnattributedNestedModelAttributedArgModel.Model
  CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+DeepNestedUnattributedArgModels.Model" +
                                 $"{NewLine}"
                    }
                });
        }

        // Invalid
        public class UArgModelApp { public void Do(UnattributedArgModel model) { } }
        // Valid & can verify order
        public class AArgModelApp { public void Do(AttributedArgModel model) { } }
        // Invalid
        public class UNestedModelUArgModelApp { public void Do(UnattributedNestedModelUnattributedArgModel model) { } }
        // Invalid
        public class UNestedModelAArgModelApp { public void Do(UnattributedNestedModelAttributedArgModel model) { } }
        // Invalid
        public class ANestedModelUArgModelApp { public void Do(AttributedNestedModelUnattributedArgModel model) { } }
        // Valid & can verify order
        public class ANestedModelAArgModelApp { public void Do(AttributedNestedModelAttributedArgModel model) { } }
        // Valid & can verify order
        public class DeepNestedUArgModelsApp { public void Do(DeepNestedUnattributedArgModels model) { } }

        // Invalid
        public class UnattributedArgModel : IArgumentModel
        {
            public string Arg1 { get; set; } = null!;
        }

        // Valid & can verify order
        public class AttributedArgModel : IArgumentModel
        {
            [OrderByPositionInClass]
            public string Arg1 { get; set; } = null!;

            [Operand]
            public string Operand1 { get; set; } = null!;
        }

        // Invalid
        public class DeepNestedUnattributedArgModels : IArgumentModel
        {
            // operand must be attributed so we can get parent arg model alerts
            public UnattributedNestedModelAttributedArgModel Model { get; set; } = null!;
        }

        // Invalid
        public class UnattributedNestedModelUnattributedArgModel : IArgumentModel
        {
            public UnattributedArgModel Model { get; set; } = null!;
        }

        // Invalid
        public class UnattributedNestedModelAttributedArgModel : IArgumentModel
        {
            public AttributedArgModel Model { get; set; } = null!;
        }

        // Invalid
        public class AttributedNestedModelUnattributedArgModel : IArgumentModel
        {
            [OrderByPositionInClass]
            public UnattributedArgModel Model { get; set; } = null!;
        }

        // Valid & can verify order
        public class AttributedNestedModelAttributedArgModel : IArgumentModel
        {
            [OrderByPositionInClass]
            public AttributedArgModel Model { get; set; } = null!;

            [OrderByPositionInClass]
            public string Arg2 { get; set; } = null!;

            [Operand]
            public string Operand2 { get; set; } = null!;

            // mimic operand defined first but returned last when reflected
            [Operand(1)] public string OperandDefinedFirstReflectedLast { get; set; } = null!;
        }
    }
}
