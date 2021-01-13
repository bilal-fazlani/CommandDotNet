using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.Prompting
{
    public static class HistoryKeyHandlers
    {
        public static ReadLineConfig UseDefaultHistory(this ReadLineConfig config)
        {
            config.KeyHandlers.Map(ConsoleKey.UpArrow, PrevHistory);
            config.KeyHandlers.Map(ConsoleKey.DownArrow, NextHistory);
            config.KeyHandlers.Map(ConsoleKey.PageUp, FirstHistory);
            config.KeyHandlers.Map(ConsoleKey.PageDown, LastHistory);
            config.KeyHandlers.MapControlShift(ConsoleKey.H, DisplayHistory);
            return config;
        }

        public static ReadLineConfig UseReadLineHistory(this ReadLineConfig config)
        {
            config.KeyHandlers.MapControl(ConsoleKey.P, PrevHistory);
            config.KeyHandlers.MapControl(ConsoleKey.N, NextHistory);
            return config;
        }

        private class HistoryContext
        {
            public ICollection<string> History;
            public int Index;

            public HistoryContext(ICollection<string> history)
            {
                History = history;

                // start at last item in the history
                Index = History.Count - 1;
            }
        }

        public static void PrevHistory(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            var historyContext = ctx.GetHistoryContext();
            if (historyContext.History.Any())
            {
                ctx.Line.ClearLineAndWrite(historyContext.History.ElementAt(historyContext.Index));
                if (historyContext.Index > 0)
                {
                    historyContext.Index--;
                }
            }
        }

        public static void NextHistory(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            var historyContext = ctx.GetHistoryContext();
            if (historyContext.History.Any())
            {
                if (historyContext.Index < historyContext.History.Count-1)
                {
                    historyContext.Index++;
                    if (historyContext.Index == historyContext.History.Count)
                    {
                        ctx.Line.ClearLine();
                    }
                    else
                    {
                        ctx.Line.ClearLineAndWrite(historyContext.History.ElementAt(historyContext.Index));
                    }
                }
            }
        }

        private static void FirstHistory(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.GetHistoryContext().Index = 0;
            PrevHistory(info, ctx);
        }

        private static void LastHistory(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            ctx.GetHistoryContext().Index = 0;
            NextHistory(info, ctx);
        }

        private static void DisplayHistory(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            var history = ctx.GetHistoryContext().History.ToCsv(Environment.NewLine);
            ctx.Line.Notify(history, true);
        }

        private static HistoryContext GetHistoryContext(this ReadLineContext ctx)
        {
            return (HistoryContext) ctx.State.GetOrAdd(typeof(HistoryContext),
                type => new HistoryContext(ctx.History ?? new List<string>()))!;
        }
    }
}