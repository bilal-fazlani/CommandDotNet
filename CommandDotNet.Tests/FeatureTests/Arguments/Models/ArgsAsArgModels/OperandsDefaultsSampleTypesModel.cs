using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OperandsDefaultsSampleTypesModel : ISampleTypesArgumentsModel
    {
        [Operand]
        public bool BoolArg { get; set; } = true;

        [Operand]
        public string? StringArg { get; set; } = "lala";

        [Operand]
        public int StructArg { get; set; } = 3;

        [Operand]
        public int? StructNArg { get; set; } = 4;

        [Operand]
        public DayOfWeek EnumArg { get; set; } = DayOfWeek.Tuesday;

        [Operand]
        public Uri? ObjectArg { get; set; } = new Uri("http://google.com");

        [Operand]
        public List<string>? StringListArg { get; set; } = new List<string> {"red", "blue"};
    }
}