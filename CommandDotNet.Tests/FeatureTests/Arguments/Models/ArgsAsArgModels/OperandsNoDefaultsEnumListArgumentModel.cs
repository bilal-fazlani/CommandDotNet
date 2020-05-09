using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsEnumListArgumentModel : IEnumListArgumentModel
    {
        [Operand]
        public List<DayOfWeek>? EnumListArg { get; set; }
    }
}