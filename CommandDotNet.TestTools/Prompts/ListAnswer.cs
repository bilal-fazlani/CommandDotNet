using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.TestTools.Prompts
{
    public class ListAnswer : Answer
    {
        /// <summary>Constructs a response for prompt of a list of values</summary>
        /// <param name="valueList">The response values. This is converted to an enumerable of <see cref="ConsoleKeyInfo"/> delimited by Enter.</param>
        /// <param name="promptFilter">Applied to the prompt text. Use this to ensure the answer is for the correct prompt.</param>
        /// <param name="reuse">When false, this answer is discarded after use.</param>
        public ListAnswer(IEnumerable<string> valueList, Predicate<string>? promptFilter = null, bool reuse = false)
            : base(valueList.SelectMany(v => v.ToConsoleKeyInfos()?.AppendEnterKey()), promptFilter, reuse)
        {
        }
    }
}