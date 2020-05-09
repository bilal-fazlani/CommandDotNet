using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.Extensions;

namespace CommandDotNet.Execution
{
    internal class ResolverService
    {
        internal ResolveStrategy ArgumentModelResolveStrategy { get; set; }
        internal ResolveStrategy CommandClassResolveStrategy { get; set; }
        internal IDependencyResolver? BackingResolver { private get; set; }

        internal object ResolveArgumentModel(Type modelType)
        {
            // Default uses TryResolve for IArgumentModel because they're
            // expected to be POCOs and not require DI.
            // DI can be used to share the instance with
            // other items in the scope or to load values
            // via the container, perhaps from configuration
            // sources.
            return ConditionalTryResolve(modelType, out var item, ArgumentModelResolveStrategy)
                ? item!
                : Activator.CreateInstance(modelType);
        }

        internal object ResolveCommandClass(Type classType, CommandContext commandContext)
        {
            // Default uses Resolve so the container can throw an exception if the class isn't registered.
            // if null is returned, then the container gives consent for other the class to
            // be created by this framework. 
            if (ConditionalTryResolve(classType, out var item, CommandClassResolveStrategy))
            {
                return item!;
            }

            var parameterResolversByType = commandContext.AppConfig.ParameterResolversByType;

            // NOTE: it's possible, that a class could appear more than once in the hierarchy.
            //       because this seems VERY unlikely to be needed, skip this case until
            //       proven it is needed to avoid additional state and complexity to
            //       reuse the instance.  the instance should be reused to map to expectation
            //       for DI that one instance is created per scope.
            var ctor = classType.GetConstructors()
                .Select(c => new { c, p = c.GetParameters() })
                .Where(cp => cp.p.Length == 0 || cp.p.All(p => parameterResolversByType.ContainsKey(p.ParameterType)))
                .OrderByDescending(cp => cp.p.Length)
                .FirstOrDefault();

            if (ctor == null)
            {
                throw new AppRunnerException(
                    $"No viable constructors found for {classType}. " +
                    $"Constructor can only contain parameters of type: {parameterResolversByType.Keys.ToOrderedCsv()}");
            }

            var parameters = ctor.p
                .Select(p => parameterResolversByType[p.ParameterType](commandContext))
                .ToArray();

            item = ctor.c.Invoke(parameters);

            // only dispose instances created by this middleware, not by a container
            RegisterDisposable(commandContext, item);
            return item;
        }

        internal void OnRunCompleted(CommandContext commandContext)
        {
            // When we support a cli session, we'll need to capture exceptions here
            // and best effort dispose of all instances
            commandContext.Services.GetOrDefault<Disposables>()?.Items.ForEach(i => i.Dispose());
        }

        private bool ConditionalTryResolve(Type type, out object? item, ResolveStrategy resolveStrategy)
        {
            if (BackingResolver == null)
            {
                item = null;
                return false;
            }

            if (resolveStrategy == ResolveStrategy.TryResolve)
            {
                return BackingResolver.TryResolve(type, out item);
            }

            item = BackingResolver.Resolve(type);
            if (item != null)
            {
                return true;
            }

            if (resolveStrategy == ResolveStrategy.ResolveOrThrow)
            {
                throw new ResolverReturnedNullException(type);
            }
            return false;
        }

        private static void RegisterDisposable(CommandContext commandContext, object item)
        {
            if (item is IDisposable disposableItem)
            {
                commandContext.Services.GetOrAdd(() => new Disposables()).Items.Add(disposableItem);
            }
        }

        private class Disposables
        {
            internal readonly List<IDisposable> Items = new List<IDisposable>();
        }
    }
}