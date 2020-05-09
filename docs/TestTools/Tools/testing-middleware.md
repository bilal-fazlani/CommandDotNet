# Testing Middleware

Testing middleware and the extensibility points generally requires access to the `CommandContext` and `AppConfig`.

## Accessing CommandContext

=== "RunInMem"

    ```c#
    [Test]
    public void SomeTest()
    {
        var result = new AppRunner<App>().RunInMem("some args");
        var context = result.CommandContext;
    }
    ```

=== "BDD Verify"

    ```c#
    public void SomeTest()
    {
        var result = new AppRunner<App>()
            .Verify(new Scenario
            {
                When = {Args = "some args"},
                Then =
                {
                    // Action<CommandContext> to perform assertions
                    AssertContext = ctx => ...
                }
            });
        var context = result.CommandContext;
    }
    ```

Both `RunInMem` and `Verify` return a CommandContext to use for further assertions.

Verify also has the Scenario.Then.AssertContext callback for a more fluid syntax approach.

## Accessing invocation values

When your middleware can modify the invocation, such as parameter values and class instances, you'll want to access the invocation metadata to verify. 

There are a few [CommandContext extension methods](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/CommandContextTestExtensions.cs) to make this easier.

To get the [InvocationStep](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Execution/InvocationStep.cs), which contains the Command, Invocation and Instance, use:

* `GetCommandInvocationInfo()` to get the invocation of the command.
* `GetCommandInvocationInfo<TCommandClass>()` to get the invocation of the command with the `.Instance` typed as `TCommandClass`.
* `GetInterceptorInvocationInfo<TInterceptorClass>()` to get the invocation of the interceptor in the specified class with the `.Instance` typed as `TInterceptorClass`.

The `InvocationInfo` includes the properties for [InvocationStep](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Execution/InvocationStep.cs) and [Invocation](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Execution/IInvocation.cs). The xml comments in the linked source files indicate which stage each property is loaded.


The `InvocationInfo` properties `ArgumentParameters` and `ArgumentParameterValues` include only those parameters that define arguments. They do not include parameters of type InterceptorExecutionDelegate, CommandContext, IConsole or  any other type configured as a parameter resolver. Those are available in via `NonArgumentParameters` and `NonArgumentParameterValues`

The `InvocationInfo` includes a `WasInvoked` property. To use this, configure the AppRunner with `.TrackInvocations()`.  A friendly error message will remind you if your forget.

### Example

We'll start with the following WebClient with a single *Download* command and an [interceptor method](../../Extensibility/interceptors.md) to authenticate commands.  You can imagine there would be other commands like Upload, Post, Get, etc.

```c#
public class WebClient
{
    public void Authenticate(InterceptorExecutionDelegate next, 
                            string user, string pwd)
    {
        _client.Connect(user, pwd);
        return next();
    }

    public void Download(string url, IConsole console)
    {
        _client.Find(filter).ForEach(i => console.Out.WriteLine(i.Id));
    }
}
```

And then we can assert some values passed to the methods like this.

=== "RunInMem"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void SomeTest()
        {
            var result = new AppRunner<WebClient>()
                .TrackInvocations()
                .RunInMem("--user me --pwd shhhh Download http://all-the-internet.com");

            var context = result.CommandContext;

            var invocation = context.GetCommandInvocationInfo<WebClient>();
            invocation.WasInvoked.Should().Be(true);
            invocation.ArgumentParameterValues.First().Should()
                .Be("http://all-the-internet.com");

            invocation = context.GetInterceptorInvocationInfo<WebClient>()
            invocation.WasInvoked.Should().Be(true);
            invocation.ArgumentParameterValues.Should()
                .Be(new []{"me", "shhhh"});
        }
    }
    ```

=== "BDD Verify"

    ```c#
    public void SomeTest()
    {
        new AppRunner<WebClient>()
            .TrackInvocations()
            .Verify(new Scenario
            {
                When = {Args = "--user me --pwd shhhh Download http://all-the-internet.com"},
                Then =
                {
                    AssertContext = ctx => 
                    {
                        var invocation = ctx.GetCommandInvocationInfo<WebClient>();
                        invocation.WasInvoked.Should().Be(true);
                        invocation.ArgumentParameterValues.First().Should()
                            .Be("http://all-the-internet.com");

                        invocation = ctx.GetInterceptorInvocationInfo<WebClient>()
                        invocation.WasInvoked.Should().Be(true);
                        invocation.ArgumentParameterValues.Should()
                            .Be(new []{"me", "shhhh"});
                    }
                }
            });
    }
    ```

!!! Tip
    CommandDotNet has an extensive set of tests like this to verify all of it's features.
    You should only need to test invocations when verifying custom middleware.