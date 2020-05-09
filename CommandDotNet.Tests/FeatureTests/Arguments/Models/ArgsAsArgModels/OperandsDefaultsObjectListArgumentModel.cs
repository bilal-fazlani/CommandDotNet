using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsDefaultsObjectListArgumentModel : IObjectListArgumentModel
    {
        [Operand]
        public List<Uri>? ObjectListArg { get; set; } = new List<Uri>
        {
            new Uri("http://google.com"),
            new Uri("http://github.com")
        };
    }
}