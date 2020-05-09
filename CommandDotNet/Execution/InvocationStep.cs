using System;
using CommandDotNet.Extensions;
using static System.Environment;

namespace CommandDotNet.Execution
{
    public class InvocationStep : IIndentableToString
    {
        /// <summary>
        /// The instance of the class containing the invocation method.<br/>
        /// Populated during <see cref="MiddlewareStages.BindValues"/>
        /// </summary>
        public object? Instance { get; set; }

        /// <summary>
        /// The command the invocation will fulfill.<br/>
        /// Populated during <see cref="MiddlewareStages.ParseInput"/>
        /// </summary>
        public Command Command { get; }

        private IInvocation _invocation;

        /// <summary>
        /// The <see cref="IInvocation"/> to be invoked for the command<br/>
        /// Populated during <see cref="MiddlewareStages.ParseInput"/>
        /// </summary>
        public IInvocation Invocation
        {
            get => _invocation;
            set => _invocation = value ?? throw new ArgumentNullException(nameof(value));
        }

        public InvocationStep(Command command, IInvocation invocation)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            _invocation = invocation ?? throw new ArgumentNullException(nameof(invocation));
        }

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            return $"{nameof(InvocationStep)}:{NewLine}" +
                   $"{indent}{nameof(Command)}={Command?.Name}{NewLine}" +
                   $"{indent}{nameof(Invocation)}={Invocation?.MethodInfo?.FullName(true)}{NewLine}" +
                   $"{indent}{nameof(Instance)}={Instance}";
        }
    }
}