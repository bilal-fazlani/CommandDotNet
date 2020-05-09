# Dependency Injection

This document assumes you're already familiar with the Dependency Injection (DI) pattern. If not, the frameworks listed below describe the pattern in their documentation.

## TLDR, How to enable
=== "MicrosoftDependencyInjection"
    === ".NET CLI"

        ```
        dotnet add package CommandDotNet.IoC.MicrosoftDependencyInjection
        ```

    === "Nuget Package Manager"

        ```
        Install-Package CommandDotNet.IoC.MicrosoftDependencyInjection
        ```
    Enable [MicrosoftDependencyInjection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1) with <br/> `appRunner.UseMicrosoftDependencyInjection(...)`

=== "Autofac"
    === ".NET CLI"

        ```
        dotnet add package CommandDotNet.IoC.Autofac
        ```

    === "Nuget Package Manager"

        ```
        Install-Package CommandDotNet.IoC.Autofac
        ```
    Enable [Autofac](https://autofac.org/) with <br/> `appRunner.UseAutofac(...)`

=== "SimpleInjector"
    === ".NET CLI"

        ```
        dotnet add package CommandDotNet.IoC.SimpleInjector
        ```

    === "Nuget Package Manager"

        ```
        Install-Package CommandDotNet.IoC.SimpleInjector
        ```
    Enable [SimpleInjector](https://simpleinjector.org/) with <br/> `appRunner.UseSimpleInjector(...)`

=== "Custom & 3rd Party"
    `No package required`
 
    Enable the [custom resolver](#custom-resolvers) with <br/> `appRunner.UseDependencyResolver(myCustomContainer, ...)`

## Configuration options

When a resolver is registered, it will be used to resolve instances for 

* command classes using `IDependencyResolver.Resolve` 
* `IArgumentModel` classes using `IDependencyResolver.TryResolve`.

DI containers have different behaviors for `Resolve` and `TryResolve`. In most cases, `Resolve` will throw an exception if an instance isn't registered. Some containers, like MicrosoftDependencyInjection, will return null instead. When `TryResolve` is used or `Resolve` returns null, CommandDotNet will attempt to instantiate an instance.

`UseDependencyResolver` contains the following configuraion parameters:

```c#
public static AppRunner UseDependencyResolver(
    ...
    Func<CommandContext, IDisposable> runInScope = null,
    ResolveStrategy argumentModelResolveStrategy = ResolveStrategy.TryResolve,
    ResolveStrategy commandClassResolveStrategy = ResolveStrategy.Resolve,
    bool useLegacyInjectDependenciesAttribute = false
}
```

* __runInScope__: if provided, the scope will be created at the beginning of the run and disposed at the end
* __argumentModelResolveStrategy__: the `ResolveStrategy` used to resolve `IArgumentModel` classes.
* __commandClassResolveStrategy__: the `ResolveStrategy` used to resolve command classes.
* __useLegacyInjectDependenciesAttribute__: when true, resolve instances for command class properties marked with `[InjectProperty]`. This feature is deprecated and may be removed with next major release.

These parameters also exist for the AutoFac, MicrosoftDependencyInjection and SimpleInjector packages

## Custom Resolvers

To implement another 3rd party resolver, follow the example for [one of the existing ones](https://github.com/bilal-fazlani/commanddotnet). Submit a PR so we can keep it up-to-date for you. 

To implement your own custom resolver, implement the `IDependencyResolver`. See the [TestDependencyResolver](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestDependencyResolver.cs) for an example.

## Registering all command classes

Use `appRunner.GetCommandClassTypes()` to get all the command class types that could be instantiated from the AppRunner.

```c#
private static void RegisterSimpleInjector(AppRunner appRunner)
{
    var container = new SimpleInjector.Container();
    foreach(Type type in appRunner.GetCommandClassTypes())
    {
        container.Register(type);
    }
    appRunner.UseSimpleInjector(container);
}
```