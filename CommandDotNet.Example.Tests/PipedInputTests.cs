using System;
using System.Collections.Generic;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using NUnit.Framework;

namespace CommandDotNet.Example.Tests
{
    [TestFixture]
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            var result = new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .RunInMem("List aaa bbb", pipedInput: new[] { "ccc", "ddd" });

            result.ExitCode.Should().Be(0);
            result.Console.AllText().Should().Be(@"aaa
bbb
ccc
ddd
");
        }

        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues_BDD()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When=
                    {
                        Args = "List aaa bbb",
                        PipedInput = new[] { "ccc", "ddd" }
                    },
                    Then =
                    {
                        Output = @"aaa
bbb
ccc
ddd
"
                    }
                });
        }

        private class App
        {
            public void List(IConsole console, List<string> args) =>
                console.WriteLine(string.Join(Environment.NewLine, args));
        }
    }
}