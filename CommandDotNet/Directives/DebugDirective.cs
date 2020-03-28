using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Directives
{
    internal static class DebugDirective
    {
        // this is obviously not the ideal design.  major code smell.
        // but... it meets our needs simply until we have a way
        // to call UseDebugDirective from the tests.
        internal static bool InTestHarness { private get; set; }

        internal static AppRunner UseDebugDirective(this AppRunner appRunner, bool? waitForDebuggerToAttach = null)
        {
            return appRunner.Configure(c =>
            {
                c.Services.Add(new DebugDirectiveContext(waitForDebuggerToAttach ?? !InTestHarness));
                c.UseMiddleware(AttachDebugger, MiddlewareSteps.DebugDirective.Stage, MiddlewareSteps.DebugDirective.Order);
            });
        }

        // adapted from https://github.com/dotnet/command-line-api directives
        private static Task<int> AttachDebugger(CommandContext commandContext, ExecutionDelegate next)
        {
            if (commandContext.Tokens.HasDebugDirective())
            {
                Debugger.Attach(
                    commandContext.AppConfig.CancellationToken,
                    commandContext.Console,
                    commandContext.AppConfig.Services.Get<DebugDirectiveContext>().WaitForDebuggerToAttach);
            }

            return next(commandContext);
        }

        private class DebugDirectiveContext
        {
            public bool WaitForDebuggerToAttach { get; }

            public DebugDirectiveContext(bool waitForDebuggerToAttach)
            {
                WaitForDebuggerToAttach = waitForDebuggerToAttach;
            }
        }
    }
}