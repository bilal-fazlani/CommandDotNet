using CommandDotNet.Prompts;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Prompting
{
    public class PrompterTests
    {
        public PrompterTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void InjectedPrompterCanPromptForValues()
        {
            new AppRunner<App>()
                .UsePrompting()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = "Do",
                        OnPrompt = Respond.WithText("who's there")
                    },
                    Then =
                    {
                        Output = @"knock knock: who's there
who's there
"
                    }
                });
        }

        public class App
        {
            public void Do(IConsole console, IPrompter prompter)
            {
                var answer = prompter.PromptForValue("knock knock", out bool isCancellationRequested);
                console.Out.WriteLine(answer);
            }
        }
    }
}