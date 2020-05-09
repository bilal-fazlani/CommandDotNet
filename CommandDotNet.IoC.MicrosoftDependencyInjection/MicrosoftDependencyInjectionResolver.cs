using System;
using CommandDotNet.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace CommandDotNet.IoC.MicrosoftDependencyInjection
{
    public class MicrosoftDependencyInjectionResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public MicrosoftDependencyInjectionResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Resolve(Type type)
        {
            return _serviceProvider.GetRequiredService(type);
        }

        public bool TryResolve(Type type, out object? item)
        {
            item = _serviceProvider.GetService(type);
            return item is { };
        }
    }
}