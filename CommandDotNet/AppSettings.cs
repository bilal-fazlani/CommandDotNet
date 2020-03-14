﻿using System;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Logging;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    public class AppSettings : ILogToString
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
        /// When false, unexpected arguments will result in a parse failure with help message.<br/>
        /// When true, unexpected arguments will be ignored
        /// </summary>
        public bool IgnoreUnexpectedOperands { get; set; }

        /// <summary>
        /// When arguments are not decorated with [Operand] or [Option]
        /// DefaultArgumentMode is used to determine which mode to use.
        /// Operand is the default.
        /// </summary>
        public ArgumentMode DefaultArgumentMode { get; set; } = ArgumentMode.Operand;

        /// <summary>
        /// Set to true to prevent tokenizing arguments as directives,
        /// captured in <see cref="CommandContext.Tokens"/>.
        /// Arguments with the [directive syntax] will be tokenized
        /// as values instead.
        /// </summary>
        public bool DisableDirectives { get; set; }

        /// <summary>Settings specific to built-in help providers</summary>
        public AppHelpSettings Help { get; set; } = new AppHelpSettings();

        /// <summary>
        /// The collection of <see cref="IArgumentTypeDescriptor"/>'s use to convert arguments
        /// from the commandline to the parameter & property types for the command methods.
        /// </summary>
        public ArgumentTypeDescriptors ArgumentTypeDescriptors { get; internal set; } = new ArgumentTypeDescriptors();

        #region Obsolete Members

        [Obsolete("Use DefaultArgumentMode instead")]
        public ArgumentMode MethodArgumentMode
        {
            get => DefaultArgumentMode;
            set => DefaultArgumentMode = value;
        }

        [Obsolete("Use IgnoreUnexpectedArguments instead")]
        public bool ThrowOnUnexpectedArgument
        {
            get => !IgnoreUnexpectedOperands;
            set => IgnoreUnexpectedOperands = !value;
        }

        [Obsolete("this is only used to display the arg separator in help. it does not make the separated arguments available for use.")]
        public bool AllowArgumentSeparator { get; set; }

        [Obsolete("Use Help.TextStyle")]
        public HelpTextStyle HelpTextStyle
        {
            get => Help.TextStyle;
            set => Help.TextStyle = value;
        }

        #endregion

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(string indent)
        {
            var appSettingsProps = this.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(p => p.Name);

            var prefix = $"{indent}  ";

            var props = appSettingsProps.Select(p =>
            {
                var value = p.GetValue(this);
                return $"{prefix}{p.Name}: {(value is ILogToString logToString ? logToString.ToString(prefix) : value)}";
            });
            return $"{indent}{nameof(AppSettings)}:{Environment.NewLine}{props.ToCsv(Environment.NewLine)}";
        }
    }
}