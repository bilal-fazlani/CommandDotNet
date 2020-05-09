using Autofac;
using Autofac.Core.Registration;
using CommandDotNet.IoC.Autofac;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class AutofacTests
    {
        private object LifetimeScopeTag => "CommandDotNet.CallContext";

        public AutofacTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void ShouldWork()
        {
            var container = ConfigureAutofacContainer();

            new AppRunner<IoCApp>()
                .UseAutofac(container)
                .RunInMem("Do");
        }

        [Fact]
        public void CanSpecifyScope()
        {
            var container = ConfigureAutofacContainer();

            ISomeIoCService svcBeforeRun;
            using (var scope = container.BeginLifetimeScope(LifetimeScopeTag))
            {
                svcBeforeRun = container.Resolve<ISomeIoCService>();
            }

            var result = new AppRunner<IoCApp>()
                .UseAutofac(container, runInScope: ctx => container.BeginLifetimeScope(LifetimeScopeTag))
                .RunInMem("Do");

            var app = result.CommandContext.GetCommandInvocationInfo<IoCApp>().Instance;

            app!.FromCtor.Should().BeSameAs(app.FromInterceptor);

            // TODO: why is this the same instance?
            //app.FromCtor.Should().NotBeSameAs(svcBeforeRun);
        }

        [Fact]
        public void WhenUnregisteredType_ShouldThrowException()
        {
            var container = ConfigureAutofacContainer(skipApp: true);

            Assert.Throws<ComponentNotRegisteredException>(() =>
                new AppRunner<IoCApp>()
                    .UseAutofac(container, runInScope: ctx => container.BeginLifetimeScope(LifetimeScopeTag))
                    .RunInMem("Do")
            ).Message.Should().StartWith("The requested service 'CommandDotNet.Tests.CommandDotNet.IoC.IoCApp'");

        }

        private static IContainer ConfigureAutofacContainer(bool skipApp = false)
        {
            var containerBuilder = new ContainerBuilder();
            if (!skipApp)
            {
                containerBuilder.RegisterType<IoCApp>().InstancePerLifetimeScope();
            }
            containerBuilder.RegisterType<SomeIoCService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SomeIoCService>().As<ISomeIoCService>().InstancePerLifetimeScope();
            return containerBuilder.Build();
        }
    }
}