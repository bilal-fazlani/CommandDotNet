using CommandDotNet.Extensions;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools.Scenarios;
using CommandDotNet.Tokens;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CustomTokenTransformationTests
    {
        public CustomTokenTransformationTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void CanRegisterCustomTokenTransformation()
        {
            new AppRunner<App>()
                .Configure(c =>
                    c.UseTokenTransformation("test", 1,
                        (ctx, tokens) => tokens.Transform(
                            skipDirectives: true,
                            skipSeparated: true,
                            transformation: t =>
                                t.TokenType == TokenType.Value && t.Value == "like"
                                    ? Tokenizer.TokenizeValue("roses").ToEnumerable()
                                    : t.ToEnumerable())))
                .Verify(new Scenario
                {
                    When = {Args = "Do --opt1 smells like"},
                    Then =
                    {
                        Output = "smells roses"
                    }
                });
        }

        public class App
        {
            public void Do(IConsole console, [Option] string opt1, string arg1 = "wet dog")
            {
                console.Out.Write($"{opt1} {arg1}");
            }
        }
    }
}
