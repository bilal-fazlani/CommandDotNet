using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommandDotNet.Builders;
using CommandDotNet.ConsoleOnly;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Parsing;
using CommandDotNet.Tokens;

namespace CommandDotNet
{
    public class AppConfigBuilder
    {
        private short _orderAdded = 0;
        
        private readonly SingleRegistrationGuard<ExecutionMiddleware> _middlewareSingleRegistrationGuard = 
            new SingleRegistrationGuard<ExecutionMiddleware>(
                "middleware", 
                middleware => middleware.Method.FullName(includeNamespace: true));
        private readonly SortedDictionary<MiddlewareStages, List<(ExecutionMiddleware middleware, short order, short orderAdded)>> _middlewareByStage = 
            new SortedDictionary<MiddlewareStages, List<(ExecutionMiddleware middleware, short order, short orderAdded)>>();

        private readonly SingleRegistrationGuard<string> _tokenTransformationSingleRegistrationGuard = 
            new SingleRegistrationGuard<string>("token transformation", name => name);
        private readonly Dictionary<string, TokenTransformation> _tokenTransformationsByName = 
            new Dictionary<string, TokenTransformation>();

        private readonly SingleRegistrationGuard<Type> _parameterResolverSingleRegistrationGuard = 
            new SingleRegistrationGuard<Type>("parameter resolver", type => type.FullName);
        private readonly Dictionary<Type, Func<CommandContext, object>> _parameterResolversByType = new Dictionary<Type, Func<CommandContext, object>>
        {
            [typeof(CommandContext)] = ctx => ctx,
            [typeof(IConsole)] = ctx => ctx.Console,
            [typeof(CancellationToken)] = ctx => ctx.CancellationToken
        };

        public AppConfigBuilder(AppSettings appSettings)
        {
            AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            NameTransformation = (attributes, memberName, overrideName, commandNodeType) => overrideName ?? memberName;
        }

        public AppSettings AppSettings { get; }

        /// <summary>
        /// Configures the app to use the resolver to create instances
        /// of command classes and argument models.<br/>
        /// use appRunner.UseDependencyResolver(...) extension method to set this instance.
        /// </summary>
        public IDependencyResolver? DependencyResolver { get; internal set; }

        /// <summary>Replace the internal help provider with given help provider</summary>
        public IHelpProvider? CustomHelpProvider { get; set; }

        /// <summary>
        /// Add a name transformation to enforce name consistency across commands, operands and options.<br/>
        /// memberName: The name of the class, method, property or parameter defining the class or argument.<br/>
        /// nameOverride:The name provided via attribute or other extensibility point.<br/>
        /// commandNodeType: the <see cref="CommandNodeType"/> the name is for.
        /// </summary>
        /// <remarks>
        /// To enforce casing rules, configure `appRunner.UseNameCasing(...)` from the nuget package CommandDotNet.NameCasing
        /// </remarks>
        public NameTransformation? NameTransformation { get; set; }

        /// <summary>Replace the internal system console with provided console</summary>
        public IConsole Console { get; set; } = new SystemConsole();

        /// <summary>
        /// <see cref="OnRunCompleted"/> is triggered when after the pipeline has completed execution.
        /// Use this to cleanup any events after the run.<br/>
        /// This is useful for tests and when using new AppRunner instances to create a cli session.
        /// </summary>
        public event Action<OnRunCompletedEventArgs>? OnRunCompleted;

        public BuildEvents BuildEvents { get; } = new BuildEvents();
        public TokenizationEvents TokenizationEvents { get; } = new TokenizationEvents();

        /// <summary>
        /// Services registered for the lifetime of the application.<br/>
        /// Use to store configurations for use by middleware.<br/>
        /// </summary>
        public Services Services { get; } = new Services();

        /// <summary>
        /// Resolvers functions registered here are available to inject into constructors, interceptor methods and command methods.<br/>
        /// Types must be resolvable from the <see cref="CommandContext"/><br/>
        /// Default types: <see cref="CommandContext"/>, <see cref="IConsole"/>, <see cref="CancellationToken"/>
        /// </summary>
        public AppConfigBuilder UseParameterResolver<T>(Func<CommandContext,T> resolver) where T: class
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            _parameterResolverSingleRegistrationGuard.Register(typeof(T));
            _parameterResolversByType.Add(typeof(T), resolver);
            return this;
        }

