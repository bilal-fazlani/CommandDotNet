# TestConfig

## TestConfig

The `TestConfig` is used to control what is logged to output during a test run.

The default value will output the console buffer, both Out and Error, on error.

Set TestConfig.Default to provide a different default.  If you're using a test framework that doesn't provide for a way to set the default before tests are run, looking at you XUnit, then implement an `IDefaultTestConfig` as [shown below](#idefaulttestconfig) 

```c#
public class TestConfig
{
    /// <summary>
    /// Default scans loaded assemblies for <see cref="IDefaultTestConfig"/>
    /// and stores the config with the lowest <see cref="Priority"/>
    /// </summary>
    public static TestConfig Default { get; set; }

    /// <summary>Nothing will be printed and errors will be captured</summary>
    public static TestConfig Silent { get; set; }

    /// <summary>
    /// Configuration to be used when no exception has 
    /// escaped <see cref="AppRunner.Run"/><br/>
    /// Default: prints nothing
    /// </summary>
    public OnSuccessConfig OnSuccess { get; set; }

    /// <summary>
    /// Configuration to be used when no exception has 
    /// escaped <see cref="AppRunner.Run"/><br/>
    /// Default: prints <see cref="PrintConfig.ConsoleOutput"/>
    /// </summary>
    public OnErrorConfig OnError { get; set; }

    /// <summary>When true, CommandDotNet logs will output to logLine</summary>
    public bool PrintCommandDotNetLogs { get; set; }

    /// <summary>
    /// To identify the <see cref="TestConfig"/> in case 
    /// the expected config was not used.<br/>
    /// Will be auto-populated when created from 
    /// <seealso cref="IDefaultTestConfig"/>
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// When multiple <see cref="IDefaultTestConfig"/>s are found,
    /// the <see cref="TestConfig"/> with the lowest priority will be used.<br/>
    /// This property is only needed when providing the default via 
    /// <see cref="IDefaultTestConfig"/><br/>
    /// Set <see cref="Default"/> directly to avoid use of this property.<br/>
    /// Create and .gitignore a <see cref="IDefaultTestConfig"/> 
    /// with short.MinValue for verbose local logging and quite CI logging.<br/>
    /// </summary>
    public short Priority { get; set; } = short.MaxValue;
    
    /// <summary>
    /// Returns a clone of the TestConfig after applying 
    /// the <see cref="alter"/> action
    /// </summary>
    public TestConfig Where(Action<TestConfig> alter){}

    public class OnSuccessConfig
    {
        public PrintConfig Print { get; set; }
    }

    public class OnErrorConfig
    {
        /// <summary>
        /// When true, errors escaping <see cref="AppRunner.Run"/> will be
        /// captured in <see cref="AppRunnerResult"/> and 
        /// <see cref="AppRunnerResult.ExitCode"/> will be set to 1. 
        /// This mimics how the shell will process it.
        /// </summary>
        public bool CaptureAndReturnResult { get; set; }
        public PrintConfig Print { get; set; }
    }

    public class PrintConfig
    {
        /// <summary>When true, all options will be printed</summary>
        public bool All
        {
            get => AppConfig && CommandContext 
                    && ConsoleOutput && ParseReport;
            set => AppConfig = CommandContext 
                    = ConsoleOutput = ParseReport = value;
        }

        /// <summary>Print the <see cref="AppConfig"/></summary>
        public bool AppConfig { get; set; }

        /// <summary>Print the <see cref="CommandContext"/></summary>
        public bool CommandContext { get; set; }
        
        /// <summary>Print the <see cref="TestConsole.AllText"/></summary>
        public bool ConsoleOutput { get; set; }

        /// <summary>
        /// Print the output of <see cref="ParseReporter.Report"/> 
        /// to see how values are assigned to arguments
        /// </summary>
        public bool ParseReport { get; set; }
    }
}
```

## IDefaultTestConfig

Implement an `IDefaultTestConfig` when you need the config to be auto-discovered.

`TestConfig.Default` will lazy load a default if one is not provided. The lazy load will scan all loaded assemblies for implementations of `IDefaultTestConfig`. The resulting TestConfigs will be loaded and the one
with the lowest priority will be selected.

```c#
/// <summary>Implement this to provide a TestConfig for tests</summary>
public interface IDefaultTestConfig
{
    /// <summary>The TestConfig to use as a default</summary>
    TestConfig Default { get; }
}
```

This supports the scenario where a dev keeps a `DevDefaultTestConfig` in the local directory to log All on an error and ConsoleOutput on success. The file can be added to .gitignore, just like an appSettings.env.local file. This is possible with the new project files that only require the file to be present.

```c#
public class DevDefaultTestConfig : IDefaultTestConfig
{
    public TestConfig Default => new TestConfig
    {
        Priority = short.MinValue,
        PrintCommandDotNetLogs = true,
        OnSuccess = {Print = {ConsoleOutput = true}},
        OnError = {Print = {All = true}}
    };
}
```

This approach was chosen because it was simple to implement, required no additional json parsing libraries and works well with the maintainers work flow. If you need json support, start with this config and the serializer of your choice. We may add another package with this eventually.

```c#
public class JsonDefaultTestConfig : IDefaultTestConfig
{
    const string JsonFile = "CommandDotNet.TestConfig.json";
    public TestConfig Default 
    {
        get
        {
            if (File.Exists(JsonFile))
            {
                var json = File.ReadAllText(JsonFile);
                var testConfig = JsonSerializer.Deserialize<TestConfig>(json);
                testConfig.Source = Path.GetFullPath(JsonFile);
                if (!testConfig.Priority.HasValue)
                {
                    testConfig.Priority = 0;
                }
                return testConfig;
            }

            return null;
        }
    }
}
```

## Example of all output

Here is an example of what you'll see when `PrintCommandDotNetLogs=true` and `On___.Print.All=true`.

ConsoleOutput and ParseReport will the most useful in regular scenarios.

The CommandContext and AppConfig sections are likely more helpful when debugging middleware or other extensibility points or failures following an update of the framework.

```bash

I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: <CaptureState>b__1 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: TokenizeInputMiddleware 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: CreateRootCommand 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: ParseInputMiddleware 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: AssembleInvocationPipelineMiddleware 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: CheckIfShouldShowHelp 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: BindValues 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: ResolveCommandClassInstances 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: Middleware 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: InjectTestCaptures 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > begin: invoke middleware: InvokeInvocationPipelineMiddleware 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: InvokeInvocationPipelineMiddleware 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: InjectTestCaptures 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: Middleware 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: ResolveCommandClassInstances 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: BindValues 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: CheckIfShouldShowHelp 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: AssembleInvocationPipelineMiddleware 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: ParseInputMiddleware 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: CreateRootCommand 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: TokenizeInputMiddleware 
I CommandDotNet.Execution.ExecutionMiddlewareExtensions > end: invoke middleware: <CaptureState>b__1 

Console output <begin> ------------------------------
<no output>
Console output <end> ------------------------------


CommandContext:
  RootCommand:Command Command:App  (CommandDotNet.Tests.CommandDotNet.FluentValidation.ModelValidationTests+App)
  ShowHelpOnExit:False
  Original.Args:Save 1 john john@doe.com
  Tokens:Save 1 john john@doe.com
  ParseResult:ParseResult:
    TargetCommand:Command Command:Save  (CommandDotNet.Tests.CommandDotNet.FluentValidation.ModelValidationTests+App.Save)
    RemainingOperands:
    SeparatedArguments:
    ParseError:
  InvocationPipeline:InvocationPipeline:
    TargetCommand:InvocationStep:
      Command=Save
      Invocation=CommandDotNet.Tests.CommandDotNet.FluentValidation.ModelValidationTests+App.Save
      Instance=CommandDotNet.Tests.CommandDotNet.FluentValidation.ModelValidationTests+App



Parse report <begin> ------------------------------
command: Save

arguments:

  Id <Number>
    value: 1
    inputs: 1
    default:

  Name <Text>
    value: john
    inputs: john
    default:

  Email <Text>
    value: john@doe.com
    inputs: john@doe.com
    default:
Parse report <end> ------------------------------


AppRunner<App>:
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
      DefaultArgumentSeparatorStrategy: PassThru
      DisableDirectives: False
      Help: AppHelpSettings:
        ExpandArgumentsInUsage: False
        PrintHelpOption: False
        TextStyle: Detailed
        UsageAppName: 
        UsageAppNameStyle: Adaptive
      IgnoreUnexpectedOperands: False
    DependencyResolver: CommandDotNet.TestTools.TestDependencyResolver
    HelpProvider: CommandDotNet.Help.HelpTextProvider
    TokenTransformations:
      expand-clubbed-flags(2147483647)
      split-option-assignments(2147483647)
    MiddlewarePipeline:
      HelpMiddleware.PrintHelp
      <>c__DisplayClass0_0.<CaptureState>b__1
      TokenizerPipeline.TokenizeInputMiddleware
      ClassModelingMiddleware.CreateRootCommand
      CommandParser.ParseInputMiddleware
      ClassModelingMiddleware.AssembleInvocationPipelineMiddleware
      HelpMiddleware.CheckIfShouldShowHelp
      BindValuesMiddleware.BindValues
      ResolveCommandClassesMiddleware.ResolveCommandClassInstances
      FluentValidationMiddleware.Middleware
      AppRunnerTestExtensions.InjectTestCaptures
      ClassModelingMiddleware.InvokeInvocationPipelineMiddleware
    ParameterResolvers:
      CommandDotNet.CommandContext
      CommandDotNet.Rendering.IConsole
      System.Threading.CancellationToken

```