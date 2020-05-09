# BDD Verify

The BDD style uses the `Verify` extension method which wraps [RunInMem](run-in-mem.md) with assertions defined in a BDD manner using [object initializers](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers) for some syntactic goodness.


```c#
[Test]
public void Checkout_NewBranch_WithoutBranchFlag_Fails()
{
    new AppRunner<Git>()
        .UseDefaultMiddleware()
        .Verify(new Scenario
        {
            When = { Args = "checkout lala" },
            Then =
            {
                ExitCode = 1,
                Output = "error: pathspec 'lala' did not match any file(s) known to git"
            }
        });
}
```

## Orchestration

First, it calls `RunInMem` and finally it returns an `AppRunnerResult`, so all options for `RunInMem` are available in `Verify`.

Additionally it

* Captures any exceptions, returns exit code of 1 and outputs the message to the console, as you would see in the shell.
* Asserts
    * ExitCode... asserts is 0 when ExitCode is specified
    * Console Output
    * TestCaptures

This approach works well with BDD frameworks like [SpecFlow](https://specflow.org/) where scenarios can be defined in other sources and mapped to code.

## More on BDD

Verify uses the Scenario to follow the Given-When-Then BDD syntax.

### Given
**Given** is the context, which is the AppRunner with configurations. If it helps, when you read `new AppRunner<Git>...`, replace `new` with `given` so it becomes `given AppRunner<git>...`

### When
**When** is the action, which is the user input. It always starts with Args, or an ArgsArray if you're testing a scenario the CommandLineStringSplitter doesn't split as expected.  In addtiion to args, users can provide piped input and answer prompts

```c#
When = 
{
    Args = $"Knock knock",
    PipedInput = new[] {"orange", "orange you glad I didn't say banana?"},
    OnPrompt = Respond.WithText("rose"),
    OnReadLine = console => "blue"
}
```
These are the option available. In practice, you'll only provide one of `PipedInput`, `OnPrompt` or `OnReadLine`. When `PipedInput` is available, System.Console does not allow Console.Read so you can either have piped input or prompt. `OnPrompt` registers a delegate for `OnReadLine` will overrule any `OnReadLine` delegate provided.

See [Testing Piped Input](../Tools/testing-piped-input.md) and [Testing Propmts](../Tools/testing-prompts.md) for more on those topics.

### Then
**Then** is the assertion, which is generally the console output.

```c#
Then =
{
    ExitCode = 1,
    Output = "exact match",
    OutputContainsTexts = 
    {
        "this", "and this", "and also this"
    },
    OutputNotContainsTexts = 
    {
        "but not this", "or this", "and definitely not this"
    },
    AssertOutput = output => output.Should()...,
    AssertContext = context => 
    {
        context.Should()...,
        context.GetCommandInvocation().ParameterValues.Should()...
    }
    Captured =
    {
        new SomeExpectedObject{AnOperand="some-value"}
    },
    AllowUnspecifiedCaptures = true
}
```

**ExitCode** is always checked. If a value is not provided, 0 is used.

**Output** is the ordered merge of Standard Out and Error Out and expects an exact match. For some output, this can be brittle and `contains` check is a better choice.

**OutputContainsTexts** and **OutputNotContainsTexts** will check for the presence of the strings in the output.

**AssertOutput** is an `Action<string>` containing the console output. Use this for more complex assertions.

**AssertContext** is an `Action<CommandContext>`. Use this to assert on items in the context.

**Captured** will check if [TestCaptures](test-captures.md) contains the given objects.

**AllowUnspecifiedCaptures** when true, [TestCaptures](test-captures.md) can contain objects not specified in `Captured`. When false, if an object in [TestCaptures](test-captures.md) is not matched by an object in `Captured` an exception is thrown.

!!! Obsoleted
    *Captured* and *AllowUnspecifiedCaptures* have been obsoleted in favor of *AssertContext => ctx.GetCommandInvocation().ParameterValues...* and using the Assert library of your choice to verify parameters are wat is expected.
