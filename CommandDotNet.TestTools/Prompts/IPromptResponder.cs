using System;
using CommandDotNet.ConsoleOnly;

namespace CommandDotNet.TestTools.Prompts
{
    public interface IPromptResponder
    {
        ConsoleKeyInfo OnReadKey(TestConsole testConsole);
    }
}