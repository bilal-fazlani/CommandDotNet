using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class MethodDef : IMethodDef
    {
        public static readonly Type MiddlewareNextParameterType = typeof(ExecutionDelegate);
        public static readonly Type InterceptorNextParameterType = typeof(InterceptorExecutionDelegate);

        private readonly AppConfig _appConfig;
        private Result? _result;
        private IReadOnlyCollection<IArgument>? _arguments;

        private Result GetResult() => _result ??= new Result(MethodInfo, _appConfig);

        public MethodInfo MethodInfo { get; }

        public IReadOnlyCollection<IArgumentDef> ArgumentDefs => GetResult().ArgumentDefs;

        public IReadOnlyCollection<IArgument> Arguments => _arguments
            ??= ArgumentDefs
                .Where(a => !Equals(a.Argument, null))
                .Select(a => a.Argument!)
                .ToReadOnlyCollection();

        public IReadOnlyCollection<ParameterInfo> Parameters => GetResult().Parameters;

        public object[] ParameterValues => GetResult().Values;

        public MethodDef(MethodInfo method, AppConfig appConfig)
        {
            MethodInfo = method ?? throw new ArgumentNullException(nameof(method));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        public static bool IsInterceptorMethod(MethodBase methodBase)
        {
            return methodBase.GetParameters().Any(IsExecutionDelegate);
        }

        private static bool IsExecutionDelegate(ParameterInfo parameterInfo)
        {
            return parameterInfo.ParameterType == MiddlewareNextParameterType
                   || parameterInfo.ParameterType == InterceptorNextParameterType;
        }

        public object Invoke(CommandContext commandContext, object instance, ExecutionDelegate next)
        {
            return GetResult().Invoke(commandContext, instance, next);
        }

        public override string ToString()
        {
            return $"{MethodInfo.GetType().Name}:{MethodInfo.DeclaringType?.Name}.{MethodInfo.Name}(" +
                   $"{MethodInfo.GetParameters().Select(p => $"{p.ParameterType} {p.Name}").ToCsv()})";
        }

        private class Result
        {
            private readonly MethodInfo _methodInfo;
            private readonly AppConfig _appConfig;

            private ParameterInfo? _nextParameterInfo;
            private readonly List<Action<CommandContext>> _resolvers = new List<Action<CommandContext>>();

            internal IReadOnlyCollection<IArgumentDef> ArgumentDefs;
            internal ParameterInfo[] Parameters;
            internal object[] Values;

            public Result(MethodInfo methodInfo, AppConfig appConfig)
            {
                _appConfig = appConfig;
                _methodInfo = methodInfo;

                Parameters = _methodInfo.GetParameters();

                var isMiddleware = Parameters.Any(IsExecutionDelegate);

                var argumentMode = isMiddleware
                    ? ArgumentMode.Option
                    : _appConfig.AppSettings.DefaultArgumentMode;

                Values = new object[Parameters.Length];

                var parametersByName = Parameters.ToDictionary(
                    p => p.Name,
                    p => (param: p, args: GetArgsFromParameter(p, argumentMode).ToCollection()));

                var arguments = parametersByName.Values
                    .OrderBy(v => v.param.Position)
                    .SelectMany(p => p.args)
                    .ToReadOnlyCollection();

                ArgumentDefs = arguments;
            }

            public object Invoke(CommandContext commandContext, object instance, ExecutionDelegate next)
            {
                if (_nextParameterInfo != null)
                {
                    if (next == null)
                    {
                        throw new AppRunnerException(
                            $"Invalid operation. {nameof(ExecutionDelegate)} {_nextParameterInfo.Name} parameter not provided for method: {_nextParameterInfo.Member.FullName()}. " +
                            $"Check middleware to ensure it hasn't misconfigured the {nameof(CommandContext.InvocationPipeline)}");
                    }

                    if (_nextParameterInfo.ParameterType == InterceptorNextParameterType)
                    {
                        var nextLite = new InterceptorExecutionDelegate(() => next(commandContext));
                        Values[_nextParameterInfo.Position] = nextLite;
                    }
                    else
                    {
                        Values[_nextParameterInfo.Position] = next;
                    }
                }

                _resolvers?.ForEach(r => r(commandContext));

                return _methodInfo.Invoke(instance, Values);
            }

            private IEnumerable<IArgumentDef> GetArgsFromParameter(ParameterInfo parameterInfo, ArgumentMode argumentMode)
            {
                if (parameterInfo.ParameterType.InheritsFrom<IArgumentModel>())
                {
                    return GetArgumentsFromModel(
                        parameterInfo.ParameterType,
                        argumentMode,
                        null,
                        value => Values[parameterInfo.Position] = value);
                }

                if (_appConfig.ParameterResolversByType.TryGetValue(parameterInfo.ParameterType, out var resolve))
                {
                    _resolvers.Add(context => Values[parameterInfo.Position] = resolve(context));
                    return Enumerable.Empty<IArgumentDef>();
                }

                if (IsExecutionDelegate(parameterInfo))
                {
                    _nextParameterInfo = parameterInfo;
                    return Enumerable.Empty<IArgumentDef>();
                }

                return new ParameterArgumentDef(
                        parameterInfo,
                        GetArgumentType(parameterInfo, argumentMode),
                        _appConfig,
                        Values)
                    .ToEnumerable();
            }

            private IEnumerable<IArgumentDef> GetArgsFromProperty(PropertyData propertyData, object modelInstance, ArgumentMode argumentMode)
            {
                var propertyInfo = propertyData.PropertyInfo;
                return propertyData.IsArgModel
                    ? GetArgumentsFromModel(
                        propertyInfo.PropertyType,
                        argumentMode,
                        propertyInfo.GetValue(modelInstance),
                        value => propertyInfo.SetValue(modelInstance, value), propertyData)
                    : new PropertyArgumentDef(
                            propertyInfo,
                            GetArgumentType(propertyInfo, argumentMode),
                            _appConfig,
                            modelInstance)
                        .ToEnumerable();
            }

            private IEnumerable<IArgumentDef> GetArgumentsFromModel(Type modelType, ArgumentMode argumentMode,
                object? existingDefault, Action<object> instanceCreated, PropertyData? parentProperty = null)
            {
                var instance = existingDefault ?? _appConfig.ResolverService.ResolveArgumentModel(modelType);

                if (existingDefault == null)
                {
                    instanceCreated?.Invoke(instance);
                }

                return modelType
                    .GetDeclaredProperties()
                    .Select((p, i) => new PropertyData(
                        p, i,
                        parentProperty,
                        GetArgumentType(p, argumentMode)))
                    .OrderBy(pd => pd.LineNumber.GetValueOrDefault(int.MaxValue))
                    .ThenBy(pd => pd.PropertyIndex) //use reflected order for options since order can be inconsistent
                    .SelectMany(pd => GetArgsFromProperty(pd, instance, argumentMode));
            }

            private static CommandNodeType GetArgumentType(ICustomAttributeProvider info, ArgumentMode argumentMode)
            {
                return info.IsOption(argumentMode) ? CommandNodeType.Option : CommandNodeType.Operand;
            }

            private class PropertyData
            {
                private readonly PropertyData? _parentProperty;

                public PropertyInfo PropertyInfo { get; }
                public bool IsArgModel { get; }
                public int? LineNumber { get; }
                public int PropertyIndex { get; }

                public PropertyData(PropertyInfo propertyInfo, int propertyIndex, PropertyData? parentProperty, CommandNodeType commandNode)
                {
                    _parentProperty = parentProperty;
                    PropertyInfo = propertyInfo;
                    PropertyIndex = propertyIndex;
                    IsArgModel = propertyInfo.PropertyType.InheritsFrom<IArgumentModel>();
                    LineNumber = propertyInfo.GetCustomAttribute<OperandAttribute>()?.CallerLineNumber
                                 ?? propertyInfo.GetCustomAttribute<OptionAttribute>()?.CallerLineNumber
                                 ?? propertyInfo.GetCustomAttribute<OrderByPositionInClassAttribute>()?.CallerLineNumber;

                    var isOperand = !IsArgModel && commandNode == CommandNodeType.Operand;

                    if (isOperand)
                    {
                        if (!LineNumber.HasValue)
                        {
                            throw new InvalidConfigurationException("Operand property must be attributed with " +
                                                                    $"{nameof(OperandAttribute)} or {nameof(OrderByPositionInClassAttribute)} to guarantee consistent order. " +
                                                                    $"Property: {propertyInfo.DeclaringType?.FullName}.{propertyInfo.Name}");
                        }

                        var parentsWithoutLineNumber = ParentsWithoutLineNumber().ToList();
                        if (parentsWithoutLineNumber.Any())
                        {
                            var props = parentsWithoutLineNumber
                                .Select(p => p.PropertyInfo)
                                .Select(p => $"  {p.DeclaringType?.FullName}.{p.Name}")
                                .ToCsv(Environment.NewLine);
                            throw new InvalidConfigurationException("Operand property must be attributed with " +
                                                                    $"{nameof(OperandAttribute)} or {nameof(OrderByPositionInClassAttribute)} to guarantee consistent order. " +
                                                                    $"Properties:{Environment.NewLine}{props}");
                        }
                    }
                }

                private IEnumerable<PropertyData> ParentsWithoutLineNumber()
                {
                    for (var p = _parentProperty; p != null; p = p._parentProperty)
                    {
                        if (!p.LineNumber.HasValue)
                        {
                            yield return p;
                        }
                    }
                }
            }
        }
    }
}