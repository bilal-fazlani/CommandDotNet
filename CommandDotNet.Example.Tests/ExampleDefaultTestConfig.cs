using CommandDotNet.Builders;
using CommandDotNet.TestTools;

namespace CommandDotNet.Example.Tests
{
    public class ExampleDefaultTestConfig : IDefaultTestConfig
    {
        public TestConfig Default => new TestConfig
        {
            //PrintCommandDotNetLogs = true,
            OnSuccess = {Print = {ConsoleOutput = true}},
            OnError = {Print = {ConsoleOutput = true, CommandContext = true}},
            AppInfoOverride = new AppInfo(
                false, false, false,
                typeof(Program).Assembly,
                "testhost.dll", "testhost.dll", "1.1.1.1")
        };
    }
}