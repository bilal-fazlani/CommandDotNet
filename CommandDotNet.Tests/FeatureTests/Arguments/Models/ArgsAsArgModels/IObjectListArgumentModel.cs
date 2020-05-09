using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public interface IObjectListArgumentModel : IArgumentModel
    {
        List<Uri>? ObjectListArg { get; set; }
    }
}