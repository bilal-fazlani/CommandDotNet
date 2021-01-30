using CommandDotNet.TestTools;

namespace CommandDotNet.Tests
{
    public class CmdNetDefaultTestConfig : IDefaultTestConfig
    {
        public TestConfig Default => new TestConfig
        {
            OnError = {Print = {ConsoleOutput = true, CommandContext = true}}
        };
    }
}