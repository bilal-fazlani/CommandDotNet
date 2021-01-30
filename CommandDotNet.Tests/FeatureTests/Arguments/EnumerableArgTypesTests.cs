using System.Collections.Generic;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class EnumerableArgTypesTests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public EnumerableArgTypesTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void EnumerableModel_BasicHelp_Includes_Arguments()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "EnumerableModel -h"},
                Then = { Output = @"Usage: dotnet testhost.dll EnumerableModel [options] <Operands>

Arguments:
  Operands

Options:
  --Options
" }
            });
        }

        [Fact]
        public void EnumerableModel_DetailedHelp_Includes_ArgumentsAsMultiple()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "EnumerableModel -h"},
                Then = { Output = @"Usage: dotnet testhost.dll EnumerableModel [options] <Operands>

Arguments:

  Operands (Multiple)  <TEXT>

Options:

  --Options (Multiple)  <TEXT>
" }
            });
        }

        [Fact]
        public void EnumerableParams_BasicHelp_Includes_Arguments()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "Enumerable -h"},
                Then = {Output = @"Usage: dotnet testhost.dll Enumerable [options] <operands>

Arguments:
  operands

Options:
  --options
" }
            });
        }

        [Fact]
        public void EnumerableParams_DetailedHelp_Includes_ArgumentsAsMultiple()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "Enumerable -h"},
                Then = {Output = @"Usage: dotnet testhost.dll Enumerable [options] <operands>

Arguments:

  operands (Multiple)  <TEXT>

Options:

  --options (Multiple)  <TEXT>
" }
            });
        }

        [Fact]
        public void EnumerableParams_Exec_MapsArguments()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Enumerable --options aaa --options bbb ccc ddd"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(new[] {"aaa", "bbb"}, new[] {"ccc", "ddd"})
                }
            });
        }

        [Fact]
        public void EnumerableModel_Exec_MapsArguments()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "EnumerableModel --Options aaa --Options bbb ccc ddd"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(new EnumerableModel
                    {
                        Options = new[] {"aaa", "bbb"},
                        Operands = new[] {"ccc", "ddd"}
                    })
                }
            });
        }

        [Fact]
        public void CollectionParams_Exec_MapsArguments()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Collection --options aaa --options bbb ccc ddd"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(new[] {"aaa", "bbb"}, new[] {"ccc", "ddd"})
                }
            });
        }

        [Fact]
        public void ArrayParams_Exec_MapsArguments()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Array --options aaa --options bbb ccc ddd"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(new[] {"aaa", "bbb"}, new[] {"ccc", "ddd"})
                }
            });
        }

        private class App
        {
            public void List([Option]List<string> options, List<string> operands)
            {
            }

            public void Enumerable([Option]IEnumerable<string> options, IEnumerable<string> operands)
            {
            }

            public void EnumerableModel(EnumerableModel model)
            {
            }

            public void Collection([Option]ICollection<string> options, ICollection<string> operands)
            {
            }

            public void Array([Option] string[] options, string[] operands)
            {
            }
        }

        private class EnumerableModel : IArgumentModel
        {
            [Option]
            public IEnumerable<string> Options { get; set; } = null!;

            [Operand]
            public IEnumerable<string> Operands { get; set; } = null!;
        }
    }
}
