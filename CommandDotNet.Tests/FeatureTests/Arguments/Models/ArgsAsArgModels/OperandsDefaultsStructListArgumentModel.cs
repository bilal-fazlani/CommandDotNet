using System.Collections.Generic;
using CommandDotNet.Attributes;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsDefaultsStructListArgumentModel : IStructListArgumentModel
    {
        [Operand]
        public List<int> StructListArg { get; set; } = new List<int> { 3, 4 };
    }
}