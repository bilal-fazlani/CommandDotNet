# Developing Middleware

Here are some of the tips we've picked up while developing middleware and other extensibility components for CommandDotNet.

## Diagnostics tools are your friend

Take advantage the [debug](../Diagnostics/debug-directive.md), [parse](../Diagnostics/parse-directive.md) & [command logger](../Diagnostics/command-logger.md) features.

## TestTools

The test tools were developed specifically to make it easier to test middleware. Checkout [testing middleware](../TestTools/Tools/testing-middleware.md) for some tips.

## Descriptive method names

When registering a middlware delegate via `.UseMiddleware(delegate, step)`, use a method instead of a delegate and give the method an informative name so it's clear when it shows up in an exception stacktrace or [command logger](../Diagnostics/command-logger.md) output

```
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
```
## Specifying middleware order

Registration is broken into separate stages as described [here](../Extensibility/middleware.md).

You can register middleware using `appRunner.Configure(c => c.UseMiddleware(MyMethod, MiddlewareStages.PreTokenize))`. 
Middleware will be added in the order you provide for each stage.

```c#
appRunner.Configure(c => 
    c
        .UseMiddleware(MiddlwareA, MiddlewareStages.PreTokenize)
        .UseMiddleware(MiddlwareB, MiddlewareStages.PostParseInputPreBindValues)
        .UseMiddleware(MiddlwareC, MiddlewareStages.PreTokenize)
    ))
```

In this example, MiddlewareC will run immediately after MiddlewareA in the PreTokenize stage and MiddlewareB will run afterwards in the PostParseInputPreBindValues stage.

### Specifying order relative to a framework middleware

If you need to ensure your middleware needs to run immediately before or after a framework middleware, you can use the static class `MiddlewareSteps` to get the order for the given middleware.

For example, if you need middleware to run before `MiddlewareSteps.DebugDirective`...

```c#
appRunner.Configure(c => 
    c
        .UseMiddleware(MiddlwareA, MiddlewareSteps.DebugDirective - 1)
    ))
```

These values of the order can change between releases always use the MiddlwareSteps value. 

The steps are generally separated by at least a value of 1000 and relative to zero, short.Min or short.Max.  

## Middleware Config

It's convenient to use a delegate for middleware when you have additional parameters to pass in. 
The pattern we've adopted in CommandDotNet uses a private `Config` class to keep the parameters.

```c#
public static FluentValidationMiddleware
{
    public static AppRunner UseFluentValidation(this AppRunner appRunner, bool showHelpOnError = false)
    {
        return appRunner.Configure(c =>
        {
            c.UseMiddleware(ValidateModels, MiddlewareSteps.FluentValidation);
            c.Services.Add(new Config(showHelpOnError));
        });
    }
    
    private class Config
    {
        public bool ShowHelpOnError { get; }
        public Config(bool showHelpOnError) => 
            ShowHelpOnError = showHelpOnError;
    }

    private static Task<int> ValidateModels(CommandContext ctx, ExecutionDelegate next)
    {
        var showHelpOnError = ctx.AppConfig.Services.GetOrThrow<Config>().ShowHelpOnError;
        ... 
    }
}
```

This example is from our FluentValidation middleware. Notice `UseMiddleware` is called before adding the config. 
This is intentional because the `UseMiddleware` method will throws an informative error message when a middleware is registered multiple times.
Adding a duplicate service throws duplicate key exception and is less actionable.

`UseTokenTransformation` and `UseParameterResolver` also throw informative error messages so use one of those three methods first to take advantage of the duplicate registration checks.

## Registering middleware multiple times

By default, an exception will be thrown if a middleware delegate is registered more than once.  Override this with the option `allowMultipleRegistrations` parameter in the `UseMiddleware` registration method. `UseMiddleware(MyMethod, step, allowMultipleRegistrations: true)`

## Error messages

When possible, middleware should handle it's own errors, printing a message to the console and returning an exit code.

If help should be shown, set `commandContext.ShowHelpOnExit=true`. 
