using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class DefaultArgumentModeTests
    {
        private static readonly AppSettings OperandMode = TestAppSettings.BasicHelp.Clone(a => a.DefaultArgumentMode = ArgumentMode.Operand);
        private static readonly AppSettings OptionMode = TestAppSettings.BasicHelp.Clone(a => a.DefaultArgumentMode = ArgumentMode.Option);

        public DefaultArgumentModeTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void GivenOperandMode_InInterceptor_NonAttributedParamsDefaultTo_Operand()
        {
            new AppRunner<App>(OperandMode).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    OutputNotContainsTexts = { "Arguments" },
                    OutputContainsTexts =
                    {
                        @"Options:
  --ctorDefault
  --ctorOption"
                    }
                }
            });
        }

        [Fact]
        public void GivenOperandMode_InMethod_NonAttributedParamsDefaultTo_Operand()
        {
            new AppRunner<App>(OperandMode).Verify(new Scenario
            {
                When = {Args = "Method -h"},
                Then =
                {
                    OutputContainsTexts =
                    {
                        @"Arguments:
  default
  operand",
                        @"Options:
  --option"
                    }
                }
            });
        }

        [Fact]
        public void GivenOperandMode_InModel_NonAttributedParamsDefaultTo_Operand()
        {
            new AppRunner<App>(OperandMode).Verify(new Scenario
            {
                When = {Args = "Model -h"},
                Then =
                {
                    OutputContainsTexts =
                    {
                        @"Arguments:
  Default
  Operand",
                        @"Options:
  --Option"
                    }
                }
            });
        }

        [Fact]
        public void GivenOptionMode_InInterceptor_NonAttributedParamsDefaultTo_Option()
        {
            new AppRunner<App>(OptionMode).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    OutputNotContainsTexts = { "Arguments" },
                    OutputContainsTexts =
                    {
                        @"Options:
  --ctorDefault
  --ctorOption"
                    }
                }
            });
        }

        [Fact]
        public void GivenOptionMode_InMethod_NonAttributedParamsDefaultTo_Option()
        {
            new AppRunner<App>(OptionMode).Verify(new Scenario
            {
                When = {Args = "Method -h"},
                Then =
                {
                    OutputContainsTexts =
                    {
                        @"Arguments:
  operand",
                        @"Options:
  --default
  --option"
                    }
                }
            });
        }

        [Fact]
        public void GivenOptionMode_InModel_NonAttributedParamsDefaultTo_Option()
        {
            new AppRunner<App>(OptionMode).Verify(new Scenario
            {
                When = {Args = "Model -h"},
                Then =
                {
                    OutputContainsTexts =
                    {
                        @"Arguments:
  Operand",
                        @"Options:
  --Default
  --Option"
                    }
                }
            });
        }

        private class App
        {
            public Task<int> Middleware(CommandContext context, ExecutionDelegate next, string ctorDefault, [Option] string ctorOption)
            {
                return next(context);
            }

            public void Model(Model model)
            {
            }

            public void Method(string @default, [Operand] string operand, [Option] string option)
            {
            }
        }

        private class Model : IArgumentModel
        {
            // using OrderByPositionInClass allows this to be either option or operand based on default mode
            // it's unlikely this will every be used like this since it doesn't seem to make sense to define an 
            // argument as option or operand depending on the setting.
            [OrderByPositionInClass]
            public string Default { get; set; } = null!;
            [Operand]
            public string Operand { get; set; } = null!;
            [Option]
            public string Option { get; set; } = null!;
        }
    }
}
