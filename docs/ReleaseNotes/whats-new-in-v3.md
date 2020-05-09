# Version 3 Change Summary

Version 3 was a significant rewrite of CommandDotNet version 2.

The framework has remained mostly backward compatible for basic scenarios. 
Anything beyond basic scenarios will encounter a breaking change.

We welcome everyones help in testing the changes, providing usability feedback, bugs, missing documentation, etc.

Given the scope of changes, it's likely something is missed in this doc. If you notice something, please create an issue or submit a pull request for the docs.  Thank you.

Check build warnings after updating and update obsolete references.  They'll be removed in the next major update.

## What are the changes?

### New architecture
The architecture is now based around a [middleware pipeline](../Extensibility/middleware.md).

### Arguments terminology
[Argument terminology](../argument-terminology.md) has been updated and made consistent throughout the framework. The term "Parameters" is generally replaced with "Arguments".

### Constructor based dependency injection is now possible
[read more below](#interceptor-methods)

## Summary of new features

* [Response file](../ArgumentValues/response-files.md) support 
* [HelpTextProvider](../Help/help.md) can be overridden to make targetted changes to a section of help.
* [Test tools](../TestTools/overview.md), helpful for end-to-end test and testing framework extensions, like middleware components.
* [Parameter resolvers](../Extensibility/parameter-resolvers.md).
* [Ctrl+C support](../OtherFeatures/cancellation.md) with CancellationToken.
* [Arity](../Arguments/argument-arity.md) calculated for arguments and can be updated in middleware.
* [Password](../Arguments/passwords.md) type added to prevent accidental logging of password values and hide passwords during prompting.
* [Prompt](../ArgumentValues/prompting.md) tool can be used directly in methods and honors Password and Ctrl+C and has several extensibility points.
* Lists parameters can be defined as arrays or enumerables that can be streamed from piped input or files.
* [Piped input](../ArgumentValues/piped-arguments.md) can be mapped to operand lists.
* SimpleInjector container support.
* IoC runInScope to enable isolated instances in each run.
* `IArgumentModel` instances can be resolved from containers. Defaults can be populated from configs and those values will appear in help.
* [Newer Release Alerts](../OtherFeatures/newer-release-alerts.md) to alert users when running an older verion of your application.
* External dependencies Humanizr and FluentValidation have been extracted to nuget packages: CommandDotNet.NameCasing & CommandDotNet.FluentValidation.

Several bugs were fixed along the way.

## Breaking Changes

We initially tried to roll out this update in backward compatible phases. Due to the scope of changes, that proved more burdonsome that it was worth. We decided the benefits were worth the price of breaking changes. Hopefully you'll agree.

In some cases, i.e. renamed attributes, the old method or class has been marked with `[Obsolete]` and warnings will suggest how to upgrade.  These will be removed in the next major release, `4.x`

There are a set of changes your IDE should be able to help you with.  For example, namespaces were updated to reflect their functional purpose. Your IDE will likely suggest the fixes and then you can remove the old namespaces. 

### AppSettings & Configuration

The following have been disabled by default and moved to middelware configurations

* Version option: `appRunner.UseVersionMiddleware()`
* Prompting for missing arguments: `appRunner.UsePrompting(...)`
* Name casing: `appRunner.UseNameCasing(...)` via nuget: CommandDotNet.NameCasing
* Fluent validation: `appRunner.UseFluentValidation()` via nuget: CommandDotNet.FluentValidation nuget

* `AppSettings.Help.UsageAppNameStyle`: `UsageAppNameStyle.Adaptive` no longer defaults to `UsageAppNameStyle.Global`. Assign the global tool name to `AppSettings.Help.UsageAppName` instead.

### Interceptor Methods

#### Interceptor Methods replace Constructor Options 

Constructor-based dependency injection was not possible because constructors were used to define options that can be used for all subcommands defined in that class prevented.

Those constructors will need to be replaced with interceptor methods.  There are two signatures for interceptor methods.

`Task<int> Interceptor(CommandContext ctx, ExecutionDelegate next, ...)`

and

`Task<int> Interceptor(InterceptorExecutionDelegate next, ...)`

The method name does not matter.  What does matter is the `Task<int>` return type and use of either `ExecutionDelegate` or `InterceptorExecutionDelegate`.  The former requires a `CommandContext`. Options can be defined in these methods but are not required.

#### Interceptor Methods scope includes all ancestor commands

Constructor options were only avialable for subcommands defined within the same class as the constructor.
This lead to non-obvious behavior with multi-level subcommands where users could supply options that were never used.

Example, let's say we have a console app `api.exe` for an api and the app mimics the RESTful design of the api. 

``` c#
public class Api
{
    public Api(string url){ ... }

    public Users Users{ get; set; }
}

public class Users
{
    public void List()
}
```

The commands are:

* `ApiApp` - the root app, with a constructor defining the option `--url`
  * `Users` - command from a nested class
    * `List` - a method to list the users

Usage: `api --url {api-url} users list`

In v2 the Api class would not be instantiated and the --url option would be dropped.  

v3 fixes this by keeping a pipeline of all interceptors.

If you need the old behavior, use the following code to keep only the interceptor in the same class as the target command. 

``` c#
appRunner.Configure(c => c.UseMiddleware((ctx, next) =>
{
    var pipeline = ctx.InvocationPipeline;
    pipeline.AncestorInterceptors = pipeline.All
        .Where(s => s.Invocation.MethodInfo.DeclaringType == pipeline.TargetCommand.Invocation.MethodInfo.DeclaringType)
        .ToList();
    return next(ctx);
}, MiddlewareStages.PostParseInputPreBindValues));
```

### Configuration

Use `AppRunner.Configure` for any configuration not located in AppSettings. This includes setting a custom help text provider, dependency resolver, etc.

### CommandInvoker removed
The middleware pipeline architecture obsoleted the CommandInvoker. Look at the `CommandInvokerTests` in the repo to see how the feature can be implemented using middleware. Where the CommandInvoker only worked for the command method, the new architecture allows interogating all of the interceptor methods.

### Prompt for list values
Delimited by line instead of by comma. Enter an empty line to submit the list.