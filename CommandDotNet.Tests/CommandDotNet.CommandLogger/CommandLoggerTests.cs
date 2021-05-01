using System;
using System.Text;
using System.Threading.Tasks;
using CommandDotNet.ConsoleOnly;
using CommandDotNet.Execution;
using Diag=CommandDotNet.Diagnostics;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.CommandLogger
{
    public class CommandLoggerTests
    {
        public CommandLoggerTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void PasswordsAreObscured()
        {
            new AppRunner<App>()
                .UseCommandLogger(excludeSystemInfo: true)
                .Verify(new Scenario
                {
                    When = {Args = "[cmdlog] --password super-secret Do lala"},
                    Then =
                    {
                        Output = @"
***************************************
Original input:
  [cmdlog] --password ***** Do lala

command: Do

arguments:

  textOperand <Text>
    value: lala
    inputs: lala
    default:

options:

  textOption <Text>
    value:
    inputs:
    default:

  password <Text>
    value: *****
    inputs: ***** (from: --password *****)
    default:
***************************************
"
                    }
                });
        }

        [Fact]
        public void SystemInfo_IsShown_ByDefault()
        {
            new AppRunner<App>()
                .UseCommandLogger()
                .Verify(new Scenario
                {
                    When = {Args = "[cmdlog] Do"},
                    Then =
                    {
                        Output = $@"
***************************************
Original input:
  [cmdlog] Do

command: Do

arguments:

  textOperand <Text>
    value:
    inputs:
    default:

options:

  textOption <Text>
    value:
    inputs:
    default:

  password <Text>
    value:
    inputs:
    default:

Tool version  = testhost.dll 1.1.1.1
.Net version  = {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.Trim()}
OS version    = {System.Runtime.InteropServices.RuntimeInformation.OSDescription.Trim()}
Machine       = {Environment.MachineName}
Username      = {Environment.UserDomainName}\{Environment.UserName}
***************************************
"
                    }
                });
        }

        [Fact]
        public void AppConfig_CanBe_Shown()
        {
            #region example AppConfig output

            /*
AppConfig:
  AppSettings:
    ArgumentTypeDescriptors: ArgumentTypeDescriptors:
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.BoolTypeDescriptor
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.EnumTypeDescriptor
      ErrorReportingDescriptor > DelegatedTypeDescriptor<String>: 'Text'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Password>: 'Text'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Char>: 'Character'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Int64>: 'Number'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Int32>: 'Number'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Int16>: 'Number'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Decimal>: 'Decimal'
      ErrorReportingDescriptor > DelegatedTypeDescriptor<Double>: 'Double'
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.ComponentModelTypeDescriptor
      ErrorReportingDescriptor > CommandDotNet.TypeDescriptors.StringCtorTypeDescriptor
    BooleanMode: Implicit
    DefaultArgumentMode: Operand
    DisableDirectives: False
    GuaranteeOperandOrderInArgumentModels: False
    Help: AppHelpSettings:
      ExpandArgumentsInUsage: False
      PrintHelpOption: False
      TextStyle: Detailed
      UsageAppName:
      UsageAppNameStyle: Adaptive
    IgnoreUnexpectedOperands: False
  DependencyResolver:
  HelpProvider: CommandDotNet.Help.HelpTextProvider
  TokenTransformations:
    expand-clubbed-flags(2147483647)
    split-option-assignments(2147483647)
  MiddlewarePipeline:
    <>c__DisplayClass0_0.<CaptureState>b__1
    TokenizerPipeline.TokenizeInputMiddleware
    ClassModelingMiddleware.CreateRootCommand
    CommandParser.ParseInputMiddleware
    ClassModelingMiddleware.AssembleInvocationPipelineMiddleware
    HelpMiddleware.DisplayHelp
    BindValuesMiddleware.BindValues
    ResolveCommandClassesMiddleware.ResolveCommandClassInstances
    AppRunnerTestExtensions.InjectTestOutputs
    CommandLoggerMiddleware.CommandLogger
    ClassModelingMiddleware.InvokeInvocationPipelineMiddleware
  ParameterResolvers:
    CommandDotNet.CommandContext
    CommandDotNet.Rendering.IConsole
    System.Threading.CancellationToken
             */

            #endregion

            new AppRunner<App>()
                .UseCommandLogger(excludeSystemInfo: true, includeAppConfig: true)
                .Verify(new Scenario
                {
                    When = {Args = "[cmdlog] Do"},
                    Then =
                    {
                        OutputContainsTexts =
                        {
                            "AppConfig:",
                            "  AppSettings:",
                            "  DependencyResolver:",
                            "  HelpProvider:",
                            "  TokenTransformations:",
                            "  MiddlewarePipeline:",
                            "  ParameterResolvers:"
                        }
                    }
                });
        }

        [Fact]
        public void AdditionalInfo_CanBe_Null()
        {
            new AppRunner<App>()
                .UseCommandLogger(excludeSystemInfo: true, additionalInfoCallback: ctx => null)
                .Verify(new Scenario
                {
                    When = {Args = "[cmdlog] Do"},
                    Then =
                    {
                        Output = @"
***************************************
Original input:
  [cmdlog] Do

command: Do

arguments:

  textOperand <Text>
    value:
    inputs:
    default:

options:

  textOption <Text>
    value:
    inputs:
    default:

  password <Text>
    value:
    inputs:
    default:
***************************************
"
                    }
                });
        }

        [Fact]
        public void AdditionalInfo_CanBe_Shown()
        {
            new AppRunner<App>()
                .UseCommandLogger(excludeSystemInfo: true, additionalInfoCallback: ctx => new[]
                {
                    ("header1", "value1"),
                    ("header2", "value2")
                })
                .Verify(new Scenario
                {
                    When = {Args = "[cmdlog] Do"},
                    Then =
                    {
                        Output = @"
***************************************
Original input:
  [cmdlog] Do

command: Do

arguments:

  textOperand <Text>
    value:
    inputs:
    default:

options:

  textOption <Text>
    value:
    inputs:
    default:

  password <Text>
    value:
    inputs:
    default:

header1  = value1
header2  = value2
***************************************
"
                    }
                });
        }

        [Fact]
        public void WriterFactory_Can_ReturnNull_And_NothingIsLogged()
        {
            var sb = new StringBuilder();

            new AppRunner<App>()
                .UseCommandLogger(excludeSystemInfo: true, writerFactory: context => null)
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then =
                    {
                        Output = ""
                    }
                });
        }

        [Fact]
        public void WriterFactory_CanBe_Provided()
        {
            var sb = new StringBuilder();

            new AppRunner<App>()
                .UseCommandLogger(excludeSystemInfo: true, writerFactory: context => text => sb.AppendLine(text))
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then =
                    {
                        Output = ""
                    }
                });

            sb.ToString().Should().Be(@"
***************************************
Original input:
  Do

command: Do

arguments:

  textOperand <Text>
    value:
    inputs:
    default:

options:

  textOption <Text>
    value:
    inputs:
    default:

  password <Text>
    value:
    inputs:
    default:
***************************************
");
        }

        [Fact]
        public void DefaultBehavior_DoesOutput_With_CmdLogDirective()
        {
            new AppRunner<App>()
                .UseCommandLogger()
                .Verify(new Scenario
                {
                    When = {Args = "[cmdlog] Do"},
                    Then = {OutputContainsTexts = {"Original input:"}}
                });
        }

        [Fact]
        public void DefaultBehavior_DoesNotOutput_Without_CmdLogDirective()
        {
            new AppRunner<App>()
                .UseCommandLogger()
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then = {Output = ""}
                });
        }

        [Fact]
        public void CommandLogger_Log_CanBeCalled_BeforeParseResults()
        {
            // this supports printing the CommandLogger in error handling.

            new AppRunner<App>()
                .UseCommandLogger(excludeSystemInfo: true, additionalInfoCallback: ctx => new[]
                {
                    ("header1", "value1"),
                    ("header2", "value2")
                })
                .Configure(c => c.UseMiddleware((context, next) =>
                {
                    Diag.CommandLogger.Log(context, context.Console.Out.WriteLine);
                    return ExitCodes.Success;
                }, MiddlewareStages.PostTokenizePreParseInput))
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then =
                    {
                        AssertOutput = o => { o.Should().ContainAll("header1", "value1", "header2", "value2"); }
                    }
                });

        }

        public class App
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, Password password)
            {
                return next();
            }

            public void Do(
                [Option] string textOption,
                [Operand] string textOperand)
            {
            }
        }
    }
}