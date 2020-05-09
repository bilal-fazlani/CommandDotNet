using System;

namespace CommandDotNet
{
    public class CommandNodeType
    {
        public static readonly CommandNodeType Command = new CommandNodeType(nameof(Command)) {IsCommand = true};
        public static readonly CommandNodeType Operand = new CommandNodeType(nameof(Operand)) {IsOperand = true};
        public static readonly CommandNodeType Option = new CommandNodeType(nameof(Option)) {IsOption = true};

        private readonly string _name;

        public bool IsCommand { get; private set; }
        public bool IsOperand { get; private set; }
        public bool IsOption { get; private set; }
        public bool IsArgument => IsOperand || IsOption;

        private CommandNodeType(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString()
        {
            return $"{nameof(CommandNodeType)}: {_name}";
        }
    }
}