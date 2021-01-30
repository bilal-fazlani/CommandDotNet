# CommandDotNet

## 4.1.3

Fixes a bug where the TypoSuggest middleware threw an exception when an argument value was an empty string.

## 4.1.2

Fixes a bug where values were not assigned to options added via the builder.

## 4.1.1

Fixes a bug where a negative number can treated as a short name. eg. `add -5 10` would have resulted in -5 being parsed as an option. 

Add restriction that short names can only be letters. If an arg can be parsed as a negative number, it will not be confused with a short name.

## 4.1.0

### unrequested help returns 1 instead of 0

When help is shown because a given command is not executable, an error code of 1 is returned to indicate a command was not successfully run.

### new types supported for arguments

In addition to types with a TypeConverter or Ctor with a single string parameter. any type with a public static Parse method with a single string parameter are now eligible as types for arguments.

### API

* AppRunnerException will not be passed to the error handler. Previously they were always printed to the console.

### Obsoleted

* `ArgumentArity.Default(IArgument)` for the other ArgumentArity.Default method. It was inadequate in some cases and could lead to bugs.
* `BooleanMode.Unknown` in favor of `BooleanMode?`.
* `AppInfo.GetAppinfo(CommandContext)` in favor of `AppInfo.GetAppInfo()`. The former exists to support CommandDotNet tests.
* `Command.GetIgnoreUnexpectedOperands(AppSettings)` & `Command.GetArgumentSeparatorStrategy(AppSettings)` in favor of the properties on the command which reflect AppSettings defaults.


## 4.0.2

Make FlattenedArgumentModels via the IInvocation. 
This makes it easier to access all argument models for an invocation, including those defined as properties of other models.
It also prevents additional reflection calls to walk a hierarchy. The work was already done and now it's shared.

## 4.0.1

Add PropertyInfo or ParameterInfo to IArgument.Services. This makes it possible for middleware to access them from a command and to determine if an argument is from a parameter or property.

## 4.0.0

### Nullable Reference Types

The library has been updated to support Nullable Reference Types

### Default behavior changes

Version 4 is removing obsolete members and changing default behaviors made possible since the v3 was introduced.

* default `AppSettings.Help.ExpandArgumentsInUsage` to true.
    * arguments are expanded in the usage section of help.
        * old: `add [options] [arguments]`
        * new: `add [options] <x> <y>`
* default `AppSettings.DefaultArgumentSeparatorStrategy` to `EndOfOptions`. See [Argument Separator](../ArgumentValues/argument-separator.md) for details.
    * Help will append ` [[--] <arg>...]` to the usage example when `DefaultArgumentSeparatorStrategy=PassThru`
* make `AppSettings.LongNameAlwaysDefaultsToSymbolName` the only behavior and remove the setting. `LongName` can be removed with `[Option(LongName=null)]`.
    * Look for places in your apps where `[Option(ShortName="a")]` with setting a LongName. If you don't want a LongName then add `LongName=null` otherwise the long name will default from the parameter or property name.
