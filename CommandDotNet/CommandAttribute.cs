using System;
using CommandDotNet.Parsing;

namespace CommandDotNet
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandAttribute : Attribute, INameAndDescription
    {
        public string? Name { get; set; }
        
        public string? Description { get; set; }

        public string? Usage { get; set; }

        public string? ExtendedHelpText { get; set; }

        /// <summary>
        /// Overrides <see cref="AppSettings.IgnoreUnexpectedOperands"/><br/>
        /// When false, unexpected operands will generate a parse failure.<br/>
        /// When true, unexpected arguments will be ignored and added to <see cref="ParseResult.RemainingOperands"/><br/>
        /// </summary>
        public bool IgnoreUnexpectedOperands
        {
            get => IgnoreUnexpectedOperandsAsNullable.GetValueOrDefault();
            set => IgnoreUnexpectedOperandsAsNullable = value;
        }

        /// <summary>Returns a nullable version of <see cref="IgnoreUnexpectedOperands"/> that will be null if a value was not assigned</summary>
        public bool? IgnoreUnexpectedOperandsAsNullable { get; private set; }

        /// <summary>
        /// The <see cref="ArgumentSeparatorStrategy"/> for the <see cref="Command"/>
        /// </summary>
        public ArgumentSeparatorStrategy ArgumentSeparatorStrategy
        {
            get => ArgumentSeparatorStrategyAsNullable.GetValueOrDefault();
            set => ArgumentSeparatorStrategyAsNullable = value;
        }

        /// <summary>Returns a nullable version of <see cref="ArgumentSeparatorStrategy"/> that will be null if a value was not assigned</summary>
        public ArgumentSeparatorStrategy? ArgumentSeparatorStrategyAsNullable { get; private set; }
    }
}