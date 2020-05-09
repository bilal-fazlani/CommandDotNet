using System.Collections.Generic;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class StringCtorObjectsAsArgumentsTests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public StringCtorObjectsAsArgumentsTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void BasicHelp_Includes_StringCtorObjects()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do <arg>

Arguments:
  arg
"
                }
            });
        }

        [Fact]
        public void BasicHelp_List_Includes_StringCtorObjects()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "DoList -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll DoList <args>

Arguments:
  args
"
                }
            });
        }

        [Fact]
        public void DetailedHelp_Includes_StringCtorObjects()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do <arg>

Arguments:

  arg  <FILENAME>
"
                }
            });
        }

        [Fact]
        public void DetailedHelp_List_Includes_StringCtorObjects()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "DoList -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll DoList <args>

Arguments:

  args (Multiple)  <FILENAME>
"
                }
            });
        }

        [Fact]
        public void Exec_ConvertsStringToObject()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "DoList some-value another-value"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new List<StringCtorObject>
                        {
                            new StringCtorObject("some-value"),
                            new StringCtorObject("another-value")
                        })
                }
            });
        }

        [Fact]
        public void Exec_List_ConvertsStringToObject()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Do some-value"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new StringCtorObject("some-value"))
                }
            });
        }

        private class App
        {
            public void Do(StringCtorObject arg)
            {
            }

            public void DoList(List<StringCtorObject> args)
            {
            }
        }

        private class StringCtorObject
        {
            public string Filename { get; }

            public StringCtorObject(string filename)
            {
                Filename = filename;
            }
        }
    }
}
