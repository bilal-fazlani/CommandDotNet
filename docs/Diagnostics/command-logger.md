# Command Logger

## TLDR, How to enable 
1. Enable the feature with `appRunner.UseCommandLogger()` or use `appRunner.UseDefaultMiddleware()`

## Command logging

This feature logs information about the command just before the command method is executed.

The default behavior registers a `[cmdlog]` directive that outputs to the console using `CommandContext.Console.Out`.

example usage: 

```bash
dotnet example.com [cmdlog] add 1 2
```

## Outputs

### Command and arguments
The command to be executed and the argument values as described in the [parse directive](../Diagnostics/parse-directive.md).

```bash
command: LaunchRocket

arguments:

  planets <Text>
      value: mars, earth, jupiter, mercury, venus, saturn
      inputs:
        [argument] mars, earth, jupiter
        [piped stream]
      default: source=AppSetting key=--planets: mars, jupiter

options:
  
  crew <Text>
      value: Aaron, Alex
      inputs: [prompt] Aaron, Alex
      default: Bilal

  username <Text>
      value: Bilal
      inputs:
      default: source=EnvVar key=Username: Bilal
  
  password <Text>
      value: *****
      inputs: [prompt] *****
      default: source=EnvVar key=Password: *****
```

### System information

System information is included by default and can be excluded by setting `excludeSystemInfo: true`. 

The default set of information is:

```bash
Tool version  = testhost.dll 16.2.0
.Net version  = .NET Core 4.6.26614.01
OS version    = Microsoft Windows 10.0.18363
Machine       = MyComputer
Username      = MyComputer\Me
```

Additional information can be provided by setting the `additionalInfoCallback` parameter with a 
`Func<CommandContext, IEnumerable<(string key, string value)>>`.  Any CommandContext state can be included.

### AppConfig

AppConfig can be included by setting `includeAppConfig: true`. This

```bash
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
    Help: CommandDotNet.Help.AppHelpSettings
    IgnoreUnexpectedOperands: False
  DependencyResolver:
  HelpProvider: CommandDotNet.Help.HelpTextProvider
  TokenTransformations:
    expand-clubbed-flags
    split-option-assignments
  MiddlewarePipeline:
    TokenizerPipeline.TokenizeInputMiddleware
    ClassModelingMiddleware.CreateRootCommand
    CommandParser.ParseInputMiddleware
    ClassModelingMiddleware.AssembleInvocationPipelineMiddleware
    HelpMiddleware.DisplayHelp
    BindValuesMiddleware.BindValues
    ResolveCommandClassesMiddleware.ResolveCommandClassInstances
    AppRunnerTestExtensions.InjectTestCaptures
    CommandLoggerMiddleware.CommandLogger
    ClassModelingMiddleware.InvokeInvocationPipelineMiddleware
  ParameterResolvers:
    CommandDotNet.CommandContext
    CommandDotNet.Rendering.IConsole
    System.Threading.CancellationToken
```

## Replacing the default behavior

Use `writerFactory` parameter to conditionally provide a target for the log. 

If the factory returns null, the command will not be logged.

How to use:

#### Change the target output

`writerFactory: ctx => Log.Info`

Logs every command to logging framework. No `[cmdlog]` directive

#### Enable via attribute

`writerFactory: ctx => ctx.ParseResult.TargetCommand.HasAttribute<EnableCommandLogger>() ? ctx.Console.Out.WriteLine : (Action<string>)null`

Logs only commands attributed with your custom `EnableCommandLoggerAttribute`. No `[cmdlog]` directive

#### Enable via directive 

Allow user to enable as a [directive](../Extensibility/directives.md)
`writerFactory: ctx => ctx.Tokens.TryGetDirective("cmdlog", out _) ? ctx.Console.Out.WriteLine : (Action<string>)null`

Usage: `dotnet example.com [cmdlog] add 1 2`

!!!Note
    this is the default behavior.

#### Enable via root option

Add an intercepor method to your root command with an `--logcmd` option. This also makes the option visible to users via help.

```c#
public class RootApp
{
    public Task<int> Interceptor(InterceptorExecutionDelegate next, CommandContext ctx,
        [Option(Description="Output the command wiht arguments and system info")] bool cmdlog)
    {
        if(cmdlog)
        {
            CommandLogger.Log(ctx);
        }
        next();
    }
}
```

used as `dotnet example.dll --logcmd Add 1 2`

#### Blended, Enable via root option or attribute

* Always output to logs 
* output to the console when... 
    * The user runs with the `[cmdlog]` directive
    * The command is attributed with your custom `EnableCommandLoggerAttribute`

```c#
appRunner.UseCommandLogger(writerFactory: ctx => 
{
    // EnableCommandLogger is just an example name you could implement
    if (ctx.ParseResult.TargetCommand.HasAttribute<EnableCommandLogger>()
        || (bool)ctx.RootCommand.Options.FindOption("logcmd").Value)
    {
        return Log.IsInfoEnabled()
            ? log =>
            {
                Log.Info(log);
                ctx.Console.Out.WriteLine(log);
            }
            : (Action<string>)ctx.Console.Out.WriteLine;
    }

    return Log.IsInfoEnabled()
        ? Log.Info
        : (Action<string>)null;
});
```