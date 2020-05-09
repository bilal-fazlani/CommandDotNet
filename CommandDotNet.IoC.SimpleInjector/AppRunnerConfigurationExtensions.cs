using System;
using CommandDotNet.Execution;
using SimpleInjector;

namespace CommandDotNet.IoC.SimpleInjector
{
    public static class AppRunnerConfigurationExtensions
    {
        /// <summary>Use the provided SimpleInjector container to resolve command classes and <see cref="IArgumentModel"/>s</summary>
        /// <param name="appRunner">the <see cref="AppRunner"/></param>
        /// <param name="container">the SimpleInjector container to use</param>
        /// <param name="runInScope">if provided, the scope will be created at the beginning of the run and disposed at the end</param>
        /// <param name="argumentModelResolveStrategy">
        /// the <see cref="ResolveStrategy"/> used to resolve <see cref="IArgumentModel"/>s.
        /// </param>
        /// <param name="commandClassResolveStrategy">
        /// the <see cref="ResolveStrategy"/> used to resolve command classes.
        /// </param>
        public static AppRunner UseSimpleInjector(
            this AppRunner appRunner, 
            Container container, 
            Func<CommandContext, IDisposable>? runInScope = null,
            ResolveStrategy argumentModelResolveStrategy = ResolveStrategy.TryResolve,
            ResolveStrategy commandClassResolveStrategy = ResolveStrategy.Resolve)
        {
            return appRunner.UseDependencyResolver(new SimpleInjectorResolver(container),
                runInScope,
                argumentModelResolveStrategy,
                commandClassResolveStrategy);
        }
    }
}
