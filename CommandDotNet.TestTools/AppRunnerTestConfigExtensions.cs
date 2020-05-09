using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools
{
    public static class AppRunnerTestConfigExtensions
    {
        public static AppRunner TrackingInvocations(this AppRunner runner)
        {
            return runner.Configure(c => c.UseMiddleware(TrackingInvocationMiddleware, 
                new MiddlewareStep(MiddlewareStages.PostBindValuesPreInvoke, short.MaxValue)));
        }

        internal static Task<int> TrackingInvocationMiddleware(CommandContext context, ExecutionDelegate next)
        {
            context.InvocationPipeline.All.ForEach(step => step.Invocation = new TrackingInvocation(step.Invocation));
            return next(context);
        }
    }
}