* make `AppSettings.GuaranteeOperandOrderInArgumentModels` the only behavior and remove the setting.
    * see this [Argument Models section](../Arguments/argument-models/#guaranteeing-the-order-of-arguments) for details
* enable [CommandLogger](../Diagnostics/command-logger.md) in `.UseDefaultMiddleware()` as `cmdlog` directive. This can be useful for diagnostics.
* Command.FindOption will throw an exception if the alias is for an operand or subcommand instead of an option. Previously it would return null.

#### Added

* The type `MiddlewareSteps` declares most of the framework defined middleware to make it easier to inject your custom middleware in the desired order. 

#### Changed

* When registering middleware, the OrderInStage parameter has been changed from `int` to `short`.

#### Moved

* `CommandDotNet.Directives.Parse.ParseReporter` - moved to `CommandDotNet.Diagnostics.Parse.ParseReporter`
* `CommandDotNet.Directives.Debugger` - moved to `CommandDotNet.Diagnostics.Debugger`
* `AppConfig.CancellationToken` - moved to `CommandDotNet.CancellationToken`. This enables running nested commands within an interactive session.  See [Ctrl+C and CancellationToken](../OtherFeatures/cancellation.md#interactive-sessions) for more details.

#### Removed or Replaced

* `AppSettings`
    * `MethodArgumentMode` - replaced by `DefaultArgumentMode`
    * `ThrowOnUnexpectedArgument` - replaced by `IgnoreUnexpectedArguments`
    * `AllowArgumentSeparator` - was never used for functionality, only to show `--` in help.
    * `HelpTextStyle` - replaced by `Help.TextStyle`
    * `Help`
        * `GlobalTool` - replaced by `UsageAppName`
* `VersionInfo` - replaced by `AppInfo`
* `ApplicationMetadataAttribute` - replaced by `CommandAttribute`
* `ArgumentAttribute` - replaced by `OperandAttribute`
* `ArgumentMode.Parameter` - replaced by `Operand`
* `InjectPropertyAttribute` - v3 made ctor injection possible and that should be used.
* `Command`, `Option`, `Operand` ctor taking with `Command parent` parameter. `Parent` is now assigned by adding to a command.
* `OptionAttribute`
    * `Inherited` - replaced by `AssignToExecutableSubcommands`
* `Option`
    * `DefaultValue` - use `Option.Default`
    * `Inherited` - replaced by `AssignToExecutableSubcommands`
* `Operand`
    * `DefaultValue` - use `Option.Default`
* `IArgument`
    * `DefaultValue` - use `Option.Default`
* `appRunner.UseDefaultsFromConfig` extension method that returns string, in favor of method with same name returning `ArgumentDefault`
* `TokenCollection` public ctor - Use `Tokenizer.Tokenize` extension method to generate tokens and `TokenCollection.Transform` to transform them. Ensures source tokens are correctly mapped.
* `AnsiConsole` - no longer supported. Use a package like ColorConsole or Pastel.
* `MiddlewareSteps.Help.Stage` & `MiddlewareSteps.Help.Order` - replaced by nested `MiddlewareSteps.Help` classes
* `ServicesExtensions.GetOrAdd<T>` - use `ContextDataExtensions.GetOrAdd<T>`

## 3.6.5

#### Typo suggestions for argument values

Typo Suggestions middleware now also makes suggestions for [for argument values](../Help/typo-suggestions.md#suggestions-for-argument-values).

#### Bug: Subcommands should not be parsed after first operand

Subcommands were still being searched by the parser after the first operand was provided. This bug has been fixed.

## 3.6.4

Reimplement UseErrorHandler(CommandContext, Exception). See updated [documentation](../Diagnostics/exceptions.md).

Fix bug where parser did not fail when an option value was skipped when immediately followed by another option.

## 3.6.3

Remove .UseErrorHandler config method until the bug is fixed. Updated documentation with the approach.

## 3.6.2

* An informative error message will output when a [response file](../ArgumentValues/response-files.md) is not found.
* TokenizerPipeline, on error, 
    * no longer throws a TokenTransformationException
    * now prints error messages to IConsole.Error, logs the exception and return ExitCodes.Error (1)
    * this is best practice for console middleware.
* added `ExitCodes` class with `Success`, `Error` & `ValidationError`. These can be used in the middleware pipeline.
* added MiddlewareSteps.ErrorHandler step to be used to register error handlers
* added CommandLogger.HasLoggedFor and made CommandLogger.Log safe to use before ParseResult is populated. This will be useful for logging on error. See [Exceptions > Printing config information](../Diagnostics/exceptions.md#printing-config-information) for details

## 3.6.1

### Remove extraneous NewLine

A few components could leave an extra NewLine hanging around. This has been removed for cleaner test exception handling.  Affects CommandLogger, FluentValidation, ParseDirective and when AppRunner prints an exception before exit.

## 3.6.0

### Feature

#### Argument Separator following [Posix Guideline](https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html#tag_12_02) 10

> The first -- argument that is not an option-argument should be accepted as a delimiter indicating the end of options. Any following arguments should be treated as operands, even if they begin with the '-' character.

Before this release, all arguments following `--` were captured to `CommandContext.ParseResult.SeparatedArguments` and not made available for parsing. That prevented use of operands with values beginning with `-` or `--` or enclosed in square brackets when directives were not disabled.

So `calculator.exe add -1 -2` was not possible.  With this release, `calculator.exe add -- -1 -2` is possible.  

All arguments following `--` are still captured to `CommandContext.ParseResult.SeparatedArguments`, but now may include values for operands of the current command.  Compare w/ `CommandContext.ParseResult.RemainingOperands` to see if any were mapped into the command.

See [Argument Separator](../ArgumentValues/argument-separator.md) for more help.

As part of this update, `CommandContext.ParseResult.SeparatedArguments` && `CommandContext.ParseResult.RemainingOperands` were changed from `IReadOnlyCollection<Token>` to `IReadOnlyCollection<string>`. 

#### CommandLogger.Log

Make CommandLogger a public class so commands, interceptors and middleware can run it directly.

This makes the last pattern in the [Command Logger](../Diagnostics/command-logger.md) help possible, using an interceptor option to trigger the log.

### API

#### CommandContext.ShowHelpOnExit

You can now trigger help to be display after validation checks have failed. See [help docs](../Help/help.md#printing-help) for details.

#### CommandContext.PrintHelp()

`PrintHelp()` extension method enables printing help from anywhere there's a CommandContext

#### exception.GetCommandContext()

Most every exception that escapes the appRunner will have a CommandContext in the Data property.

Use the `GetCommandContext()` extension method to get it and then PrintHelp or ParseReporter.Report or ...

#### CommandAttribute parse hints

The followingw were added to the CommandAttribute to override AppSettings for the given command.

* `IgnoreUnexpectedArguments` to override `AppSettings.IgnoreUnexpectedArguments`
* `ArgumentSeparatorStrategy` to override `AppSettings.DefaultArgumentSeparatorStrategy`

#### Console Write___ extension methods

Write an object to Console.Out and Console.Error.  The object will be converted to string.

* `Write(this IStandardStreamWriter writer, object value)`
* `WriteLine(this IStandardStreamWriter writer, object value)`

Use `console.Write` and `console.WriteLine` to write to console.Out.

* `Write(this IConsole console, object value)`
* `WriteLine(this IConsole console, object value)`

## 3.5.1

Fix bug: when enabled w/ default configuration, Command Logger no longer outputs unless [cmdlog] is specified

## 3.5.0

### Feature

#### Command Logger

A middleware that will output...

* parse directive results
* optionally some system info: os version, .net version, machine name, ...
* optionally AppConfig
* can configure to include additional info with access to CommandContext

... and then run the command. Output can be forwarded to console or logs or ...

See [Command Logger help](../Diagnostics/command-logger.md) for more details

#### AppSettings.LongNameAlwaysDefaultsToSymbolName 

This setting will default to true in the next major version.

When setting `[Option(ShortName="a")]`, the LongName would no longer default to the parameter or property name and so you'd have to explicitely set the LongName if you wanted both. We want to change that behavior so that if you only want a short name, you'd need to `[Option(ShortName="a", LongName=null)]`.
AppSettings.LongNameAlwaysDefaultsToSymbolName allows us to introduce the behavior in a non-breaking manner until the next release.

[#183](https://github.com/bilal-fazlani/commanddotnet/issues/183)

## 3.4.0

### Feature

#### Enhanced Parse directive

The [parse directive](../Diagnostics/parse-directive.md) has been updated to show 

* argument values
* default values w/ sources 
* input values with sources, including response files if enabled.

Password values are replaced with ***** when the `Password` type is used.

### API

#### TokenCollection public ctor deprecated

Use Tokenizer.Tokenize extension method to generate tokens and TokenCollection.Transform to transform them

## 3.3.0

### Feature

#### Fix bug: support multiple default value config sources

There was a bug when using multiple config sources for default values, i.e. EnvVar and AppSettings.
The last one set would override previous ones. Resolved in this release.

### API

#### add ext method IArgument.IsObscured

Returns true if the type is `Password`

## 3.2.0

### API

#### Disable LogProvider by default

* LogProvider.IsDisabled is set to true in AppRunner static ctor.

#### [Guarantee Operand order in IArgumentModel](../Arguments/argument-models.md/#guaranteeing-the-order-of-arguments)

The order of operands defined in IArgumentModel classes were never deterministic because .Net does not guarantee the order properties are reflected.

[CallerLineNumber] was used in the OperantAttribute ctor to ensure the order is always based on the order properties are defined in the class.

!!! Warning
    The issue wasn't correctly resolved until 4.0.0

## 3.1.0

### Feature

#### Typo Suggestions enhancements

Support mis-ordered keywords: nameuser -> username

Trim results & improve accuracy

#### Support self-contained executable 

[AppInfo](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Builders/AppInfo.cs) interrogates the MainModule and EntryAssembly to determine if the app is run from `dotnet`, as a self-contained executable or a standard executable. This is used for help and to determine the correct app version.

[#197](https://github.com/bilal-fazlani/commanddotnet/issues/197), 
[#179](https://github.com/bilal-fazlani/commanddotnet/issues/179)

### API

#### AppInfo replaces VersionInfo

VersionInfo became a subset of features of AppInfo

#### IArgument.Default and ArgumentDefault

ArgumentDefault was introduced to track the source of a default value. We can determine if it came from source or an external source and the key used.
This is used in later versions for the Parse directive and CommandLogger feature.



## 3.0.2

### Feature

#### .UseDefaultMiddleware()

- added [Typo Suggestions](#typo-suggestions)

#### Typo Suggestions

[Typo Suggestions](../Help/typo-suggestions.md) middleware to suggest commands or options when a provided one wasn't found.

#### AppSettings.ExpandArgumentsInUsage

Expand arguments in the usage section so the names and order of all arguments are shown.

See [help docs](../Help/help.md#expandargumentsinusage) for more details.

[#186](https://github.com/bilal-fazlani/commanddotnet/issues/186), 

### API

#### Setting parent commands
Option & Operand & Command should now be created without a parent command. Parent will be assigned when added to a command.



## 3.0.1

### Feature

#### [%UsageAppName%](../Help/help.md#usageappname-tempate) 
To show usage examples in a command description, extended help or overridden usage section, use %UsageAppName%. This text will be replaced usage app name from one of the options above. See [the help docs](../Help/help.md#usageappname-tempate) for more.

### API

#### improve stack traces with AppOutputBase and PathMap project settings
```
<AppOutputBase>$(MSBuildProjectDirectory)\</AppOutputBase>
<PathMap>$(AppOutputBase)=CommandDotNet/</PathMap>
```
