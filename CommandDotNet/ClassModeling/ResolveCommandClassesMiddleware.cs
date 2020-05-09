using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling
{
    internal static class ResolveCommandClassesMiddleware
    {
        internal static Task<int> ResolveCommandClassInstances(CommandContext commandContext, ExecutionDelegate next)
        {
            var pipeline = commandContext.InvocationPipeline;

            InvocationStep? parentStep = null;
            foreach (var ic in pipeline.All)
            {
                if (parentStep != null 
                    && parentStep.Invocation.MethodInfo.DeclaringType == ic.Invocation.MethodInfo.DeclaringType)
                {
                    // this is true when the interceptor method is in the same class as the command method 
                    ic.Instance = parentStep.Instance;
                    continue;
                }
                ic.Instance = GetInstance(ic, parentStep, commandContext);
                parentStep = ic;
            }

            return next(commandContext);
        }

        private static object? GetInstance(
            InvocationStep invocationStep,
            InvocationStep? parentStep,
            CommandContext commandContext)
        {
            var command = invocationStep.Command;
            var commandDef = command.GetCommandDef();
            if (commandDef == null)
            {
                return null;
            }

            var classType = commandDef.CommandHostClassType;
            if (classType == null)
            {
                return null;
            }

            var instance = commandContext.AppConfig.ResolverService.ResolveCommandClass(classType, commandContext);
            if (instance != null)
            {
                SetInstanceOnParent(parentStep, classType, instance);
            }
            return instance;
        }

        private static void SetInstanceOnParent(InvocationStep? parentStep, Type classType, object instance)
        {
            if (parentStep == null)
            {
                return;
            }

            var parent = parentStep.Instance!;
            parent.GetType().GetProperties()
                .Where(p =>
                    p.CanWrite
                    && p.PropertyType == classType
                    && p.HasAttribute<SubCommandAttribute>()
                    && p.GetValue(parent) == null)
                .ForEach(p => p.SetValue(parent, instance));
        }
    }
}