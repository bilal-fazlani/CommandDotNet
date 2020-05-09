using System;
using Autofac;
using CommandDotNet.Execution;

namespace CommandDotNet.IoC.Autofac
{
    public static class AppRunnerConfigurationExtensions
    {
        /// <summary>Use the provided Autofac container to resolve command classes and <see cref="IArgumentModel"/>s</summary>
        /// <param name="appRunner">the <see cref="AppRunner"/></param>
        /// <param name="container">the Autofac container to use</param>
        /// <param name="runInScope">if provided, the scope will be created at the beginning of the run and disposed at the end</param>
        /// <param name="argumentModelResolveStrategy">
        /// the <see cref="ResolveStrategy"/> used to resolve <see cref="IArgumentModel"/>s.
        /// </param>
        /// <param name="commandClassResolveStrategy">
        /// the <see cref="ResolveStrategy"/> used to resolve command classes.
        /// </param>
        public static AppRunner UseAutofac(
            this AppRunner appRunner, 
            IContainer container, 
            Func<CommandContext, IDisposable>? runInScope = null,
            ResolveStrategy argumentModelResolveStrategy = ResolveStrategy.TryResolve,
            ResolveStrategy commandClassResolveStrategy = ResolveStrategy.Resolve)
        {
            return appRunner.UseDependencyResolver(new AutofacResolver(container),
                runInScope,
                argumentModelResolveStrategy,
                commandClassResolveStrategy);
        }
    }
}
