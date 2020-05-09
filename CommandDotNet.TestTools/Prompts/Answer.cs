using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;
using CommandDotNet.Prompts;

namespace CommandDotNet.TestTools.Prompts
{
    /// <summary>
    /// Responses for a prompt when 
    /// <see cref="IPrompter"/> or <see cref="IArgumentPrompter"/> are used
    /// </summary>
    public class Answer : IAnswer
    {
        public ICollection<ConsoleKeyInfo> ConsoleKeys { get; }
        public bool Reuse { get; }
        public Predicate<string>? PromptFilter { get; }
        public bool ShouldFail => false;

        /// <summary>Constructs a response for prompt of a list of values</summary>
        /// <param name="consoleKeys">The <see cref="ConsoleKeyInfo"/> that represent the console input.</param>
        /// <param name="promptFilter">Applied to the prompt text. Use this to ensure the answer is for the correct prompt.</param>
        /// <param name="reuse">When false, this answer is discarded after use.</param>
        public Answer(
            IEnumerable<ConsoleKeyInfo>? consoleKeys, 
            Predicate<string>? promptFilter = null,
            bool reuse = false)
        {
            ConsoleKeys = consoleKeys?.ToCollection() ?? EmptyCollection<ConsoleKeyInfo>.Instance;
            Reuse = reuse;
            PromptFilter = promptFilter;
        }
    }
}