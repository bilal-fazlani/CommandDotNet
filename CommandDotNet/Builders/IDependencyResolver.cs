using System;

namespace CommandDotNet.Builders
{
    public interface IDependencyResolver
    {
        object? Resolve(Type type);
        bool TryResolve(Type type, out object? item);
    }
}