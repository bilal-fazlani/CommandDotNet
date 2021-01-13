using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;
using CommandDotNet.Rendering;

namespace CommandDotNet.Prompting
{
    public static class ConsoleReadLineExtensions
    {
        public static IEnumerable<string> ReadLines(this IConsole console, bool isSecret = false, string? promptText = null,
            string? historyKey = null, ICollection<string>? history = null, ReadLineConfig? readLineConfig = null)
        {
            if (promptText is { })
            {
                console.WriteLine(promptText);
            }

            // use a single config for each line
            readLineConfig ??= ReadLineConfig.Factory();

            while (true)
            {
                var result = console.ReadLine(
                    isSecret: isSecret,
                    historyKey: historyKey,
                    history: history,
                    readLineConfig: readLineConfig);

                if (result.IsNullOrEmpty())
                {
                    yield break;
                }

                yield return result;
            }
        }

        // TODO: consider adding a Prompt extension method in the CommandDotNet space w/o ReadLineConfig for simpler use
        public static string ReadLine(this IConsole console, bool isSecret = false, string? promptText = null,
            string? historyKey = null, ICollection<string>? history = null, ReadLineConfig? readLineConfig = null)
        {
            if (promptText is { })
            {
                console.Write(promptText);
            }

            var context = new ReadLineContext(new Line(console, isSecret), readLineConfig);
            
            if (history is { })
            {
                if (historyKey is { })
                {
                    throw new ArgumentException($"only one can be provided: {nameof(historyKey)} OR {nameof(history)}.");
                }

                context.History = history;
            }
            else if (historyKey is { })
            {
                context.History = context.Config.History?.GetOrAdd(historyKey, key => new List<string>());
            }

            return console.ReadLine(context);
        }

        private static string ReadLine(this IConsole console, ReadLineContext ctx)
        {
            var original = console.In.TreatControlCAsInput;
            using var restoreOriginal = new DisposableAction(() => console.In.TreatControlCAsInput = original);
            console.In.TreatControlCAsInput = true;

            foreach (var info in console.ReadKeys())
            {
                
                if (ctx.Config.KeyHandlers.TryGet(info, out KeyHandlerDelegate? action))
                {
                    action!.Invoke(info, ctx);
                }
                else
                {
                    ctx.Line.Write(info);
                }

                if (ctx.ShouldExitPrompt)
                {
                    break;
                }
            }
            console.WriteLine();

            var result = ctx.Line.Text;
            ctx.History?.Add(result);

            return result;
        }

        private static IEnumerable<ConsoleKeyInfo> ReadKeys(this IConsole console)
        {
            while(true)
            {
                yield return console.In.ReadKey(true);
            }
        }
    }
}
    