# Testing Prompts

Prompts 

Prompts are easily tested using the `Resond` methods to provide `Answer`s for the OnPrompt value.

Notice `Respond.WithText` in the examples below.

=== "RunInMem"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            var result = new AppRunner<App>()
                .UsePrompting()
                .RunInMem("TellAJoke", onPrompt: Respond.WithText("who's there"));

            result.ExitCode.Should().Be(0);
            result.OutputShouldBe(@"knock knock: who's there
    who's there
    ");
        }
    }

    private class App
    {
        public void TellAJoke(IConsole console, IPrompter prompter)
        {
            var answer = prompter.PromptForValue("knock knock", out _);
            console.Out.WriteLine(answer);
        }
    }
    ```

=== "BDD Verify"

    ```c#
    public void InjectedPrompterCanPromptForValues()
    {
        new AppRunner<App>()
            .UsePrompting()
            .Verify(new Scenario
            {
                When = 
                {
                    Args = "TellAJoke",
                    OnPrompt = Respond.WithText("who's there")
                },
                Then =
                {
                    Output = @"knock knock: who's there
    who's there
    "
                }
            });
    }

    private class App
    {
        public void TellAJoke(IConsole console, IPrompter prompter)
        {
            var answer = prompter.PromptForValue("knock knock", out _);
            console.Out.WriteLine(answer);
        }
    }
    ```

=== "TestConsole"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            var promptResponder = Respond.WithText("who's there");
            var testConsole = new TestConsole(onReadKey: promptResponder.OnReadKey);
            var appRunner = new AppRunner<App>()
                .Configure(c => c.Console = testConsole);

            // remaining test code
        }
    }
    ```

Use this same pattern when verifying prompts as expected for missing arguments.

For more examples, see our [prompting tests](https://github.com/bilal-fazlani/commanddotnet/tree/master/CommandDotNet.Tests/FeatureTests/Prompting)

## Respond

The Respond class has several helper methods. They all return an IPromptResponder populated with one or more `IAnswer`s.

`IAnswers` have the follwing properties:

* *ConsoleKeys*: the `ConsoleKeyInfos` to return.
* *Reuse*: when false, the Answer will be discarded after first use.
* *PromptFilter*: a predicate to determine if the answer should be used. 
    * The last line of the console output is used the value passed to the predicate.
* *ShouldFail*: when true, a `UnexpectedPromptFailureException` will be thrown


### Respond.WithText

`Respond.WithText` converts a text string to a collection of `ConsoleKeyInfos`.

Optional paramaters: `promptFilter` and `reuse`

### Respond.WithList

`Respond.WithList` converts a collection of text strings to a collection of `ConsoleKeyInfos`

```c#
OnPrompt = Respond.WithList(new []{"a","b","c"});
```

Optional paramaters: `promptFilter` and `reuse`

### Respond.With(Answers)

`Respond.With` is used when more than one prompt answer is required.

```c#
OnPrompt = Respond.With(
    new TextAnswer("groceries", 
        prompt => prompt == "enter list name:"),
    new ListAnswer(new[] {"apples", "bananas", "cherries"},
        prompt => prompt == "enter items:"))
```

### Respond.FailOnPrompt

`Respond.FailOnPrompt` creates an answer with `ShouldFail=true`, resulting in a `UnexpectedPromptFailureException` on prompt.

Optional paramaters: `promptFilter`
