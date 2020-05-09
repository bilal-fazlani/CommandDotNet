using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using CommandDotNet.Extensions;
using CommandDotNet.Rendering;

namespace CommandDotNet.Execution
{
    public static class CancellationHandlers
    {
        // using a stack to handle interactive sessions
        // so ctrl+c for a long running interactive command
        // stops only that command and not the interactive session
        private static readonly Stack<CommandContext> SourceStack = new Stack<CommandContext>();

        private class Handler
        {
            public CancellationTokenSource Source { get; }
            public bool ShouldIgnoreCtrlC { get; set; }

            public Handler(CancellationTokenSource source)
            {
                Source = source ?? throw new ArgumentNullException(nameof(source));
            }
        }

        private static Handler? GetHandler(this CommandContext ctx) => ctx.Services.GetOrDefault<Handler>();
        private static void SetHandler(this CommandContext ctx, CancellationTokenSource src) => ctx.Services.Add(new Handler(src));

        static CancellationHandlers()
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        internal static void BeginRun(CommandContext ctx)
        {
            var tokenSource = new CancellationTokenSource();
            ctx.CancellationToken = tokenSource.Token;
            ctx.SetHandler(tokenSource);
            SourceStack.Push(ctx);
        }

        internal static void EndRun()
        {
            SourceStack.Pop();
            if (!SourceStack.Any())
            {
                // the app will exit now
                // clean up for tests
                Console.CancelKeyPress -= Console_CancelKeyPress;
                AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            }
        }

        /// <summary>
        /// Prefer <see cref="IConsole.TreatControlCAsInput"/> when possible.
        /// Use this in cases where another component depends on the <see cref="Console.CancelKeyPress"/>
        /// event and CommandDotNet should ignore the event during this time. 
        /// </summary>
        public static IDisposable IgnoreCtrlC()
        {
            // in case the feature isn't enabled but this is called.
            if (!SourceStack.Any())
            {
                return DisposableAction.Null;
            }

            var handler = SourceStack.Peek().GetHandler()!;
            handler.ShouldIgnoreCtrlC = true;
            return new DisposableAction(() => handler.ShouldIgnoreCtrlC = false);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            var handler = SourceStack.Peek()?.GetHandler();
            if (handler == null)
            {
                return;
            }
            if (!handler.ShouldIgnoreCtrlC)
            {
                StopRun(handler);
                e.Cancel = true;
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Shutdown();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                Shutdown();
            }
        }

        private static void StopRun(Handler? handler)
        {
            if (handler == null)
            {
                // this is an edge case race condition.
                return;
            }
            var src = handler.Source;
            if (!src.IsCancellationRequested)
            {
                src.Cancel();
            }
        }

        private static void Shutdown()
        {
            SourceStack.ForEach(ctx => StopRun(ctx.GetHandler()));
        }

        internal static class TestAccess
        {
            internal static ConsoleCancelEventArgs Console_CancelKeyPress()
            {
                var args = Activate<ConsoleCancelEventArgs>(ConsoleSpecialKey.ControlC);
                CancellationHandlers.Console_CancelKeyPress(new object(), args);
                return args;
            }

            internal static void CurrentDomain_ProcessExit() =>
                CancellationHandlers.CurrentDomain_ProcessExit(new object(), EventArgs.Empty);

            internal static UnhandledExceptionEventArgs CurrentDomain_UnhandledException(bool isTerminating)
            {
                var ex = new Exception("some random exception");
                var args = Activate<UnhandledExceptionEventArgs>(ex, isTerminating);
                CancellationHandlers.CurrentDomain_UnhandledException(new object(), args);
                return args;
            }

            private static T Activate<T>(params object[] parameters)
            {
                var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance;
                var parameterTypes = parameters.Select(p => p.GetType()).ToArray();
                var constructorInfo = typeof(T).GetConstructor(bindingFlags, null, parameterTypes, null);
                return (T)constructorInfo.Invoke(parameters);
            }
        }
    }
}