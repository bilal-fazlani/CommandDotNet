using System;

namespace CommandDotNet.TestTools.Prompts
{
    public class UnexpectedPromptFailureException : Exception
    {
        public UnexpectedPromptFailureException(string promptText)
            : base($"unexpected prompt: {promptText}")
        {
        }
    }
}