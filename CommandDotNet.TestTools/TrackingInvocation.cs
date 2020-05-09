using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Execution;

namespace CommandDotNet.TestTools
{
    internal class TrackingInvocation : IInvocation
    {
        private readonly IInvocation _backingInvocation;

        public bool WasInvoked { get; private set; }
        public bool Errored => InvocationError != null;
        public Exception? InvocationError { get; private set; }

        public IReadOnlyCollection<IArgument> Arguments => _backingInvocation.Arguments;
        public IReadOnlyCollection<ParameterInfo> Parameters => _backingInvocation.Parameters;
        public object[] ParameterValues => _backingInvocation.ParameterValues;
        public MethodInfo MethodInfo => _backingInvocation.MethodInfo;

        public TrackingInvocation(IInvocation backingInvocation)
        {
            _backingInvocation = backingInvocation ?? throw new ArgumentNullException(nameof(backingInvocation));
        }

        public object Invoke(CommandContext commandContext, object instance, ExecutionDelegate next)
        {
            WasInvoked = true;
            try
            {
                return _backingInvocation.Invoke(commandContext, instance, next);
            }
            catch (Exception e)
            {
                InvocationError = e;
                throw;
            }
        }
    }
}