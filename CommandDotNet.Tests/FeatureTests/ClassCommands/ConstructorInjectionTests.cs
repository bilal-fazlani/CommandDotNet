using System.Threading;
using CommandDotNet.ConsoleOnly;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class ConstructorInjectionTests
    {
        public ConstructorInjectionTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void ShouldInstantiateCommandClassUsingCtorWithMostInjectableServices()
        {
            var result = new AppRunner<UseLargestCtorApp>().RunInMem("Do");
            var app = result.CommandContext.GetCommandInvocationInfo<UseLargestCtorApp>().Instance;
            app!.CommandContext.Should().NotBeNull();
            app.Console.Should().NotBeNull();
            app.CancellationToken.Should().NotBeNull();
        }

        class UseLargestCtorApp
        {
            public CommandContext CommandContext = null!;
            public IConsole Console = null!;
            public CancellationToken CancellationToken;

            public UseLargestCtorApp()
            {
            }

            public UseLargestCtorApp(CommandContext commandContext)
            {
                CommandContext = commandContext;
            }

            public UseLargestCtorApp(CommandContext commandContext, IConsole console)
            {
                CommandContext = commandContext;
                Console = console;
            }

            public UseLargestCtorApp(CommandContext commandContext, IConsole console, CancellationToken cancellationToken)
            {
                CommandContext = commandContext;
                Console = console;
                CancellationToken = cancellationToken;
            }

            public void Do()
            {
            }
        }
    }
}