using System;

namespace CommandDotNet.ConsoleOnly
{
    public static class ConsoleKeyInfoExtensions
    {
        public static bool IsCtrlC(this ConsoleKeyInfo key)
        {
            return key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control);
        }
    }
}