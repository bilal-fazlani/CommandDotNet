using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Builders
{
    internal static class DependencyResolverMiddleware
    {
        internal static AppRunner UseDependencyResolver(AppRunner appRunner, 
            IDependencyResolver dependencyResolver,
            Func<CommandContext, IDisposable>? runInScope,
            ResolveStrategy argumentModelResolveStrategy,
            ResolveStrategy commandClassResolveStrategy)
        {
            return appRunner.Configure(c =>
            {
                c.DependencyResolver = dependencyResolver;
                c.Services.Add(new ResolverService
                {
                    ArgumentModelResolveStrategy = argumentModelResolveStrategy,
                    CommandClassResolveStrategy = commandClassResolveStrategy
                });
                if (runInScope != null)
                {
                    c.UseMiddleware(RunInScope, MiddlewareSteps.DependencyResolver.BeginScope);
                    c.Services.Add(new Config(runInScope));
                }
            });
        }

        private static Task<int> RunInScope(CommandContext context, ExecutionDelegate next)
        {
            var config = context.AppConfig.Services.GetOrThrow<Config>();
            using (config.RunInScopeCallback(context))
            {
                return next(context);
            }
        }

        private class Config
        {
            public Func<CommandContext, IDisposable> RunInScopeCallback;

            public Config(Func<CommandContext, IDisposable> runInScopeCallback)
            {
                RunInScopeCallback = runInScopeCallback ?? throw new ArgumentNullException(nameof(runInScopeCallback));
            }
        }
    }
}