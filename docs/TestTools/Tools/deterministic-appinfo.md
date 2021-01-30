# Deterministic AppName for tests

CommandDotNet uses `Assembly.GetEntryAssembly()` and `Process.GetCurrentProcess().MainModule` to generate the AppInfo and AppName used in the auto-generated help.

Unless specified in `AppSettings.Help.UsageAppName`, the AppName is derived from the either the entry assembly or the main module, depending on whether it's a self contained executable.

When running tests, the entry assembly may be null and the main module may be the test runner. 
When null, an exception will be thrown. 
When the test runner is used, the AppName is not deterministic for the tests. 
The AppName printed in the usage section of the help will changed depending on which runner is being used.

For example, 

* when run using `dotnet test`, the AppName will be "dotnet testhost.dll"
* when run using the Resharper test runner, the AppName will be "ReSharperTestRunner64.exe"

There are a few ways around this...

## AppSettings.Help.UsageAppName

The simplest approach is to override the UsageAppName in the AppSettings.

If your app is configuration the AppRunner and you want to ensure you're using the same configuration in tests,
consider using a static AppRunner factory to use in both your Program and tests. 

```c#
public class Program
{
    static int Main(string[] args)
    {
        Debugger.AttachIfDebugDirective(args);

        // set appName to null to let the framework generate the AppName from the file name
        return GetAppRunner(null).Run(args);
    }

    // set appName default to prevent the need to specify it in all tests
    public static AppRunner GetAppRunner(string? appName = "MyAppName")
    {
        return new AppRunner<MyCommands>(new AppSettings { Help { UsageAppName = appName } });
    }
}

public class Tests
{
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            var result = Program.GetAppRunner().RunInMem("-h");

            result.ExitCode.Should().Be(0);
            result.Console.Out.ShouldContain(@"Usage: MyAppName ");
        }
    }

    private class Task
    {
        public void New(string task)
        {
            console.Out.WriteLine($"new task: {task}");
        }
    }
}
```

## AppInfo.SetInstanceResolver

FOr test runners frameworks that can execute code before all tests (NUnit), use `AppInfo.SetResolver` set the AppInfo for all tests.

## TestConfig.AppInfoOverride

FOr test runners frameworks that can *not* execute code before all tests (XUnit), use `TestConfig.AppInfoOverride` set the AppInfo for all tests.

see [IDefaultTestConfig](test-config.md#idefaulttestconfig) and [tips for xunit](tips-test-runners.md) for more details.

