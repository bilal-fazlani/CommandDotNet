using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsParams
{
    public interface IArgsDefaultsSampleTypesMethod
    {
        void ArgsDefaults(
            bool boolArg = true,
            string stringArg = "lala",
            int structArg = 3,
            int? structNArg = 4,
            DayOfWeek enumArg = DayOfWeek.Wednesday,
            Uri? objectArg = null,
            List<string>? stringListArg = null);

        void StructListDefaults(List<int>? structListArg = null);
        void EnumListDefaults(List<DayOfWeek>? enumListArg = null);
        void ObjectListDefaults(List<Uri>? objectListArg = null);
    }
}