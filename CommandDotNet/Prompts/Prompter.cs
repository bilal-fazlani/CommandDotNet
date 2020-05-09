using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

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

            if (_console.TreatControlCAsInput)
            {
                return PromptForValueRobustImpl(promptText, isPassword, isList, out isCancellationRequested);
            }

            Log.Debug("setting console.TreatControlCAsInput = true");
            _console.TreatControlCAsInput = true;
            using (new DisposableAction(() =>
            {
                Log.Debug("setting console.TreatControlCAsInput = false");
                _console.TreatControlCAsInput = false;
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
            
            isCancellationRequested = false;
            var consoleOut = _console.Out;
            consoleOut.Write(promptText);
            if (isList)
            {
                consoleOut.Write(" [<enter> once to begin new value. <enter> twice to finish]");
            }
            consoleOut.Write(": ");
            if (isList)
            {
                consoleOut.WriteLine();
            }
            
            var values = new List<string>();
            var sb = new StringBuilder();

            do
            {
                var key = _console.ReadKey(true);

                if (key.IsCtrlC())
                {
                    sb.Length = 0;
                    isCancellationRequested = true;
                    break;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    if(!isList || key.Modifiers.HasFlag(ConsoleModifiers.Control))
                        //if (key.Modifiers == 0 || !isList)
                    {
                        break;
                    }

                    // isList == true 

                    if (sb.Length == 0)
                    {
                        // if user is confused by ctrl+enter, they may hit enter multiple times.
                        // enter for an empty entry can also exit.
                        break;
                    }

                    values.Add(sb.ToString());
                    sb.Length = 0;
                    consoleOut.WriteLine();
                }
                else if (ConsoleKeys.PassThroughKeys.Contains(key.Key))
                {
                    sb.Append(key.KeyChar);
                    consoleOut.Write(isPassword ? "" : key.KeyChar.ToString());
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        // if isPassword, there is nothing to backspace because passwords
                        // characters are immediately removed from the console
                        if (!isPassword)
                        {
                            consoleOut.Write("\b \b");
                        }
                        sb.Length = sb.Length - 1;
                    }
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    // if no prompt has been entered, the user wants to exit the prompt
                    if (sb.Length == 0)
                    {
                        break;
                    }

                    // otherwise the user wants to clear the current entry
                    ClearCurrent(sb, consoleOut);
                }
                else if (key.Key == ConsoleKey.Clear || key.Key == ConsoleKey.OemClear)
                {
                    ClearCurrent(sb, consoleOut);
                }

            } while (true);

            if (sb.Length > 0)
            {
                values.Add(sb.ToString());
            }
            consoleOut.WriteLine();
            return values;
        }

        private ICollection<string>? PromptForValueSimpleImpl(
            string promptText, bool isPassword,
            bool isList, out bool isCancellationRequested)
        {
            // cannot:
            // - skip prompts w/ escape
            // - skip out of prompting and exit app with ctrl+c
            // - support multiple entry w/ enter+modifier

            if (isPassword)
            {
                return PromptForValueRobustImpl(promptText, isPassword, isList, out isCancellationRequested);
            }

            isCancellationRequested = false;
            var consoleOut = _console.Out;
            consoleOut.Write(promptText);
            if (isList)
            {
                consoleOut.Write(" [separate values by space]");
            }
            consoleOut.Write(": ");

            var value = _console.In.ReadLine();
            consoleOut.WriteLine();
            if (value is null)
            {
                return null;
            }
            return isList
                ? CommandLineStringSplitter.Instance.Split(value).ToCollection()
                : new[] { value };
        }

        private static void ClearCurrent(StringBuilder sb, IStandardStreamWriter consoleOut)
        {
            if (sb.Length > 0)
            {
                consoleOut.Write(Enumerable.Repeat("\b \b", sb.Length).ToCsv(""));
                sb.Length = 0;
            }
        }
    }
}