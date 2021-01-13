using CommandDotNet.Rendering;

namespace CommandDotNet.Prompting
{
    internal class CursorPosition
    {
        public int Left { get; }
        public int Top { get; }

        private CursorPosition(IConsole console)
        {
            Left = console.CursorLeft;
            Top = console.CursorTop;
        }

        public static CursorPosition Snapshot(IConsole console) => new CursorPosition(console);

        public void Restore(IConsole console) => console.SetCursorPosition(Left, Top);
    }
}