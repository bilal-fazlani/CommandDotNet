using CommandDotNet.TestTools.Scenarios;
using NUnit.Framework;

namespace CommandDotNet.Example.Tests
{
    [TestFixture]
    public class ExampleCommandTests
    {
        [Test]
        public void Help_Should_Include_ExtendedHelpText()
        {
            // Must convert NewLine to what was given
            // to the ExtendedHelpText property
            Program.GetAppRunner()
                .Verify(new Scenario
                {
                    When = {Args = null},
                    Then =
                    {
                        OutputContainsTexts =
                        {
                            @$"
Directives:
  [debug] to attach a debugger to the app
  [parse] to output how the inputs were tokenized
  [log] to output framework logs to the console
  [log:debug|info|warn|error|fatal] to output framework logs for the given level or above

directives must be specified before any commands and arguments.

Example: testhost.dll [debug] [parse] [log:info] cancel-me".Replace("\r\n", "\n")
                        }
                    }
                });
        }

        [Test]
        public void Should_Include_Version_Option()
        {
            // The directives section from the command must match
            Program.GetAppRunner()
                .Verify(new Scenario
                {
                    When = { Args = null },
                    Then =
                    {
                        OutputContainsTexts =
                        {
                            @"-v | --version
  Show version information",
                        }
                    }
                });
        }
    }
}