        /// <summary>
        /// Adds the transformation to the list of transformations applied to tokens
        /// before they are parsed into commands and arguments
        /// </summary>
        public AppConfigBuilder UseTokenTransformation(string name, int order, Func<CommandContext, TokenCollection, TokenCollection> transformation)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (transformation == null) throw new ArgumentNullException(nameof(transformation));
            
            _tokenTransformationSingleRegistrationGuard.Register(name);
            _tokenTransformationsByName.Add(name, new TokenTransformation(name, order, transformation));

            return this;
        }

        /// <summary>
        /// Adds the middleware to the pipeline in the specified <see cref="MiddlewareStep"/>
        /// </summary>
        public AppConfigBuilder UseMiddleware(ExecutionMiddleware middleware, MiddlewareStep step, 
            bool allowMultipleRegistrations = false)
        {
            if (!allowMultipleRegistrations)
            {
                _middlewareSingleRegistrationGuard.Register(middleware);
            }
            
            var values = _middlewareByStage
                .GetOrAdd(step.Stage, s => new List<(ExecutionMiddleware, short, short)>());

            values.Add((middleware, step.OrderWithinStage, _orderAdded++));

            return this;
        }

        /// <summary>
        /// Adds the middleware to the pipeline in the specified <see cref="MiddlewareStages"/>.
        /// Use <see cref="orderWithinStage"/> to specify order in relation
        /// to other middleware within the same stage.
        /// </summary>
        public AppConfigBuilder UseMiddleware(ExecutionMiddleware middleware, MiddlewareStages stage, 
            short? orderWithinStage = null, bool allowMultipleRegistrations = false)
        {
            return UseMiddleware(middleware, new MiddlewareStep(stage, orderWithinStage), allowMultipleRegistrations);
        }

        internal AppConfig Build()
        {
            var helpProvider = CustomHelpProvider ?? HelpTextProviderFactory.Create(AppSettings);

            var middlewarePipeline = _middlewareByStage
                .SelectMany(kvp =>
                    kvp.Value.Select(v => new { stage = kvp.Key, v.order, v.orderAdded, v.middleware }))
                .OrderBy(m => m.stage)
                .ThenBy(m => m.order)
                .ThenBy(m => m.orderAdded)
                .Select(m => m.middleware)
                .ToArray();

            var tokenTransformations = _tokenTransformationsByName.Values
                .OrderBy(t => t.Order)
                .ToArray();

            return new AppConfig(
                AppSettings, Console, DependencyResolver, helpProvider, NameTransformation,
                OnRunCompleted, TokenizationEvents, BuildEvents, Services,
                _parameterResolversByType, middlewarePipeline, tokenTransformations);
        }

        private class SingleRegistrationGuard<T>
        {
            private readonly string _type;
            private readonly Func<T, string> _getName;

            private readonly Dictionary<T, SingleRegistrationInfo> _registrations = 
                new Dictionary<T, SingleRegistrationInfo>();

            public SingleRegistrationGuard(string type, Func<T, string> getName)
            {
                _type = type ?? throw new ArgumentNullException(nameof(type));
                _getName = getName ?? throw new ArgumentNullException(nameof(getName));
            }

            internal void Register(T key)
            {
                if (_registrations.TryGetValue(key, out var info))
                {
                    var msg = $"{_type} '{_getName(key)}' has already been registered";
                    if (AppRunnerConfigExtensions.InUseDefaultMiddleware
                        || info.InUseDefaultMiddleware)
                    {
                        var paramName = info.ExcludeParamName 
                                        ?? AppRunnerConfigExtensions.ExcludeParamName;
                        msg += $" via 'UseDefaultMiddleware'. Try `.UseDefaultMiddleware({paramName}:true)`" +
                               " to register with other extension methods.";
                    }
                    throw new InvalidConfigurationException(msg);
                }
                _registrations.Add(key, new SingleRegistrationInfo());
            }

            private class SingleRegistrationInfo
            {
                public string? ExcludeParamName;
                public bool InUseDefaultMiddleware => !ExcludeParamName.IsNullOrWhitespace();

                public SingleRegistrationInfo()
                {
                    ExcludeParamName = AppRunnerConfigExtensions.ExcludeParamName;
                }
            }
        }
    }
}