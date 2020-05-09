using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;
using static System.Environment;

namespace CommandDotNet.Execution
{
    /// <summary>The application configuration</summary>
    public class AppConfig : IIndentableToString
    {
        /// <summary>The application settings</summary>
        public AppSettings AppSettings { get; }

        /// <summary>
        /// The application console. Defaults from <see cref="System.Console"/>.
        /// Override in <see cref="AppRunner.Configure"/>
        /// </summary>
        public IConsole Console { get; }

        /// <summary>
        /// Services registered for the lifetime of the application.<br/>
        /// Use to store configurations for use by middleware.<br/>
        /// Add services in <see cref="AppRunner.Configure"/>
        /// </summary>
        public IServices Services { get; }

        /// <summary>
        /// The application <see cref="IDependencyResolver"/>.
        /// Set in <see cref="AppRunner.Configure"/>
        /// </summary>
        public IDependencyResolver? DependencyResolver { get; }

        /// <summary>
        /// The application <see cref="IHelpProvider"/>.
        /// Configure using <see cref="CommandDotNet.AppSettings.Help"/> or
        /// provide a customer provider using <see cref="AppRunner.Configure"/>
        /// </summary>
        public IHelpProvider HelpProvider { get; }

        internal Action<OnRunCompletedEventArgs>? OnRunCompleted { get; set; }
        internal TokenizationEvents TokenizationEvents { get; }
        internal BuildEvents BuildEvents { get; }
        internal IReadOnlyCollection<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IReadOnlyCollection<TokenTransformation> TokenTransformations { get; set; }
        internal Dictionary<Type, Func<CommandContext, object>> ParameterResolversByType { get; }
        internal NameTransformation NameTransformation { get; }
        internal ResolverService ResolverService { get; }

        public AppConfig(AppSettings appSettings, IConsole console,
            IDependencyResolver? dependencyResolver, IHelpProvider helpProvider,
            NameTransformation? nameTransformation, Action<OnRunCompletedEventArgs>? onRunCompleted,
            TokenizationEvents tokenizationEvents, BuildEvents buildEvents, IServices services,
            Dictionary<Type, Func<CommandContext, object>> parameterResolversByType,
            ExecutionMiddleware[] middlewarePipeline, TokenTransformation[] tokenTransformations)
        {
            AppSettings = appSettings;
            Console = console;
            DependencyResolver = dependencyResolver;
            HelpProvider = helpProvider;
            NameTransformation = nameTransformation ?? ((attributes, memberName, overrideName, commandNodeType) => overrideName ?? memberName);
            OnRunCompleted = onRunCompleted;
            TokenizationEvents = tokenizationEvents;
            BuildEvents = buildEvents;
            Services = services;
            ParameterResolversByType = parameterResolversByType;
            MiddlewarePipeline = middlewarePipeline;
            TokenTransformations = tokenTransformations;

            ResolverService = services.GetOrAdd(() => new ResolverService());
            ResolverService.BackingResolver = dependencyResolver;
            OnRunCompleted += args => ResolverService.OnRunCompleted(args.CommandContext);
        }

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            indent = indent.Increment();
            var indent2 = indent.Increment();

            var tokenTransformations = TokenTransformations
                .OrderBy(t => t.Order)
                .Select(t => $"{indent2}{t.Name}({t.Order})")
                .ToCsv(NewLine);

            var middleware = MiddlewarePipeline
                .Select(m => $"{indent2}{m.Method.FullName()}")
                .ToCsv(NewLine);

            var paramResolvers = ParameterResolversByType.Keys
                .Select(k => $"{indent2}{k}")
                .ToCsv(NewLine);

            return $"{nameof(AppConfig)}:{NewLine}" +
                   $"{indent}{AppSettings.ToString(indent.Increment())}{NewLine}" +
                   $"{indent}DependencyResolver: {DependencyResolver}{NewLine}" +
                   $"{indent}HelpProvider: {HelpProvider}{NewLine}" +
                   $"{indent}TokenTransformations:{NewLine}{tokenTransformations}{NewLine}" +
                   $"{indent}MiddlewarePipeline:{NewLine}{middleware}{NewLine}" +
                   $"{indent}ParameterResolvers:{NewLine}{paramResolvers}";
        }
    }
}