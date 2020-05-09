using System.Linq;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class ResolveArgumentModelsTests
    {
        public ResolveArgumentModelsTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void ShouldUseModelFromDependencyResolver()
        {
            var argModel = new ArgModel {Text = "some default"};
            var result = new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver {new App(), argModel})
                .Verify(new Scenario
                {
                    When = {Args = "Do lala"},
                    Then =
                    {
                        AssertContext = ctx => 
                            ctx.GetCommandInvocationInfo().ParameterValues.First().Should().BeSameAs(argModel)
                    }
                });
        }

        [Fact]
        public void HelpShouldDisplayDefaultFromResolvedModel()
        {
            new AppRunner<App>()
                .UseDependencyResolver(new TestDependencyResolver { new ArgModel { Text = "default from resolver" } })
                .Verify(new Scenario
                {
                    When = { Args = "Do -h" },
                    Then = { OutputContainsTexts = { "default from resolver" } }
                });
        }

        class App
        {
            public void Do(ArgModel argModel)
            {
            }
        }

        class ArgModel : IArgumentModel
        {
            [Operand]
            public string Text { get; set; } = null!;
        }
    }
}