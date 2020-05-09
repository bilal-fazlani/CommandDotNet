using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public interface IEnumListArgumentModel : IArgumentModel
    {
        List<DayOfWeek>? EnumListArg { get; set; }
    }
}