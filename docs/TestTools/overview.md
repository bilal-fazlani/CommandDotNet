# Overview

#### TLDR, How to enable 

nuget package: [CommandDotNet.TestTools](https://www.nuget.org/packages/CommandDotNet.TestTools)

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet.TestTools
    ```
    
=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet.TestTools

    ```

## Testing Console Apps

One of the perks of using this framework is that commands are just methods and methods are easily unit tested. Most of your tests can be unit tests, as is best-practice.

However, you may find occassions where you'd like to write end-to-end tests to confirm your app is configured correctly and will work as expected from the console. 

These tools enable you to provide end-to-end testing as if running the app in a console.

!!! Note
    These test tools are used to test all of the CommandDotNet features.<br/>They are well suited to testing middleware and other extensibility components. 

## Testing the AppRunner

The tool provides two extension methods to execute an AppRunner in memory and collect the results.

=== "RunInMem"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            var result = new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .RunInMem("List aaa bbb", pipedInput: new[] { "ccc", "ddd" });

            result.ExitCode.Should().Be(0);
            result.Console.Out.Should().Be(@"aaa
    bbb
    ccc
    ddd
    ");
        }

        private class App
        {
            public void List(IConsole console, List<string> args) =>
                console.WriteLine(string.Join(Environment.NewLine, args));
        }
    }
    ```

=== "BDD Verify"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When = 
                    {
                        Args = "List aaa bbb",
                        PipedInput = new[] { "ccc", "ddd" } 
                    },
                    Then =
                    {
                        Output = @"aaa
    bbb
    ccc
    ddd
    "
                    }
                });
        }

        private class App
        {
            public void List(IConsole console, List<string> args) =>
                console.WriteLine(string.Join(Environment.NewLine, args));
        }
    }
    ```

[RunInMem](Harness/run-in-mem.md) will run the runner and collect results. Assertions will need to be executed after.

[Verify](Harness/bdd.md) wraps `RunInMem` with declarative BDD style setup and assertions.

!!! Tip
    If you're using the [.UseDefaultMiddleware()](../OtherFeatures/default-middleware.md) method, testing with either of these will help identify behavior changes on upgrade due to new opt-in features.

## Testing your application

### Using the same configuration

When testing an application, use the same method to generate and configure the AppRunner for the console and tests. In this example, the `Program.GetAppRunner()` method is introduced and made public 
so tests get an AppRunner configured exactly as it will be when run from the console. If you have additional configuration, such as an IoC container, be sure it's included in this method.

```c#
public class Program
{
        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);

            return GetAppRunner().Run(args);
        }

        public static AppRunner GetAppRunner()
        {    
            new AppRunner<Git>()
                .UseDefaultMiddleware()
                .UseNameCasing(Case.KebabCase)
                .UseFluentValidation();
        }
}
```

```c#
[TestFixture]
public class ProgramTests
{
    [Test]
    public void Checkout_NewBranch_BranchFlag_Succeeds()
    {
        Program.GetAppRunner()
            .Verify(new Scenario
            {
                When = { Args = "checkout -b lala" },
                Then = { 
                    Output = "Switched to a new branch 'lala'" 
                }
            });
    }
}
```

### AppInfo and AppName generation in tests

CommandDotNet uses `Assembly.GetEntryAssembly()` and `Process.GetCurrentProcess().MainModule` to generate the AppInfo and AppName used in the auto-generated help.

While ideal when executing the app, this is not deterministic when executing from a test runner.

For example, 

* when run using `dotnet test`, the AppName will be "dotnet testhost.dll"
* when run using the Resharper test runner, the AppName will be "ReSharperTestRunner64.exe"

See [Deterministic AppName for tests](Tools/deterministic-appinfo.md) for tips to work around this.

## Included test tools

The framework includes the following tools that can be used independently of the BDD Framework.

### [TestConsole](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestConsole.cs)

* covers all members of System.Console
* capture output for assertions
* provide piped input
* handle ReadLine and ReadKey

### [TempFiles](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TempFiles.cs)

* Creates temp files, give content and receive a file path.
* Removes created files on dispose
* Examples: [ResponseFileTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/ResponseFileTests.cs)

### [TestDependencyResolver](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/TestDependencyResolver.cs) 

```c#
new AppRunner<App>()
    .UseDependencyResolver(new TestDependencyResolver { dbSvc, httpSvc })
    .VerifyScenario(scenario);
```

### [CaptureState](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.TestTools/AppRunnerTestExtensions.cs#L20)

* `appRunner.CaptureState(...)` extension method that can be used to capture the point-in-time state of an object within the middleware pipeline.
* Examples: [DefaultArityTests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/DefaultArityTests.cs) and [Options_Name_Tests](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Tests/FeatureTests/Arguments/Options_Name_Tests.cs)

