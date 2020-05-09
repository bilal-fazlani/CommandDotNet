using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Rendering;

namespace CommandDotNet.TestTools
{
    public class InvocationInfo<T> : InvocationInfo where T: class
    {
        public new T? Instance => (T?)base.Instance;

        public InvocationInfo(CommandContext commandContext, InvocationStep invocationStep)
            : base(commandContext, invocationStep)
        {
        }
    }

    public class InvocationInfo
    {
        private readonly CommandContext _commandContext;
        private readonly InvocationStep _invocationStep;

        public object? Instance => _invocationStep?.Instance;
        public Command? Command => _invocationStep?.Command;
        public IInvocation? Invocation => _invocationStep?.Invocation;

        public IReadOnlyCollection<IArgument>? Arguments => Invocation?.Arguments;
        public IReadOnlyCollection<ParameterInfo>? Parameters => Invocation?.Parameters;
        public object?[]? ParameterValues => Invocation?.ParameterValues;
        public MethodInfo? MethodInfo => Invocation?.MethodInfo;

        /// <summary>Returns the <see cref="ParameterInfo"/> for parameters that define arguments</summary>
        public IEnumerable<ParameterInfo>? ArgumentParameters =>
            Parameters?.Where(p => IsArgumentParameter(p, _commandContext));

        /// <summary>
        /// Returns the <see cref="ParameterInfo"/> for parameters that DO NOT define arguments.
        /// For example, <see cref="InterceptorExecutionDelegate"/>, <see cref="CommandContext"/>,
        /// <see cref="IConsole"/> and any other type configured as a parameter resolver.
        /// </summary>
        public IEnumerable<ParameterInfo>? NonArgumentParameters =>
            Parameters?.Where(p => !IsArgumentParameter(p, _commandContext));

        /// <summary>Returns the <see cref="ParameterValues"/> for parameters that define arguments</summary>
        public IEnumerable<object?>? ArgumentParameterValues =>
            ArgumentParameters?.Select(p => ParameterValues?[p.Position]);

        /// <summary>
        /// Returns the <see cref="ParameterValues"/> for parameters that DO NOT define arguments.
        /// For example, <see cref="InterceptorExecutionDelegate"/>, <see cref="CommandContext"/>,
        /// <see cref="IConsole"/> and any other type configured as a parameter resolver.
        /// </summary>
        public IEnumerable<object?>? NonArgumentParameterValues =>
            NonArgumentParameters?.Select(p => ParameterValues?[p.Position]);

        /// <summary>
        /// Returns true if the <see cref="MethodInfo"/> was invoked.<br/>
        /// <see cref="AppRunnerTestConfigExtensions.TrackingInvocations"/>
        /// must have been configured for the <see cref="AppRunner"/>.
        /// </summary>
        public bool WasInvoked =>
            CastInvocation<TrackingInvocation>(Invocation, _commandContext)?.WasInvoked ?? false;

        public InvocationInfo(CommandContext commandContext, InvocationStep invocationStep)
        {

            _commandContext = commandContext;
            _invocationStep = invocationStep ?? throw new ArgumentNullException(nameof(invocationStep));
        }

        public object Invoke(CommandContext commandContext, object instance, ExecutionDelegate next)
        {
            throw new InvalidOperationException($"{nameof(InvocationInfo)} is readonly");
        }

        private static bool IsArgumentParameter(ParameterInfo info, CommandContext ctx)
        {
            return info.ParameterType != typeof(InterceptorExecutionDelegate)
                   && info.ParameterType != typeof(ExecutionDelegate)
                   && !ctx.AppConfig.ParameterResolversByType.ContainsKey(info.ParameterType);
        }

        private static TInvocation? CastInvocation<TInvocation>(IInvocation? invocation, CommandContext ctx) 
            where TInvocation : class, IInvocation
        {
            try
            {
                return (TInvocation?)invocation;
            }
            catch (InvalidCastException e)
            {
                if (typeof(TInvocation) == typeof(TrackingInvocation))
                {
                    var isTrackingConfigured = ctx.AppConfig.MiddlewarePipeline.Any(p =>
                        p.Method.Name == nameof(AppRunnerTestConfigExtensions.TrackingInvocationMiddleware));

                    if (isTrackingConfigured)
                    {
                        throw new InvalidCastException(
                            $"{e.Message}. " +
                            $"The {nameof(AppRunner)} is configured with " +
                            $"'appRunner.{nameof(AppRunnerTestConfigExtensions.TrackingInvocations)}()'. " +
                            "Is another middleware wrapping or replacing the assignment?", e);
                    }

                    throw new InvalidCastException(
                        $"{e.Message}. " +
                        $"Configure the {nameof(AppRunner)} with " +
                        $"'appRunner.{nameof(AppRunnerTestConfigExtensions.TrackingInvocations)}()'", e);
                }

                throw;
            }
        }
    }
}