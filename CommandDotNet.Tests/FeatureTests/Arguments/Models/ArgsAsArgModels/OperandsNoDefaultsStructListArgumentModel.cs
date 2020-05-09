using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsStructListArgumentModel : IStructListArgumentModel
    {
        [Operand]
        public List<int>? StructListArg { get; set; }
    }
}