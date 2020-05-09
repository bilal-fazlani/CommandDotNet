using System.Collections.Generic;

namespace CommandDotNet.Parsing
{
    internal interface IParser
    {
        object? Parse(IArgument argument, IEnumerable<string> values);
    }
}