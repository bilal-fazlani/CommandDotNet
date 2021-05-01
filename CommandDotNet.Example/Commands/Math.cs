using System.Collections.Generic;
using System.Linq;
using CommandDotNet.ConsoleOnly;

namespace CommandDotNet.Example.Commands
{
    public class Math
    {
        public void Add(IConsole console, int x, int y) => console.WriteLine(x + y);
        public void AddRange(IConsole console, IEnumerable<int> values) => console.WriteLine(values.Sum());
        public void Subtract(IConsole console, int x, int y) => console.WriteLine(x - y);
    }
}