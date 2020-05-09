using System;
using CommandDotNet.Extensions;
using CommandDotNet.Prompts;
using CommandDotNet.Rendering;

namespace CommandDotNet.TestTools.Prompts
{
    /// <summary>
    /// Convenience methods for constructing prompt responses when
    /// <see cref="IPrompter"/> or <see cref="IArgumentPrompter"/> are used
    /// </summary>
    public static class Respond
    {
        /// <summary>Throws an exception if a prompt is requested using <see cref="IConsole.ReadKey"/></summary>
        public static IPromptResponder FailOnPrompt(Predicate<string>? promptFilter = null) => 
            new PromptResponder(new FailAnswer(promptFilter).ToEnumerable());

        /// <summary>Constructs a prompt response with a single answer</summary>
        /// <param name="answer">The response value. This is converted to an enumerable of <see cref="ConsoleKeyInfo"/>.</param>
        /// <param name="promptFilter">Applied to the prompt text. Use this to ensure the answer is for the correct prompt.</param>
        /// <param name="reuse">When false, this answer is discarded after use.</param>
        public static IPromptResponder WithText(string answer, Predicate<string>? promptFilter = null, bool reuse = false) =>
            new PromptResponder(new TextAnswer(answer, promptFilter, reuse).ToEnumerable());

        /// <summary>Constructs a prompt response with a single set of answers for a list value</summary>
        /// <param name="answers">The response values. This is converted to an enumerable of <see cref="ConsoleKeyInfo"/> delimited by Enter.</param>
        /// <param name="promptFilter">Applied to the prompt text. Use this to ensure the answer is for the correct prompt.</param>
        /// <param name="reuse">When false, this answer is discarded after use.</param>
        public static IPromptResponder WithList(string[] answers, Predicate<string>? promptFilter = null, bool reuse = false) =>
            new PromptResponder(new ListAnswer(answers, promptFilter, reuse).ToEnumerable());

        /// <summary>Constructs a prompt response for the given answers</summary>
        public static IPromptResponder With(params Answer[] answers) => new PromptResponder(answers);
    }
}