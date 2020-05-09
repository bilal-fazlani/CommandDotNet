using System;
using CommandDotNet.Builders;
using SimpleInjector;

namespace CommandDotNet.IoC.SimpleInjector
{
    public class SimpleInjectorResolver : IDependencyResolver
    {
        private readonly Container _container;

        public SimpleInjectorResolver(Container container)
        {
            _container = container;
        }

        public object Resolve(Type type)
        {
            return _container.GetInstance(type);
        }

        public bool TryResolve(Type type, out object? item)
        {
            // SimpleInjector's GetInstance will throw an exception if type is not registered.
            // Converting to IServiceProvider will return null if not registered.
            // https://stackoverflow.com/questions/10076206/prevent-simple-injector-to-throw-an-exception-when-resolving-an-unregistered-ser
            item = ((IServiceProvider) _container).GetService(type);
            return item is { };
        }
    }
}