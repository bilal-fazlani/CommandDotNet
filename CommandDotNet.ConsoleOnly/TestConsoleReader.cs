using System;

namespace CommandDotNet.ConsoleOnly
{
    internal class TestConsoleReader : IConsoleReader
    {
        private readonly IConsoleWriter _standardOut;
        private readonly Func<string?>? _onReadLine;
        private readonly Func<ConsoleKeyInfo>? _onReadKey;

        public TestConsoleReader(IConsoleWriter standardOut, Func<string?>? onReadLine, Func<ConsoleKeyInfo>? onReadKey)
        {
            _standardOut = standardOut ?? throw new ArgumentNullException(nameof(standardOut));
            _onReadLine = onReadLine;
            _onReadKey = onReadKey;
        }

        public bool KeyAvailable { get; }
        public bool NumberLock { get; }
        public bool CapsLock { get; }
        public bool TreatControlCAsInput { get; set; }

        /// <summary>
        /// Read a key from the input
        /// </summary>
        /// <param name="intercept">When true, the key is not displayed in the output</param>
        /// <returns></returns>
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            ConsoleKeyInfo consoleKeyInfo;

            do
            {
                consoleKeyInfo = _onReadKey?.Invoke()
                                 ?? new ConsoleKeyInfo('\u0000', ConsoleKey.Enter, false, false, false);

                // mimic System.Console which does not interrupt during ReadKey
                // and does not return Ctrl+C unless TreatControlCAsInput == true.
            } while (!TreatControlCAsInput && consoleKeyInfo.IsCtrlC());

            if (!intercept)
            {
                if (consoleKeyInfo.Key == ConsoleKey.Enter)
                {
                    _standardOut.WriteLine("");
                }
                else
                {
                    _standardOut.Write(consoleKeyInfo.KeyChar.ToString());
                }
            }
            return consoleKeyInfo;
        }

        public string? ReadLine()
        {
            return _onReadLine?.Invoke();
        }
    }
}