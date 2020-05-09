using CommandDotNet.IoC.SimpleInjector;
using CommandDotNet.TestTools;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class SimpleInjectorTests
    {
        public SimpleInjectorTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }
        
        [Fact]
        public void ShouldWork()
        {
            var container = new Container();
            container.Register<IoCApp>();
            container.Register<ISomeIoCService, SomeIoCService>();

            new AppRunner<IoCApp>()
                .UseSimpleInjector(container)
                .RunInMem("Do");
        }

        [Fact]
        public void CanSpecifyScope()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Register<IoCApp>(Lifestyle.Scoped);
            container.Register<ISomeIoCService, SomeIoCService>(Lifestyle.Scoped);

            ISomeIoCService svcBeforeRun;
            using (var scope = AsyncScopedLifestyle.BeginScope(container))
            {
                svcBeforeRun = container.GetInstance<ISomeIoCService>();
            }

            var result = new AppRunner<IoCApp>()
                .TrackingInvocations()
                .UseSimpleInjector(container, runInScope: ctx => AsyncScopedLifestyle.BeginScope(container))
                .RunInMem("Do");

            var app = result.CommandContext.GetCommandInvocationInfo<IoCApp>().Instance;

            app!.FromCtor.Should().BeSameAs(app.FromInterceptor);
            app.FromCtor.Should().NotBeSameAs(svcBeforeRun);
        }

        [Fact]
        public void WhenUnregisteredType_ShouldThrowException()
        {
            var container = new Container();
            // SimpleInjector will implicit register App if ISomeIoCService is registered

            Assert.Throws<ActivationException>(() =>
                new AppRunner<IoCApp>()
                    .UseSimpleInjector(container)
                    .RunInMem("Do")
            ).Message.Should().StartWith("No registration for type IoCApp could be found and an implicit registration could not be made.");
        }
    }
}