using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsNoDefaultsSampleTypesModel : ISampleTypesArgumentsModel
    {
        [Operand]
        public bool BoolArg { get; set; }

        [Operand]
        public string? StringArg { get; set; }

        [Operand]
        public int StructArg { get; set; }

        [Operand]
        public int? StructNArg { get; set; }

        [Operand]
        public DayOfWeek EnumArg { get; set; }

        [Operand]
        public Uri? ObjectArg { get; set; }

        [Operand]
        public List<string>? StringListArg { get; set; }
    }
}