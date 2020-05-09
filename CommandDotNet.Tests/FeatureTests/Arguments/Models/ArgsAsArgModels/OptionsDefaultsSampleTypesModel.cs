using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels
{
    public class OptionsDefaultsSampleTypesModel : 
        ISampleTypesArgumentsModel, IStructListArgumentModel, IEnumListArgumentModel, IObjectListArgumentModel
    {
        [Option]
        public bool BoolArg { get; set; } = true;

        [Option]
        public string? StringArg { get; set; } = "lala";

        [Option]
        public int StructArg { get; set; } = 3;

        [Option]
        public int? StructNArg { get; set; } = 4;

        [Option]
        public DayOfWeek EnumArg { get; set; } = DayOfWeek.Tuesday;

        [Option]
        public Uri? ObjectArg { get; set; } = new Uri("http://google.com");

        [Option]
        public List<string>? StringListArg { get; set; } = new List<string> { "red", "blue" };

        [Option]
        public List<int>? StructListArg { get; set; } = new List<int> { 3, 4 };

        [Option]
        public List<DayOfWeek>? EnumListArg { get; set; } = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday };

        [Option]
        public List<Uri>? ObjectListArg { get; set; } = new List<Uri>
        {
            new Uri("http://google.com"),
            new Uri("http://github.com")
        };
    }
}