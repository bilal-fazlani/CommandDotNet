using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public interface IStructListArgumentModel : IArgumentModel
    {
        List<int>? StructListArg { get; set; }
    }
}