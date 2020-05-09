using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;
using static System.Environment;

namespace CommandDotNet.Execution
{
    public class InvocationPipeline : IIndentableToString
    {
        private ICollection<InvocationStep> _ancestorInterceptors = new List<InvocationStep>();

        /// <summary>
        /// The invocations for the interceptor methods of the ancestor commands of the <see cref="TargetCommand"/>.
        /// Order is top-most parent first.
        /// </summary>
        public ICollection<InvocationStep> AncestorInterceptors
        {
            get => _ancestorInterceptors;
            set => _ancestorInterceptors = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>The invocation for the <see cref="ParseResult.TargetCommand"/></summary>
        public InvocationStep? TargetCommand { get; set; }

        /// <summary>
        /// Joins <see cref="AncestorInterceptors"/> and <see cref="TargetCommand"/> with top-most parent
        /// first and <see cref="TargetCommand"/> last.
        /// </summary>
        public IEnumerable<InvocationStep> All => TargetCommand == null
            ? AncestorInterceptors
            : AncestorInterceptors.Concat(TargetCommand.ToEnumerable());

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            var indent2 = indent.Increment();
            var root = $"{nameof(InvocationPipeline)}:{NewLine}" +
                       $"{indent}{nameof(TargetCommand)}:{TargetCommand?.ToString(indent2)}";

            var hasInterceptors = AncestorInterceptors.Any();
            if (!hasInterceptors)
            {
                return root;
            }

            root += NewLine;

            if (AncestorInterceptors.Count == 1)
            {
                var step = AncestorInterceptors.First();
                return root + $"{indent}Interceptors:{step.ToString(indent2)}";
            }

            var sb = new StringBuilder(root);
            foreach (var step in AncestorInterceptors)
            {
                sb.AppendLine($"{indent}{step.ToString(indent2)}{NewLine}");
            }
            //remove last NewLine
            sb.Remove(sb.Length - NewLine.Length, NewLine.Length);

            return sb.ToString();
        }
    }
}