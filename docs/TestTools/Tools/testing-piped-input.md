# Testing Piped Input

PipedInput is easily tested using the TestConsole or either of the `RunInMem` or BDD `Verify` methods which use the TestConsole.

Simply supply an IEnumerable<string> to the optional PipedInput parameter or property.

=== "RunInMem"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            var result = new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .RunInMem("List aaa bbb", pipedInput: new[] { "ccc", "ddd" });

            result.ExitCode.Should().Be(0);
            result.OutputShouldBe(@"aaa
    bbb
    ccc
    ddd
    ");
        }
    }
    ```

=== "BDD Verify"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When = 
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
    }
    ```

=== "TestConsole"

    ```c#
    public class PipedInputTests
    {
        [Test]
        public void PipedInput_Should_UnionWithUserSuppliedValues()
        {
            var testConsole = new TestConsole(pipedInput: new[] { "ccc", "ddd" });
            var appRunner = new AppRunner<App>()
                .Configure(c => c.Console = testConsole);

            // remaining test code
        }
    }
    ```

