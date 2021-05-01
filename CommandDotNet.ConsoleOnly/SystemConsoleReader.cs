using System;

namespace CommandDotNet.ConsoleOnly
{
    public class SystemConsoleReader : IConsoleReader
    {
        public bool KeyAvailable => Console.KeyAvailable;

        public bool NumberLock => Console.NumberLock;

        public bool CapsLock => Console.CapsLock;

        public bool TreatControlCAsInput
        {
            get => Console.TreatControlCAsInput;
            set => Console.TreatControlCAsInput = value;
        }

        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            return Console.ReadKey(intercept);
        }

        public int Read()
        {
            return Console.Read();
        }

        public string? ReadLine()
        {
            return Console.ReadLine();
        }
    }
}