using System;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.Parsing;

namespace CommandDotNet.TestTools
{

    public static class CommandContextTestExtensions
    {
        /// <summary>
        /// Returns an <see cref="InvocationInfo"/> for the <see cref="ParseResult.TargetCommand"/>
        /// </summary>
        public static InvocationInfo GetCommandInvocationInfo(this CommandContext ctx)
        {
            var step = ctx.InvocationPipeline?.TargetCommand;
            if (step is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CommandContext.InvocationPipeline)} or {nameof(InvocationPipeline.TargetCommand)} was null. " +
                    "The middleware pipeline exited early.");
            }
            return new InvocationInfo(ctx, step!);
        }

        /// <summary>
        /// Returns a generic typed <see cref="InvocationInfo"/> for the <see cref="ParseResult.TargetCommand"/><br/>
        /// Throws an exception if the type does not host the <see cref="ParseResult.TargetCommand"/>.
        /// </summary>
        public static InvocationInfo<TCommandClass> GetCommandInvocationInfo<TCommandClass>(this CommandContext ctx) where TCommandClass: class
        {
            var step = ctx.InvocationPipeline?.TargetCommand;
            if (step is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CommandContext.InvocationPipeline)} or {nameof(InvocationPipeline.TargetCommand)} was null. " +
                    "The middleware pipeline exited early.");
            }
            if (!(step!.Instance is TCommandClass))
            {
                throw new InvalidOperationException(
                    $"the {nameof(ctx.InvocationPipeline.TargetCommand)} is not hosted by " +
                    $"{step.Instance!.GetType()} and cannot be cast to {typeof(TCommandClass)}");
            }
            return new InvocationInfo<TCommandClass>(ctx, step);
        }

        /// <summary>
        /// Returns a generic typed <see cref="InvocationInfo"/> for the interceptor defined in the <see cref="TInterceptorClass"/><br/>
        /// Throws an exception if the interceptor was not in the hierarchy of the <see cref="ParseResult.TargetCommand"/>.
        /// </summary>
        public static InvocationInfo<TInterceptorClass> GetInterceptorInvocationInfo<TInterceptorClass>(this CommandContext ctx) where TInterceptorClass : class
        {
            var step = ctx.InvocationPipeline?.AncestorInterceptors?
                .FirstOrDefault(i => i.Invocation.MethodInfo.DeclaringType == typeof(TInterceptorClass));
            if (step is null)
            {
                if (ctx.InvocationPipeline?.AncestorInterceptors is null)
                {
                    throw new InvalidOperationException(
                        $"{nameof(CommandContext.InvocationPipeline)} or {nameof(InvocationPipeline.AncestorInterceptors)} was null. " +
                        "The middleware pipeline exited early");
                }

                throw new InvalidOperationException(
                    $"The interceptor for {typeof(TInterceptorClass)} is not in the hierarchy " +
                    $"of the executed command {ctx.InvocationPipeline.TargetCommand!.Command}");
            }
            return new InvocationInfo<TInterceptorClass>(ctx, step);
        }

        /// <summary>Return true if the interceptor was in the hierarchy of the <see cref="ParseResult.TargetCommand"/></summary>
        public static bool IsIntercepting<TInterceptorClass>(this CommandContext ctx) where TInterceptorClass : class
        {
            var step = ctx.InvocationPipeline?.All?
                .FirstOrDefault(i => i.Invocation.MethodInfo.DeclaringType == typeof(TInterceptorClass));
            return step != null;
        }
    }
}