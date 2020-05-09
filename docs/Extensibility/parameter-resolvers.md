# Parameter Resolvers

Parameters defined in command and interceptor methods must be a valid paramter type as defined in [Supported Argument Types](../Arguments/argument-types.md).

There are times where your command method needs access to a service in the `CommandContext`, 
say to write to the `IConsole` or perform a loop while checking the `CancellationToken`.

!!! tip
    Use `IConsole` in your methods instead of Console to simplify unit tests. [TestConsole](../TestTools/overview.md#testconsole) captures output and can mimic  input.  

One way to get these is to define an interceptor method with a `CommandContext` parameter and copy the properties to fields for use in other methods. 
This quickly gets repetitive and cumbersome.

Parameter resolvers are functions that, given a `CommandContext`, return an instance of a type that can be
passed to a command or interceptor method.  The types returned are valid types to use in command and interceptor methods and command class constructors. 

There are three default resolvers for: `CommandContext`, `IConsole` & `CancellationToken`.

Example of registering a new parameter resolver for `SomeService`

``` c#
appRunner.Configure(b => b.UseParameterResolver(ctx => ctx.Services.Get<SomeService>()));
```

``` c#
public class Calculator
{
    private ICalculator _calculator;

    public Calculator(ICalculator calculator) => _calculator = calculator;

    public Task<int> Interceptor(InterceptorExecutionDelegate next, SomeService someService)
    {
        return next();
    }
    
    [Command(Description = "Adds two numbers")]
    public void Add(int value1, int value2, IConsole console)
    {
        console.WriteLine(_calculator.Add(value1, value2));
    }

    [Command(Description = "Subtracts the second number from the first")]
    public void Subtract(int value1, int value2, IConsole console)
    {
        console.WriteLine(_calculator.Subtract(value1, value2));
    }
}
```

!!! warning
    It will be tempting to use this as a form of dependency injection, but it is a service locator pattern.
    The DI container will not understand the relationships between your dependencies, limiting the containers usefulness in some cases.

!!! tip
    Use this feature for middleware and infrasturctural components that can be used without DI.