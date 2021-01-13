using System;

namespace CommandDotNet.Prompting
{
    public static class NavigationKeyHandlers
    {
        public static ReadLineConfig UseDefaultNavigation(this ReadLineConfig config)
        {
            config.KeyHandlers.Map(ConsoleKey.LeftArrow, MoveCursorLeft);
            config.KeyHandlers.Map(ConsoleKey.RightArrow, MoveCursorRight);
            config.KeyHandlers.Map(ConsoleKey.Home, MoveCursorHome);
            config.KeyHandlers.Map(ConsoleKey.End, MoveCursorEnd);
            config.KeyHandlers.MapControl(ConsoleKey.LeftArrow, MoveToStartOfWord);
            config.KeyHandlers.MapControl(ConsoleKey.RightArrow, MoveToEndOfWord);
            return config;
        }

        public static ReadLineConfig UseReadLineNavigation(this ReadLineConfig config)
        {
            config.KeyHandlers.MapControl(ConsoleKey.B, MoveCursorLeft);
            config.KeyHandlers.MapControl(ConsoleKey.F, MoveCursorRight);
            config.KeyHandlers.MapControl(ConsoleKey.A, MoveCursorHome);
            config.KeyHandlers.MapControl(ConsoleKey.E, MoveCursorEnd);
            return config;
        }

        public static void Exit(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.ShouldExitPrompt = true;
        }

        public static void Cancel(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.ShouldExitPrompt = true;
            ctx.OnCtrlC?.Invoke();
        }

        public static void MoveToEndOfWord(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.MoveRight(ctx.Line.EndOfWordDistance());
        }

        public static void MoveToStartOfWord(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.MoveLeft(ctx.Line.StartOfWordDistance());
        }

        public static void MoveCursorEnd(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.MoveToEndOfLine();
        }

        public static void MoveCursorHome(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.MoveToStartOfLine();
        }

        public static void MoveCursorRight(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.MoveRight();
        }

        public static void MoveCursorLeft(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.MoveLeft();
        }
    }
}