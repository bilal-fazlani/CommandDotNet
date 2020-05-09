# Default middleware

CommandDotNet has the notion of core middleware and optional middleware as described in [the Middleware Pipeline](../Extensibility/middleware.md).

Core middleware is the minimum set of middleware required to process arguments. They are always part of the pipeline and cannot be removed.

Optional middleware can be injected into the middleware pipeline before arguments are processed.

```c#
static int Main(string[] args)
{
    return new AppRunner<ValidationApp>()
        .UseCancellationHandlers()
        .UseDebugDirective()
        .UseParseDirective()
        .UsePrompting()
        .UseResponseFiles()
        .UseVersionMiddleware()
        .AppendPipedInputToOperandList()
        .UseTypoSuggestions()
        .Run(args);
}
```

## Configuration fatigue
Repeating this configuration for every console app makes it very clear what additional features are being used.

CommandDotNet will release new features as new middleware. To take advantage of them, devs will need to call additional `.UseNewMiddleware` methods.

Some developers very much prefer this approach and it will always be available to them.

For other developers, this creates configuration fatigue. The fatigue of remembering which middleware to add for new apps, scanning the configuration everytime they're in the Main method, tracking new features to enable with updates to the package. Some apps and development styles do not benefit from additional work. `UseDefaultMiddleware` is the answer.

## Default middleware

To avoid the clutter and fatigue of enabling built-in middleware one-by-one, we've provided the `UseDefaultMiddleware` extension.

```c#
static int Main(string[] args)
{
    return new AppRunner<ValidationApp>()
        .UseDefaultMiddleware()
        .Run(args);
}
```
`UseDefaultMiddleware` will register all non-core middleware that does not require an external dependency and can be run with sensible defaults configurations.

This is how CommandDotNet releases new features. With `UseDefaultMiddleware`, developers are opting into new features by default.

!!! Note
    New versions of the library may include new middleware in this method. New features can change the behavior of your software, just as modifications to existing features can. This will be indicated as a minor semantic version update to the package.

!!! Note
    This list will never include middleware that requires an external dependency, like `FluentValidator`, `IoC` and `Humanizer`

## Excluding middleware

There will be times when you need to exclude a single middleware but want to keep all others.  

Each middleware has a corresponding `exclude...` parameter, as shown in the `UseDefaultMiddleware` method at the bottom of the page.

```c#
static int Main(string[] args)
{
    return new AppRunner<ValidationApp>()
        .UseDefaultMiddleware(excludeVersionMiddleware: true)
        .Run(args);
}
```

## Overriding default middleware settings

Some middleware accept options to modify behavior. To provide options, first exclude the middleware from the default and then call the middleware's extension method.

```c#
static int Main(string[] args)
{
    return new AppRunner<ValidationApp>()
        .UseDefaultMiddleware(excludePrompting: true)
        .UsePrompting(promptForMissingArguments: false)
        .Run(args);
}
```

## UseDefaultMiddleware

see [the source](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/AppRunnerConfigExtensions.cs#L24) for the current list 

... at the time of this writing

```c#
public static AppRunner UseDefaultMiddleware(this AppRunner appRunner,
    bool excludeCancellationHandlers = false,
    bool excludeDebugDirective = false,
    bool excludeParseDirective = false,
    bool excludePrompting = false,
    bool excludeResponseFiles = false,
    bool excludeVersionMiddleware = false,
    bool excludeAppendPipedInputToOperandList = false,
    bool excludeTypoSuggestions = false)
{
    if (!excludeCancellationHandlers) appRunner.UseCancellationHandlers();
    if (!excludeDebugDirective) appRunner.UseDebugDirective();
    if (!excludeParseDirective) appRunner.UseParseDirective();
    if (!excludePrompting) appRunner.UsePrompting();
    if (!excludeResponseFiles) appRunner.UseResponseFiles();
    if (!excludeVersionMiddleware) appRunner.UseVersionMiddleware();
    if (!excludeAppendPipedInputToOperandList) appRunner.AppendPipedInputToOperandList();
    if (!excludeTypoSuggestions) appRunner.UseTypoSuggestions();

    return appRunner;
}
```


