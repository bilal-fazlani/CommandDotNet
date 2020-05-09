using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Parsing;
using CommandDotNet.Tokens;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    public class AppSettings : IIndentableToString
    {
        private BooleanMode _booleanMode = BooleanMode.Implicit;

        /// <summary>
        /// When Explicit, boolean options require a 'true' or 'false' value be specified.<br/>
        /// When Implicit, boolean options are treated as Flags, considered false unless it's specified
        /// and the next argument will be considered a new argument.
        /// </summary>
        public BooleanMode BooleanMode
        {
            get => _booleanMode;
            set
            {
                if(value == BooleanMode.Unknown)
                    throw new AppRunnerException("BooleanMode can not be set to BooleanMode.Unknown explicitly");
                _booleanMode = value;
            }
        }

        /// <summary>
        /// When false, unexpected operands will generate a parse failure.<br/>
        /// When true, unexpected arguments will be ignored and added to <see cref="ParseResult.RemainingOperands"/><br/>
        /// </summary>
        public bool IgnoreUnexpectedOperands { get; set; }

        /// <summary>
        /// The default <see cref="ArgumentSeparatorStrategy"/>.
        /// This can be overridden for a <see cref="Command"/> using the <see cref="CommandAttribute"/>
        /// </summary>
        public ArgumentSeparatorStrategy DefaultArgumentSeparatorStrategy { get; set; } = ArgumentSeparatorStrategy.EndOfOptions;

        /// <summary>
        /// When arguments are not decorated with [Operand] or [Option]
        /// DefaultArgumentMode is used to determine which mode to use.
        /// Operand is the default.
        /// </summary>
        public ArgumentMode DefaultArgumentMode { get; set; } = ArgumentMode.Operand;

        /// <summary>
        /// Set to true to prevent tokenizing arguments as <see cref="TokenType.Directive"/>,
        /// captured in <see cref="CommandContext.Tokens"/>.
        /// Arguments with the [directive syntax] will be tokenized
        /// as <see cref="TokenType.Value"/>.
        /// </summary>
        public bool DisableDirectives { get; set; }

        /// <summary>Settings specific to built-in help providers</summary>
        public AppHelpSettings Help { get; set; } = new AppHelpSettings();

        /// <summary>
        /// The collection of <see cref="IArgumentTypeDescriptor"/>'s use to convert arguments
        /// from the commandline to the parameter & property types for the command methods.
        /// </summary>
        public ArgumentTypeDescriptors ArgumentTypeDescriptors { get; internal set; } = new ArgumentTypeDescriptors();

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            return this.ToStringFromPublicProperties(indent);
        }
    }
}