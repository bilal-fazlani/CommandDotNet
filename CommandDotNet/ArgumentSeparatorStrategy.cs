using CommandDotNet.Parsing;

namespace CommandDotNet
{
    /// <summary>
    /// Determines whether arguments after the argument selector '--' are
    /// parsed and collected or just collected.
    /// </summary>
    public enum ArgumentSeparatorStrategy
    {
        /// <summary>
        /// Arguments after the argument separator '--' are parsed as operands
        /// and also added to <see cref="ParseResult.SeparatedArguments"/>
        /// </summary>
        EndOfOptions,
        /// <summary>
        /// Arguments after the argument separator '--' are added to <see cref="ParseResult.SeparatedArguments"/>
        /// </summary>
        PassThru
    }
}