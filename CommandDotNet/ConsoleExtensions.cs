using CommandDotNet.Rendering;

namespace CommandDotNet
{
    public static class ConsoleExtensions
    {
        public static void Write(this IConsole console, object? value = null)
        {
            console.Out.Write(value);
        }

        public static void WriteLine(this IConsole console, object? value = null)
        {
            console.Out.WriteLine(value);
        }
    }
}