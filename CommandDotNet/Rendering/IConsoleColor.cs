using System;

namespace CommandDotNet.Rendering
{
    public interface IConsoleColor
    {
        ConsoleColor BackgroundColor { get; set; }
        ConsoleColor ForegroundColor { get; set; }
        void ResetColor();
    }
}