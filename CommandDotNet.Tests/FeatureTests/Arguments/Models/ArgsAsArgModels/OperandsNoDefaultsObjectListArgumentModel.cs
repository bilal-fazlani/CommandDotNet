using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsObjectListArgumentModel : IObjectListArgumentModel
    {
        [Operand]
        public List<Uri>? ObjectListArg { get; set; }
    }
}