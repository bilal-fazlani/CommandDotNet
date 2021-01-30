using System;
using System.Collections.Generic;
using CommandDotNet.Tests.FeatureTests.Arguments.Models.ArgsAsArgModels;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_DefinedAsArgModel_NoDefaults_Tests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public Options_DefinedAsArgModel_NoDefaults_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void SampleTypes_BasicHelp()
        {
            new AppRunner<OptionsNoDefaults>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "ArgsDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:
  --BoolArg
  --StringArg
  --StructArg
  --StructNArg
  --EnumArg
  --ObjectArg
  --StringListArg
  --StructListArg
  --EnumListArg
  --ObjectListArg
"
                }
            });
        }

        [Fact]
        public void SampleTypes_DetailedHelp()
        {
            new AppRunner<OptionsNoDefaults>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "ArgsDefaults -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll ArgsDefaults [options]

Options:

  --BoolArg

  --StringArg                 <TEXT>

  --StructArg                 <NUMBER>

  --StructNArg                <NUMBER>

  --EnumArg                   <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --ObjectArg                 <URI>

  --StringListArg (Multiple)  <TEXT>

  --StructListArg (Multiple)  <NUMBER>

  --EnumListArg (Multiple)    <DAYOFWEEK>
  Allowed values: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday

  --ObjectListArg (Multiple)  <URI>
"
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_Named()
        {
            new AppRunner<OptionsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsDefaults --StringArg green --StructArg 1 --StructNArg 2 " +
                           "--EnumArg Monday --ObjectArg http://google.com " +
                           "--StringListArg yellow --StringListArg orange " +
                           "--StructListArg 23 --StructListArg 5 " +
                           "--EnumListArg Friday --EnumListArg Tuesday " +
                           "--ObjectListArg http://apple.com --ObjectListArg http://github.com"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new OptionsNoDefaultsSampleTypesModel
                        {
                            StringArg = "green",
                            StructArg = 1,
                            StructNArg = 2,
                            EnumArg = DayOfWeek.Monday,
                            ObjectArg = new Uri("http://google.com"),
                            StringListArg = new List<string>{"yellow", "orange"},
                            StructListArg = new List<int>{23,5},
                            EnumListArg = new List<DayOfWeek>{DayOfWeek.Friday, DayOfWeek.Tuesday},
                            ObjectListArg = new List<Uri>
                            {
                                new Uri("http://apple.com"),
                                new Uri("http://github.com"),
                            }
                        })
                }
            });
        }

        [Fact]
        public void SampleTypes_Exec_OptionsNotRequired()
        {
            new AppRunner<OptionsNoDefaults>().Verify(new Scenario
            {
                When = {Args = "ArgsDefaults"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(new OptionsNoDefaultsSampleTypesModel())
                }
            });
        }

        private class OptionsNoDefaults
        {
            public void ArgsDefaults(OptionsNoDefaultsSampleTypesModel model)
            {
            }
        }
    }
}
