using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools.Prompts
{
    public class FailAnswer : IAnswer
    {
        public ICollection<ConsoleKeyInfo> ConsoleKeys { get; } = EmptyCollection<ConsoleKeyInfo>.Instance;
        public bool Reuse => false;
        public Predicate<string>? PromptFilter { get; }
        public bool ShouldFail => true;

        /// <summary>Constructs a response for prompt of a single value</summary>
        /// <param name="promptFilter">Applied to the prompt text. Use this to ensure the answer is for the correct prompt.</param>
        public FailAnswer(Predicate<string>? promptFilter = null)
        {
            PromptFilter = promptFilter;
        }
    }
}