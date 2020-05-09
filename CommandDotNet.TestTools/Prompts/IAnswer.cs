using System;
using System.Collections.Generic;
using CommandDotNet.Prompts;

namespace CommandDotNet.TestTools.Prompts
{
    /// <summary>
    /// Responses for a prompt when 
    /// <see cref="IPrompter"/> or <see cref="IArgumentPrompter"/> are used
    /// </summary>
    public interface IAnswer
    {
        /// <summary>
        /// The <see cref="ConsoleKeyInfo"/>s to write to the console
        /// </summary>
        ICollection<ConsoleKeyInfo> ConsoleKeys { get; }

        /// <summary>
        /// When false, the answer is discarded after use, otherwise it will be reused.
        /// </summary>
        bool Reuse { get; }

        /// <summary>
        /// Applied to the prompt text. Use this to ensure the answer is for the correct prompt.
        /// </summary>
        Predicate<string>? PromptFilter { get; }
        
        /// <summary>
        /// If true, an UnexpectedPrompt
        /// </summary>
        bool ShouldFail { get; }
    }
}