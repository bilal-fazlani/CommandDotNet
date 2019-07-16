﻿using System.Collections.Generic;
using CommandDotNet.Builders;
using CommandDotNet.Parsing;

namespace CommandDotNet.Execution
{
    public class ExecutionConfig
    {
        public AppSettings AppSettings { get; }
        public IDependencyResolver DependencyResolver { get; }

        public ParseEvents ParseEvents { get; }
        public BuildEvents BuildEvents { get; }
        public IContextData ContextData { get; }

        internal IReadOnlyCollection<ExecutionMiddleware> MiddlewarePipeline { get; set; }
        internal IReadOnlyCollection<InputTransformation> InputTransformations { get; set; }

        public ExecutionConfig(AppSettings appSettings, IDependencyResolver dependencyResolver,
            ParseEvents parseEvents, BuildEvents buildEvents, IContextData contextData)
        {
            AppSettings = appSettings;
            DependencyResolver = dependencyResolver;
            ParseEvents = parseEvents;
            BuildEvents = buildEvents;
            ContextData = contextData;
        }
    }
}