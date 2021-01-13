using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;

namespace CommandDotNet.Prompting
{
    public class ReadLineConfig
    {
        private static Func<ReadLineConfig> factory = () => new ReadLineConfig()
            .UseAllDefaultKeyHandlers()
            .With(c =>
            {
                c.History = DefaultHistory;
                c.OnCtrlC = DefaultOnCtrlC;
            });

        public static Func<ReadLineConfig> Factory
        {
            get => factory;
            set => factory = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static IDictionary<string, ICollection<string>>? DefaultHistory { get; set; }
        public static Action? DefaultOnCtrlC { get; set; }

        public ReadLineConfig()
        {
            KeyHandlers.Map(ConsoleKey.Enter, NavigationKeyHandlers.Exit);
            KeyHandlers.MapControl(ConsoleKey.C, NavigationKeyHandlers.Cancel);
            KeyHandlers.MapShiftAlt('?', ListKeyHandlers);
        }

        private void ListKeyHandlers(ConsoleKeyInfo info, ReadLineContext ctx)
        {
            int maxKeyLength = 0;
            int maxHandlerLength = 0;

            var sb = new StringBuilder();

            var values = KeyHandlers.AllHandlers
                .Select(h =>
                {
                    var handler = h.handler.Method.Name;
                    return (h.info, handler);
                })
                .GroupBy(h => h.handler)
                .Select(group => (
                    handler:@group.Key,
                    keys:@group
                        .Select(h => (h.info.Modifiers,name:h.info.ToFriendlyName()))
                        .OrderBy(h => h.Modifiers)
                        .ThenBy(h => h.name)
                        .Select(h => h.name)
                        .ToCsv()))
                .OrderBy(h => h.handler)
                .Select(h =>
                {
                    maxKeyLength = Math.Max(maxKeyLength, h.keys.Length);
                    maxHandlerLength = Math.Max(maxHandlerLength, h.handler.Length);
                    return h;
                })
                .ToList();
            values.ForEach(h =>
            {
                sb.AppendLine($"{h.keys.PadRight(maxKeyLength)} > {h.handler.PadRight(maxHandlerLength)}");
            });

            ctx.Line.Notify(sb.ToString(), preserveText: true);
        }

        public IDictionary<string, ICollection<string>>? History { get; set; }

        public Action? OnCtrlC { get; set; }

        public KeyHandlerMap KeyHandlers { get; } = new KeyHandlerMap();
    }
}