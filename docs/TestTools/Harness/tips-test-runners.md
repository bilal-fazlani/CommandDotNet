# Tips for NUnit & XUnit

These tips apply when using the [RunInMem](run-in-mem.md) or [BDD Verify](bdd.md) harnesses.

## Tips for NUnit

### TestConfig & IDefaultTestConfig

Configure the [TestConfig](test-config.md) in the AssemblySetup to avoid the one-time reflective cost of looking for an [IDefaultTestConfig](test-config.md#idefaulttestconfig)

### Ignore logLine

logLine will use Console.Out when not provided and NUnit handles this well so logs will output to the console. 

## Tips for XUnit

XUnit is a bit more cumbersome than NUnit when you want to log results to the test output.

### TestConfig & IDefaultTestConfig

XUnit does not have a concept like AssemblySetup so you'll want to create an [IDefaultTestConfig](test-config.md#idefaulttestconfig)

### logLine and ITestOutputHelper

To log results to the test output, you'll need to use their [ITestOutputHelper](https://xunit.net/docs/capturing-output).

Set `logLine` = `_testOutputHelper.WriteLine`.

```c#
new AppRunner<Git>().RunInMem(args, _testOutputHelper.WriteLine)
```

### AsyncLocal helper

The CommandDotNet tests use the following extension methods and ambient ITextOutputHelper class to simplify our tests. This gives us a similar experience to NUnit except we still need a constructor for every test class.

```c#
public static class Ambient
{
    private static readonly AsyncLocal<ITestOutputHelper> TestOutputHelper = new AsyncLocal<ITestOutputHelper>();

    public static ITestOutputHelper Output
    {
        get => TestOutputHelper.Value;
        set => TestOutputHelper.Value = value;
    }

    public static Action<string> WriteLine
    {
        get
        {
            var output = Output;
            if (output == null)
            {
                throw new InvalidOperationException($"{nameof(Ambient)}.{nameof(Output)} has not been set for the current test");
            }

            return output.WriteLine;
        }
    }
}

public static class AppRunnerScenarioExtensions
{
    public static AppRunnerResult RunInMem(
        this AppRunner runner,
        string args,
        Func<TestConsole, string> onReadLine = null,
        IEnumerable<string> pipedInput = null)
    {
        return runner.RunInMem(args, Ambient.WriteLine, onReadLine, pipedInput);
    }

    public static AppRunnerResult RunInMem(
        this AppRunner runner,
        string[] args,
        Func<TestConsole, string> onReadLine = null,
        IEnumerable<string> pipedInput = null)
    {
        return runner.RunInMem(args, Ambient.WriteLine, onReadLine, pipedInput);
    }

    public static AppRunnerResult Verify(this AppRunner appRunner, IScenario scenario)
    {
        // use Test.Default to force testing of TestConfig.GetDefaultFromSubClass()
        return appRunner.Verify(Ambient.WriteLine, TestConfig.Default, scenario);
    }
}
```
and we can use it in our tests like this

```c#
public class GitTests
{
    public GitTests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Fact]
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
}
```

instead of 

```c#
public class GitTests
{
    private readonly ITestOutputHelper _output;

    public GitTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Checkout_NewBranch_WithoutBranchFlag_Fails()
    {
        new AppRunner<Git>()
            .UseDefaultMiddleware()
            .Verify(_output.WriteLine, new Scenario
            {
                When = { Args = "checkout lala" },
                Then =
                {
                    ExitCode = 1,
                    Output = "error: pathspec 'lala' did not match any file(s) known to git"
                }
            });
    }
}
```
