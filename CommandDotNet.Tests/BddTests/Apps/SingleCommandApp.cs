using CommandDotNet.Attributes;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests.BddTests.Apps
{
    [ApplicationMetadata(Name = "Math")]
    public class SingleCommandApp
    {
        [InjectProperty]
        public TestOutputs TestOutputs { get; set; }

        public void Add(
            [Argument(Description = "the first operand")]
            int x,
            [Argument(Description = "the second operand")]
            int y,
            [Option(ShortName = "o", LongName = "operator", Description = "the operation to apply")]
            string operation = "+")
        {
            TestOutputs.Capture(new AddResults {X = x, Y = y, Op = operation});
        }

        public class AddResults
        {
            public int X { get; set; }
            public int Y { get; set; }
            public string Op { get; set; }
        }
    }
}