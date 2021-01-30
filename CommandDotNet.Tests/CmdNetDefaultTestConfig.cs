using CommandDotNet.Builders;
using CommandDotNet.TestTools;

namespace CommandDotNet.Tests
{
    public class CmdNetDefaultTestConfig : IDefaultTestConfig
    {
        public TestConfig Default => new TestConfig
        {
            OnError = {Print = {ConsoleOutput = true, CommandContext = true}},
            AppInfoOverride = new AppInfo(
                false, false, true, 
                typeof(CmdNetDefaultTestConfig).Assembly, 
                "testhost.dll", "testhost.dll", "1.1.1.1")
        };
    }
}