using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;
using CommandDotNet.Prompting;
using CommandDotNet.Rendering;

namespace CommandDotNet.Prompts
{
    public class Prompter : IPrompter
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly IConsole _console;

        public Prompter(IConsole console)
        {
            _console = console;
        }

        public virtual string PromptForValue(string promptText, out bool isCancellationRequested, bool isPassword = false)
        {
            return PromptForValueImpl(promptText, isPassword, false, out isCancellationRequested).FirstOrDefault();
        }

        public virtual IEnumerable<string> PromptForValues(string promptText, out bool isCancellationRequested, bool isPassword = false)
        {
            return PromptForValueImpl(promptText, isPassword, true, out isCancellationRequested);
        }

        public bool TryPromptForValue(string promptText, out string value, out bool isCancellationRequested,
            bool isPassword = false)
        {
            value = PromptForValue(promptText, out isCancellationRequested, isPassword);
            return !value.IsNullOrEmpty();
        }

        public bool TryPromptForValues(string promptText, out IEnumerable<string> values,
            out bool isCancellationRequested, bool isPassword = false)
        {
            values = PromptForValues(promptText, out isCancellationRequested, isPassword);
            return !values.IsNullOrEmpty();
        }

        private ICollection<string> PromptForValueImpl(
            string promptText, bool isPassword,
            bool isList, out bool isCancellationRequested)
        {
            // Ctrl+C does not raise Console.CancelKeyPress while Console.ReadKey is waiting for input.
            // This prevents users from cancelling out of the app while being prompted for input.
            // Especially annoying when there are several inputs requested.
            //
            // Using Console.TreatControlCAsInput, the request can be captured and returned
            // via the isCancellationRequested parameter.

            if (_console.In.TreatControlCAsInput)
            {
                return PromptForValueRobustImpl(promptText, isPassword, isList, out isCancellationRequested);
            }

            Log.Debug("setting console.TreatControlCAsInput = true");
            _console.In.TreatControlCAsInput = true;
            using (new DisposableAction(() =>
            {
                Log.Debug("setting console.TreatControlCAsInput = false");
                _console.In.TreatControlCAsInput = false;
            }))
            {
                return PromptForValueRobustImpl(promptText, isPassword, isList, out isCancellationRequested);
            }
        }

        private ICollection<string> PromptForValueRobustImpl(
            string promptText, bool isPassword,
            bool isList, out bool isCancellationRequested)
        {
            // cannot: navigate with arrow keys, insert characters

            bool ctrlC = false;
            var readLineConfig = ReadLineConfig.Factory().With(c => c.OnCtrlC = () => ctrlC = true);

            var values = isList
                ? _console.ReadLines(
                        promptText: promptText,
                        isSecret: isPassword,
                        readLineConfig: readLineConfig)
                : _console.ReadLine(
                        promptText: promptText,
                        isSecret: isPassword,
                        readLineConfig: readLineConfig)
                    .ToEnumerable();

            isCancellationRequested = ctrlC;
            return values.ToCollection();
        }

        private static void ClearCurrent(StringBuilder sb, IConsoleWriter consoleOut)
        {
            if (sb.Length > 0)
            {
                consoleOut.Write(Enumerable.Repeat("\b \b", sb.Length).ToCsv(""));
                sb.Length = 0;
            }
        }
    }
}