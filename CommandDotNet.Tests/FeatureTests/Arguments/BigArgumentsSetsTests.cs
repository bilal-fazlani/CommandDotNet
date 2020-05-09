using System;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class BigArgumentsSetsTests
    {
        private readonly ITestOutputHelper _output;

        public BigArgumentsSetsTests(ITestOutputHelper output)
        {
            Ambient.Output = _output = output;
        }

        private class Names
        {
            public int Index;
            public string ModelOption;
            public string ModelArg;
            public string ParamOption;
            public string ParamArg;

            public Names(int i)
            {
                Index = i;
                ModelOption = $"mopt_{i:D2}";
                ModelArg = $"marg_{i:D2}";
                ParamOption = $"popt_{i:D2}";
                ParamArg = $"parg_{i:D2}";
            }
        }

        [Fact]
        public void AssignmentsRetainOrder()
        {
            var model = new Model
            {
                mopt_01 = 1,
                mopt_02 = 2,
                mopt_03 = 3,
                mopt_04 = 4,
                mopt_05 = 5,
                mopt_06 = 6,
                mopt_07 = 7,
                mopt_08 = 8,
                mopt_09 = 9,
                mopt_10 = 10,
                mopt_11 = 11,
                mopt_12 = 12,
                mopt_13 = 13,
                mopt_14 = 14,
                mopt_15 = 15,
                mopt_16 = 16,
                mopt_17 = 17,
                mopt_18 = 18,
                mopt_19 = 19,
                mopt_20 = 20,
                mopt_21 = 21,
                mopt_22 = 22,
                mopt_23 = 23,
                mopt_24 = 24,
                mopt_25 = 25,
                mopt_26 = 26,
                mopt_27 = 27,
                mopt_28 = 28,
                mopt_29 = 29,
                mopt_30 = 30,
                mopt_31 = 31,
                mopt_32 = 32,
                mopt_33 = 33,
                mopt_34 = 34,
                mopt_35 = 35,
                mopt_36 = 36,
                mopt_37 = 37,
                mopt_38 = 38,
                mopt_39 = 39,
                mopt_40 = 40,
                mopt_41 = 41,
                mopt_42 = 42,
                mopt_43 = 43,
                mopt_44 = 44,
                mopt_45 = 45,
                mopt_46 = 46,
                mopt_47 = 47,
                mopt_48 = 48,
                mopt_49 = 49,
                mopt_50 = 50,
                marg_01 = 1,
                marg_02 = 2,
                marg_03 = 3,
                marg_04 = 4,
                marg_05 = 5,
                marg_06 = 6,
                marg_07 = 7,
                marg_08 = 8,
                marg_09 = 9,
                marg_10 = 10,
                marg_11 = 11,
                marg_12 = 12,
                marg_13 = 13,
                marg_14 = 14,
                marg_15 = 15,
                marg_16 = 16,
                marg_17 = 17,
                marg_18 = 18,
                marg_19 = 19,
                marg_20 = 20,
                marg_21 = 21,
                marg_22 = 22,
                marg_23 = 23,
                marg_24 = 24,
                marg_25 = 25,
                marg_26 = 26,
                marg_27 = 27,
                marg_28 = 28,
                marg_29 = 29,
                marg_30 = 30,
                marg_31 = 31,
                marg_32 = 32,
                marg_33 = 33,
                marg_34 = 34,
                marg_35 = 35,
                marg_36 = 36,
                marg_37 = 37,
                marg_38 = 38,
                marg_39 = 39,
                marg_40 = 40,
                marg_41 = 41,
                marg_42 = 42,
                marg_43 = 43,
                marg_44 = 44,
                marg_45 = 45,
                marg_46 = 46,
                marg_47 = 47,
                marg_48 = 48,
                marg_49 = 49,
                marg_50 = 50
            };
            var numbers = Enumerable.Range(1,50).Cast<object>().ToArray();
            var paramValues = model.ToEnumerable().Concat(numbers).Concat(numbers).ToArray();
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Do " +
                           "--mopt_01=1 --mopt_02=2 --mopt_03=3 --mopt_04=4 --mopt_05=5 --mopt_06=6 --mopt_07=7 --mopt_08=8 --mopt_09=9 --mopt_10=10 --mopt_11=11 --mopt_12=12 --mopt_13=13 --mopt_14=14 --mopt_15=15 --mopt_16=16 --mopt_17=17 --mopt_18=18 --mopt_19=19 --mopt_20=20 --mopt_21=21 --mopt_22=22 --mopt_23=23 --mopt_24=24 --mopt_25=25 --mopt_26=26 --mopt_27=27 --mopt_28=28 --mopt_29=29 --mopt_30=30 --mopt_31=31 --mopt_32=32 --mopt_33=33 --mopt_34=34 --mopt_35=35 --mopt_36=36 --mopt_37=37 --mopt_38=38 --mopt_39=39 --mopt_40=40 --mopt_41=41 --mopt_42=42 --mopt_43=43 --mopt_44=44 --mopt_45=45 --mopt_46=46 --mopt_47=47 --mopt_48=48 --mopt_49=49 --mopt_50=50 " +
                           "--popt_01=1 --popt_02=2 --popt_03=3 --popt_04=4 --popt_05=5 --popt_06=6 --popt_07=7 --popt_08=8 --popt_09=9 --popt_10=10 --popt_11=11 --popt_12=12 --popt_13=13 --popt_14=14 --popt_15=15 --popt_16=16 --popt_17=17 --popt_18=18 --popt_19=19 --popt_20=20 --popt_21=21 --popt_22=22 --popt_23=23 --popt_24=24 --popt_25=25 --popt_26=26 --popt_27=27 --popt_28=28 --popt_29=29 --popt_30=30 --popt_31=31 --popt_32=32 --popt_33=33 --popt_34=34 --popt_35=35 --popt_36=36 --popt_37=37 --popt_38=38 --popt_39=39 --popt_40=40 --popt_41=41 --popt_42=42 --popt_43=43 --popt_44=44 --popt_45=45 --popt_46=46 --popt_47=47 --popt_48=48 --popt_49=49 --popt_50=50 " +
                           "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 " +
                           "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 "},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(paramValues)}
            });
        }

        private class App
        {
            public void Do(Model model,
                [Option] int popt_01, [Option] int popt_02, [Option] int popt_03, [Option] int popt_04, [Option] int popt_05, [Option] int popt_06, [Option] int popt_07, [Option] int popt_08, [Option] int popt_09, [Option] int popt_10, [Option] int popt_11, [Option] int popt_12, [Option] int popt_13, [Option] int popt_14, [Option] int popt_15, [Option] int popt_16, [Option] int popt_17, [Option] int popt_18, [Option] int popt_19, [Option] int popt_20, [Option] int popt_21, [Option] int popt_22, [Option] int popt_23, [Option] int popt_24, [Option] int popt_25, [Option] int popt_26, [Option] int popt_27, [Option] int popt_28, [Option] int popt_29, [Option] int popt_30, [Option] int popt_31, [Option] int popt_32, [Option] int popt_33, [Option] int popt_34, [Option] int popt_35, [Option] int popt_36, [Option] int popt_37, [Option] int popt_38, [Option] int popt_39, [Option] int popt_40, [Option] int popt_41, [Option] int popt_42, [Option] int popt_43, [Option] int popt_44, [Option] int popt_45, [Option] int popt_46, [Option] int popt_47, [Option] int popt_48, [Option] int popt_49, [Option] int popt_50,
[Operand] int parg_01, [Operand] int parg_02, [Operand] int parg_03, [Operand] int parg_04, [Operand] int parg_05, [Operand] int parg_06, [Operand] int parg_07, [Operand] int parg_08, [Operand] int parg_09, [Operand] int parg_10, [Operand] int parg_11, [Operand] int parg_12, [Operand] int parg_13, [Operand] int parg_14, [Operand] int parg_15, [Operand] int parg_16, [Operand] int parg_17, [Operand] int parg_18, [Operand] int parg_19, [Operand] int parg_20, [Operand] int parg_21, [Operand] int parg_22, [Operand] int parg_23, [Operand] int parg_24, [Operand] int parg_25, [Operand] int parg_26, [Operand] int parg_27, [Operand] int parg_28, [Operand] int parg_29, [Operand] int parg_30, [Operand] int parg_31, [Operand] int parg_32, [Operand] int parg_33, [Operand] int parg_34, [Operand] int parg_35, [Operand] int parg_36, [Operand] int parg_37, [Operand] int parg_38, [Operand] int parg_39, [Operand] int parg_40, [Operand] int parg_41, [Operand] int parg_42, [Operand] int parg_43, [Operand] int parg_44, [Operand] int parg_45, [Operand] int parg_46, [Operand] int parg_47, [Operand] int parg_48, [Operand] int parg_49, [Operand] int parg_50)
            {
            }
        }
        public class Model : IArgumentModel
        {
            [Option]
            public int mopt_01 { get; set; }
            [Option]
            public int mopt_02 { get; set; }
            [Option]
            public int mopt_03 { get; set; }
            [Option]
            public int mopt_04 { get; set; }
            [Option]
            public int mopt_05 { get; set; }
            [Option]
            public int mopt_06 { get; set; }
            [Option]
            public int mopt_07 { get; set; }
            [Option]
            public int mopt_08 { get; set; }
            [Option]
            public int mopt_09 { get; set; }
            [Option]
            public int mopt_10 { get; set; }
            [Option]
            public int mopt_11 { get; set; }
            [Option]
            public int mopt_12 { get; set; }
            [Option]
            public int mopt_13 { get; set; }
            [Option]
            public int mopt_14 { get; set; }
            [Option]
            public int mopt_15 { get; set; }
            [Option]
            public int mopt_16 { get; set; }
            [Option]
            public int mopt_17 { get; set; }
            [Option]
            public int mopt_18 { get; set; }
            [Option]
            public int mopt_19 { get; set; }
            [Option]
            public int mopt_20 { get; set; }
            [Option]
            public int mopt_21 { get; set; }
            [Option]
            public int mopt_22 { get; set; }
            [Option]
            public int mopt_23 { get; set; }
            [Option]
            public int mopt_24 { get; set; }
            [Option]
            public int mopt_25 { get; set; }
            [Option]
            public int mopt_26 { get; set; }
            [Option]
            public int mopt_27 { get; set; }
            [Option]
            public int mopt_28 { get; set; }
            [Option]
            public int mopt_29 { get; set; }
            [Option]
            public int mopt_30 { get; set; }
            [Option]
            public int mopt_31 { get; set; }
            [Option]
            public int mopt_32 { get; set; }
            [Option]
            public int mopt_33 { get; set; }
            [Option]
            public int mopt_34 { get; set; }
            [Option]
            public int mopt_35 { get; set; }
            [Option]
            public int mopt_36 { get; set; }
            [Option]
            public int mopt_37 { get; set; }
            [Option]
            public int mopt_38 { get; set; }
            [Option]
            public int mopt_39 { get; set; }
            [Option]
            public int mopt_40 { get; set; }
            [Option]
            public int mopt_41 { get; set; }
            [Option]
            public int mopt_42 { get; set; }
            [Option]
            public int mopt_43 { get; set; }
            [Option]
            public int mopt_44 { get; set; }
            [Option]
            public int mopt_45 { get; set; }
            [Option]
            public int mopt_46 { get; set; }
            [Option]
            public int mopt_47 { get; set; }
            [Option]
            public int mopt_48 { get; set; }
            [Option]
            public int mopt_49 { get; set; }
            [Option]
            public int mopt_50 { get; set; }
            [Operand]
            public int marg_01 { get; set; }
            [Operand]
            public int marg_02 { get; set; }
            [Operand]
            public int marg_03 { get; set; }
            [Operand]
            public int marg_04 { get; set; }
            [Operand]
            public int marg_05 { get; set; }
            [Operand]
            public int marg_06 { get; set; }
            [Operand]
            public int marg_07 { get; set; }
            [Operand]
            public int marg_08 { get; set; }
            [Operand]
            public int marg_09 { get; set; }
            [Operand]
            public int marg_10 { get; set; }
            [Operand]
            public int marg_11 { get; set; }
            [Operand]
            public int marg_12 { get; set; }
            [Operand]
            public int marg_13 { get; set; }
            [Operand]
            public int marg_14 { get; set; }
            [Operand]
            public int marg_15 { get; set; }
            [Operand]
            public int marg_16 { get; set; }
            [Operand]
            public int marg_17 { get; set; }
            [Operand]
            public int marg_18 { get; set; }
            [Operand]
            public int marg_19 { get; set; }
            [Operand]
            public int marg_20 { get; set; }
            [Operand]
            public int marg_21 { get; set; }
            [Operand]
            public int marg_22 { get; set; }
            [Operand]
            public int marg_23 { get; set; }
            [Operand]
            public int marg_24 { get; set; }
            [Operand]
            public int marg_25 { get; set; }
            [Operand]
            public int marg_26 { get; set; }
            [Operand]
            public int marg_27 { get; set; }
            [Operand]
            public int marg_28 { get; set; }
            [Operand]
            public int marg_29 { get; set; }
            [Operand]
            public int marg_30 { get; set; }
            [Operand]
            public int marg_31 { get; set; }
            [Operand]
            public int marg_32 { get; set; }
            [Operand]
            public int marg_33 { get; set; }
            [Operand]
            public int marg_34 { get; set; }
            [Operand]
            public int marg_35 { get; set; }
            [Operand]
            public int marg_36 { get; set; }
            [Operand]
            public int marg_37 { get; set; }
            [Operand]
            public int marg_38 { get; set; }
            [Operand]
            public int marg_39 { get; set; }
            [Operand]
            public int marg_40 { get; set; }
            [Operand]
            public int marg_41 { get; set; }
            [Operand]
            public int marg_42 { get; set; }
            [Operand]
            public int marg_43 { get; set; }
            [Operand]
            public int marg_44 { get; set; }
            [Operand]
            public int marg_45 { get; set; }
            [Operand]
            public int marg_46 { get; set; }
            [Operand]
            public int marg_47 { get; set; }
            [Operand]
            public int marg_48 { get; set; }
            [Operand]
            public int marg_49 { get; set; }
            [Operand]
            public int marg_50 { get; set; }
        }
        
        [Fact(Skip = "Only run this test when you want to modify the number of arguments used in BigArgumentsSets")]
        public void Generator()
        {
            var range = Enumerable.Range(1, 50).Select(i => new Names(i)).ToArray();

            GenerateModel(range);
            GenerateCommandMethod(range);
            GenerateScenario(range);
        }

        private void GenerateScenario(Names[] range)
        {
            _output.WriteLine("\n  --- generate scenario --- \n");

            // expectedAssignment
            var assignExpectedOptionValues = range.Select(i => $"{i.ModelOption} = {i.Index}").ToCsv(", ");
            var assignExpectedArgValues = range.Select(i => $"{i.ModelArg} = {i.Index}").ToCsv(", ");

            // whenArgs
            var modelOptionsDo = range.Select(i => $"--{i.ModelOption}={i.Index}").ToCsv(" ");
            var paramOptionsDo = range.Select(i => $"--{i.ParamOption}={i.Index}").ToCsv(" ");
            var argsDo = range.Select(i => $"{i.Index}").ToCsv(" ");

            _output.WriteLine(
                $"var expectedAssignment = new Model\n{{\n{assignExpectedOptionValues},\n{assignExpectedArgValues}\n}};");
            _output.WriteLine("Verify(new Given<App>\n{");
            _output.WriteLine(
                $"When = {{Args = \"Do \" +\n\"{modelOptionsDo} \" +\n\"{paramOptionsDo} \" +\n\"{argsDo} \" +\n\"{argsDo} \"}},");
            _output.WriteLine("Then = {Outputs = {new Assignments {Model = expectedAssignment, Params = expectedAssignment}}}");
            _output.WriteLine("});");
        }

        private void GenerateCommandMethod(Names[] range)
        {
            _output.WriteLine("\n  --- generate Do method --- \n");

            // generate parameters
            var paramOptions = range.Select(i => $"[Option] int {i.ParamOption}");
            var paramArgs = range.Select(i => $"[Operand] int {i.ParamArg}");
            var methodParams = $"\n                {paramOptions.ToCsv()},\n                {paramArgs.ToCsv()}";

            // generate paramsModel
            var assignModelOptionsFromParams = range.Select(i => $"{i.ModelOption} = {i.ParamOption}").ToCsv(", ");
            var assignModelArgsFromParams = range.Select(i => $"{i.ModelArg} = {i.ParamArg}").ToCsv(", ");

            _output.WriteLine($"public void Do(Model model,{methodParams})\n{{");
            _output.WriteLine($"var paramsModel = new Model{{\n{assignModelOptionsFromParams},\n{assignModelArgsFromParams}\n}};\n");
            _output.WriteLine("TestOutputs.Capture(new Assignments { Model = model, Params = paramsModel }); ");
            _output.WriteLine("}");
        }

        private void GenerateModel(Names[] range)
        {
            _output.WriteLine("\n  --- generate model --- \n");

            var modelOptions = range.Select(i => $"[Option]\npublic int {i.ModelOption} {{ get; set; }}");
            var modelArgs = range.Select(i => $"[Operand]\npublic int {i.ModelArg} {{ get; set; }}");
            _output.WriteLine("public class Model : IArgumentModel\n{");
            _output.WriteLine(modelOptions.Union(modelArgs).ToCsv(Environment.NewLine));
            _output.WriteLine("}");
        }
    }
}
