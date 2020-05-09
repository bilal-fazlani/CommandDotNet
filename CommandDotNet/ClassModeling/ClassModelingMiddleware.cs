using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling
{
    internal static class ClassModelingMiddleware
    {
        internal static AppRunner UseClassDefMiddleware(this AppRunner appRunner, Type rootCommandType)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(CreateRootCommand, MiddlewareSteps.CreateRootCommand);
                c.UseMiddleware(AssembleInvocationPipelineMiddleware, MiddlewareSteps.AssembleInvocationPipeline);
                c.UseMiddleware(BindValuesMiddleware.BindValues, MiddlewareSteps.BindValues);
                c.UseMiddleware(ResolveCommandClassesMiddleware.ResolveCommandClassInstances, MiddlewareSteps.ResolveCommandClasses);
                c.UseMiddleware(InvokeInvocationPipelineMiddleware, MiddlewareSteps.InvokeCommand);
                c.Services.Add(new Config(rootCommandType));
            });
        }

        private class Config
        {
            public Type RootCommandType { get; }

            public Config(Type rootCommandType)
            {
                RootCommandType = rootCommandType;
            }
        }

        private static Task<int> CreateRootCommand(CommandContext commandContext, ExecutionDelegate next)
        {
            var config = commandContext.AppConfig.Services.GetOrThrow<Config>();
            commandContext.RootCommand = ClassCommandDef.CreateRootCommand(config.RootCommandType, commandContext);
            return next(commandContext);
        }

        private static Task<int> AssembleInvocationPipelineMiddleware(CommandContext commandContext, ExecutionDelegate next)
        {
            var command = commandContext.ParseResult!.TargetCommand;
            var commandDef = command.GetCommandDef();
            if (commandDef != null)
            {
                var pipeline = commandContext.InvocationPipeline;
                if (commandDef.IsExecutable)
                {
                    pipeline.TargetCommand = new InvocationStep(command, commandDef.InvokeMethodDef!);
                    command.GetParentCommands(includeCurrent: true)
                        .Select(cmd => (cmd, def: cmd.GetCommandDef()))
                        .Where(c => c.def != null &&
                                    c.def.HasInterceptor) // in case command is defined by a different middleware
                        .Reverse()
                        .ForEach(c =>
                        {
                            pipeline.AncestorInterceptors.Add(
                                new InvocationStep(c.cmd, c.def!.InterceptorMethodDef!));
                        });
                }
            }

            return next(commandContext);
        }

        private static Task<int> InvokeInvocationPipelineMiddleware(CommandContext commandContext, ExecutionDelegate _)
        {
            static Task<int> Invoke(InvocationStep step, CommandContext context, ExecutionDelegate next, bool isCommand)
            {
                var result = step.Invocation.Invoke(context, step.Instance!, next);
                return isCommand
                    ? result.GetResultCodeAsync()
                    : (Task<int>)result;
            }

            var pipeline = commandContext.InvocationPipeline;

            return pipeline.AncestorInterceptors
                .Select(i => new ExecutionMiddleware((ctx, next) => Invoke(i, ctx, next, false)))
                .Concat(new ExecutionMiddleware((ctx, next) => Invoke(pipeline.TargetCommand!, ctx, next, true)).ToEnumerable())
                .InvokePipeline(commandContext);
        }
    }
}