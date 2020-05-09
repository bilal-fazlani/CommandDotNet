# TestCaptures

!!! Warning
    TestCaptures have been obsoleted. Use the CommandContext extension methods `GetCommandInvocation` and `GetInterceptorInvocation<TInterceptorClass>` to access the `IInvocation.ParameterValues`.
    See [Testing Middleware](../Tools/testing-middleware.md) for details.

[TestCaptures](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestCaptures.cs) are used to test middleware components. They should not be used to test your command methods since they require adding `TestCaptures` as a property to the class defining the commands to test.

Let's start with an example

``` c#
public class FlagTests
{
    [Fact]
    public void ClubbedOptionsAreRecognized()
    {
        new AppRunner<FlagApp>()
            .Verify(new Scenario
            {
                When = { Args = "club -ab" },
                Then =
                {
                    Captured = { new ClubResult(true, true) }
                }
            });
    }

    private class FlagApp
    {
        // will be injected by the test framework
        private TestCaptures TestCaptures { get; set; }

        public int Club(
            [Option] bool a,
            [Option] bool b)
        {
            TestCaptures.Capture(new ClubResult(a, b));
        }
    }

    public class ClubResult
    {
        public bool A { get; }
        public bool B { get; }

        public ClubResult(bool a, bool b)
        {
            A = a;
            B = b;
        }
    }
}
```

Add `TestCaptures` as a property to the command class and it will be automatically injected by the test framework.

`TestCaptures` is a wrapper for a dictionary keyed by typed. Only one instance of any type can be captured. Classes like ClubResult are useful for capturing the content from multiple arguments. When used with [BDD Verify](bdd.md) the types must match exactly. Inheritance is not checked. 

This is useful for testing middleware and other extensibility mechanisms that can populate or modify arguments.

`TestCaptures` are also added to the `CommandContext.Services` so you could add middelware for tests to capture values in a more elegant way.  If you improve on our patterns, please let us know and submit a PR so we can include it in the tooling.