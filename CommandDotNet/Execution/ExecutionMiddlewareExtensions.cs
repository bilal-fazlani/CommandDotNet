using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Logging;

namespace CommandDotNet.Execution
{
    internal static class ExecutionMiddlewareExtensions
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        internal static Task<int> InvokePipeline(this IEnumerable<ExecutionMiddleware> pipeline, CommandContext commandContext)
        {
            var middlewareChain = pipeline.Aggregate(
                (first, second) =>
                    (ctx, next) =>
                        first(ctx, c =>
                        {
                            if (c.CancellationToken.IsCancellationRequested)
                            {
                                Log.Info("Cancellation requested. Aborting execution pipeline");
                                return ExitCodes.Success;
                            }
                            else
                            {
                                Log.Info($"begin: invoke middleware: {second.Method.Name}");
                                var task = second(c, next);
                                Log.Info($"end: invoke middleware: {second.Method.Name}");
                                return task;
                            }
                        }));

            return middlewareChain(commandContext, ctx => ExitCodes.Success);
        }
    }
}