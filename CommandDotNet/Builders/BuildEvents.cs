using System;

namespace CommandDotNet.Builders
{
    /// <summary>
    /// Subscribe to <see cref="OnCommandCreated"/> to enhance <see cref="Command"/>s on creation.<br/>
    /// Set in <see cref="AppRunner.Configure"/>
    /// </summary>
    public class BuildEvents
    {
        /// <summary>
        /// Emitted for a <see cref="Command"/> after it has been created.
        /// Arguments and subcommands defined in the class/method will have been added.
        /// </summary>
        public event Action<CommandCreatedEventArgs>? OnCommandCreated;

        internal void CommandCreated(CommandContext commandContext, ICommandBuilder commandBuilder)
        {
            OnCommandCreated?.Invoke(new CommandCreatedEventArgs(commandContext, commandBuilder));
        }

        public class CommandCreatedEventArgs : EventArgs
        {
            public CommandContext CommandContext { get; }
            public ICommandBuilder CommandBuilder { get; }

            public CommandCreatedEventArgs(CommandContext commandContext, ICommandBuilder commandBuilder)
            {
                CommandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
                CommandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
            }
        }
    }
}