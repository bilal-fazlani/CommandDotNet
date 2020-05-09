using System;

namespace CommandDotNet.TestTools.Prompts
{
    public class TextAnswer : Answer
    {
        /// <summary>Constructs a response for prompt of a single value</summary>
        /// <param name="value">The response value. This is converted to an enumerable of <see cref="ConsoleKeyInfo"/>.</param>
        /// <param name="promptFilter">Applied to the prompt text. Use this to ensure the answer is for the correct prompt.</param>
        /// <param name="reuse">When false, this answer is discarded after use.</param>
        public TextAnswer(string value, Predicate<string>? promptFilter = null, bool reuse = false)
            : base(value.ToConsoleKeyInfos(), promptFilter, reuse)
        {
        }
    }
}