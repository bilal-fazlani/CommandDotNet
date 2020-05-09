using System.Collections.Generic;

namespace CommandDotNet.Builders
{
    /// <summary>
    /// Represents the nested nature of the arguments that can be expressed in the command line;
    /// <see cref="Command"/>, <see cref="Operand"/> and <see cref="Option"/>.<br/>
    /// <see cref="Command"/>s are included as they are arguments of other commands.<br/>
    /// </summary>
    public interface IArgumentNode : ICustomAttributesContainer, IServicesContainer
    {
        /// <summary>The name</summary>
        string Name { get; }

        /// <summary>The description</summary>
        string? Description { get; }

        /// <summary>
        /// The <see cref="Command"/> that hosts this <see cref="IArgumentNode"/>.
        /// Is null for the root command. Some parent commands are not executable
        /// but are defined only to group for other commands.
        /// </summary>
        Command? Parent { get; }

        /// <summary>The aliases defined for this argument</summary>
        IReadOnlyCollection<string> Aliases { get; }

        /// <summary>The source that defined this argument node</summary>
        string? DefinitionSource { get; }
    }
}