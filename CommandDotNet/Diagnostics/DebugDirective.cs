using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Diagnostics
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
                c.UseMiddleware(AttachDebugger, MiddlewareSteps.DebugDirective);
                c.Services.Add(new DebugDirectiveContext(waitForDebuggerToAttach ?? !InTestHarness));
            });
        }

        // adapted from https://github.com/dotnet/command-line-api directives
        private static Task<int> AttachDebugger(CommandContext commandContext, ExecutionDelegate next)
        {
            if (commandContext.Tokens.HasDebugDirective())
            {
                Debugger.Attach(
                    commandContext.CancellationToken,
                    commandContext.Console,
                    commandContext.AppConfig.Services.GetOrThrow<DebugDirectiveContext>().WaitForDebuggerToAttach);
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