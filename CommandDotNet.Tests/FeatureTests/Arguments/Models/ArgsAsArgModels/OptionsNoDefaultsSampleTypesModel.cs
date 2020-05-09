using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OptionsNoDefaultsSampleTypesModel :
        ISampleTypesArgumentsModel, IStructListArgumentModel, IEnumListArgumentModel, IObjectListArgumentModel
    {
        [Option]
        public bool BoolArg { get; set; }

        [Option]
        public string? StringArg { get; set; }

        [Option]
        public int StructArg { get; set; }

        [Option]
        public int? StructNArg { get; set; }

        [Option]
        public DayOfWeek EnumArg { get; set; }

        [Option]
        public Uri? ObjectArg { get; set; }

        [Option]
        public List<string>? StringListArg { get; set; }

        [Option]
        public List<int>? StructListArg { get; set; }

        [Option]
        public List<DayOfWeek>? EnumListArg { get; set; }

        [Option]
        public List<Uri>? ObjectListArg { get; set; }
    }
}