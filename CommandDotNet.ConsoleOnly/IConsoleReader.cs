using System;

namespace CommandDotNet.ConsoleOnly
{
    public interface IConsoleReader
    {
        bool KeyAvailable { get; }
        bool NumberLock { get; }
        bool CapsLock { get; }

        /// <summary>
        /// Gets or Sets value determine whether Ctrl+C is treated as ordinary input.<br/>
        /// When using System.Console, set this to true before <see cref="ReadKey"/> or
        /// Ctrl+C is not captured and does not interrupt.
        /// </summary>
        bool TreatControlCAsInput { get; set; }
        
        /// <summary>
        /// Read a key from the input
        /// </summary>
        /// <param name="intercept">When true, the key is not displayed in the output</param>
        /// <returns></returns>
        ConsoleKeyInfo ReadKey(bool intercept);

        string? ReadLine();
    }
}