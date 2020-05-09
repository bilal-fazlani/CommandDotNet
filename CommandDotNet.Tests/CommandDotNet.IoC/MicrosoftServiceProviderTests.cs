using System;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using CommandDotNet.TestTools;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class MicrosoftServiceProviderTests
    {
        public MicrosoftServiceProviderTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void ShouldWork()
        {
            var serviceProvider = ConfigureMicrosoftServiceProvider();

            new AppRunner<IoCApp>()
                .UseMicrosoftDependencyInjection(serviceProvider)
                .RunInMem("Do");
        }

        [Fact]
        public void CanSpecifyScope()
        {
            var serviceProvider = ConfigureMicrosoftServiceProvider();
            ISomeIoCService svcBeforeRun;
            using (var scope = serviceProvider.CreateScope())
            {
                svcBeforeRun = serviceProvider.GetRequiredService<ISomeIoCService>();
            }

            var result = new AppRunner<IoCApp>()
                .UseMicrosoftDependencyInjection(serviceProvider, runInScope: ctx => serviceProvider.CreateScope())
                .RunInMem("Do");

            var app = result.CommandContext.GetCommandInvocationInfo<IoCApp>().Instance;

            app!.FromCtor.Should().BeSameAs(app.FromInterceptor);

            // TODO: why is this the same instance?
            //app.FromCtor.Should().NotBeSameAs(svcBeforeRun);
        }

        [Fact]
        public void WhenUnregisteredType_ShouldThrowException()
        {
            var serviceProvider = ConfigureMicrosoftServiceProvider(skipApp: true);

            Assert.Throws<InvalidOperationException>(() =>
                new AppRunner<IoCApp>()
                    .UseMicrosoftDependencyInjection(serviceProvider)
                    .RunInMem("Do")
            ).Message.Should().StartWith("No service for type 'CommandDotNet.Tests.CommandDotNet.IoC.IoCApp'");
        }

        private static IServiceProvider ConfigureMicrosoftServiceProvider(bool skipApp = false)
        {
            var serviceCollection = new ServiceCollection();
            if (!skipApp)
            {
                serviceCollection.AddScoped<IoCApp>();
            }
            serviceCollection.AddScoped<ISomeIoCService, SomeIoCService>();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}