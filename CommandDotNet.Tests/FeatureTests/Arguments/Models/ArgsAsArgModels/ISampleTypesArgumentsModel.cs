using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    /// <summary>
    /// Implement this interface for a select
    /// sample of .Net Types to feel confident
    /// all types will be handled
    /// </summary>
    public interface ISampleTypesArgumentsModel: IArgumentModel
    {
        bool BoolArg { get; set; }
        string? StringArg { get; set; }
        int StructArg { get; set; }
        int? StructNArg { get; set; }
        DayOfWeek EnumArg { get; set; }
        Uri? ObjectArg { get; set; }
        List<string>? StringListArg { get; set; }
    }
}