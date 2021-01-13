using System;

namespace CommandDotNet.Prompting
{
    public static class EditKeyHandlers
    {
        public static ReadLineConfig UseDefaultEditing(this ReadLineConfig config)
        {
            config.KeyHandlers.Map(ConsoleKey.Backspace, Backspace);
            config.KeyHandlers.MapControl(ConsoleKey.Backspace, BackspaceToStartOfWord);
            config.KeyHandlers.MapControlShift(ConsoleKey.Backspace, BackspaceToStartOfLine);
            config.KeyHandlers.Map(ConsoleKey.Delete, Delete);
            config.KeyHandlers.MapControl(ConsoleKey.Delete, DeleteToEndOfWord);
            config.KeyHandlers.MapControlShift(ConsoleKey.Delete, DeleteToEndOfLine);
            config.KeyHandlers.Map(ConsoleKey.Escape, ClearLine);
            config.KeyHandlers.Map(ConsoleKey.Clear, ClearLine);
            config.KeyHandlers.Map(ConsoleKey.OemClear, ClearLine);
            return config;
        }

        public static ReadLineConfig UseReadLineEditing(this ReadLineConfig config)
        {
            config.KeyHandlers.MapControl(ConsoleKey.H, Backspace);
            config.KeyHandlers.MapControl(ConsoleKey.D, Delete);
            config.KeyHandlers.MapControl(ConsoleKey.L, ClearLine);
            config.KeyHandlers.MapControl(ConsoleKey.U, BackspaceToStartOfLine);
            config.KeyHandlers.MapControl(ConsoleKey.K, DeleteToEndOfLine);
            config.KeyHandlers.MapControl(ConsoleKey.W, BackspaceToStartOfWord);
            return config;
        }

        public static void Backspace(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.Backspace();
        }

        public static void Delete(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.Delete();
        }

        public static void ClearLine(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.ClearLine();
        }

        public static void BackspaceToStartOfLine(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.Backspace(ctx.Line.PositionInText);
        }

        public static void DeleteToEndOfLine(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.Delete(ctx.Line.Text.Length - ctx.Line.PositionInText);
        }

        public static void BackspaceToStartOfWord(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.Backspace(ctx.Line.StartOfWordDistance());
        }

        public static void DeleteToEndOfWord(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.Line.Delete(ctx.Line.EndOfWordDistance());
        }
    }
}