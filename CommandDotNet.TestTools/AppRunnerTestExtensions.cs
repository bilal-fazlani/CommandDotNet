using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Diagnostics.Parse;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.TestTools.Prompts;
using static System.Environment;

namespace CommandDotNet.TestTools
{
    /// <summary>Run the console in memory and get the results that would be output to the shell</summary>
    public static class AppRunnerTestExtensions
    {
        /// <summary>
        /// Injects a middleware to capture state at specific point. <br/>
        /// This method does not prevent referenced objects from being mutated after capture. <br/>
        /// It is the responsibility of capture action to clone data to prevent mutation.
        /// </summary>
        public static AppRunner CaptureState(this AppRunner runner, Action<CommandContext> capture,
            MiddlewareStages middlewareStage, short? orderWithinStage = null, bool exitAfterCapture = false)
        {
            return runner.Configure(b => b.UseMiddleware((context, next) =>
            {
                capture(context);
                return exitAfterCapture 
                    ? ExitCodes.Success
                    : next(context);
            }, middlewareStage, orderWithinStage));
        }

        /// <summary>
        /// Convenience wrapper for <see cref="CaptureState"/> to capture state from the <see cref="CommandContext"/>
        /// </summary>
        public static T? GetFromContext<T>(this AppRunner runner,
            string[] args,
            Func<CommandContext, T> capture,
            Action<string?>? logLine = null,
            MiddlewareStages middlewareStage = MiddlewareStages.PostBindValuesPreInvoke,
            short? orderWithinStage = null)
            where T : class
        {
            T? state = default;
            runner.CaptureState(
                    ctx => state = capture(ctx),
                    exitAfterCapture: true,
                    middlewareStage: middlewareStage,
                    orderWithinStage: orderWithinStage)
                .RunInMem(args, logLine, config: TestConfig.Silent);
            return state;
        }

        /// <summary>Run the console in memory and get the results that would be output to the shell</summary>
        public static AppRunnerResult RunInMem(this AppRunner runner,
            string args,
            Action<string?>? logLine = null,
            Func<TestConsole, string>? onReadLine = null,
            IEnumerable<string>? pipedInput = null,
            IPromptResponder? promptResponder = null,
            TestConfig? config = null)
        {
            return runner.RunInMem(args.SplitArgs(), logLine, onReadLine, pipedInput, promptResponder, config);
        }

        /// <summary>Run the console in memory and get the results that would be output to the shell</summary>
        public static AppRunnerResult RunInMem(this AppRunner runner,
            string[] args,
            Action<string?>? logLine = null,
            Func<TestConsole, string>? onReadLine = null,
            IEnumerable<string>? pipedInput = null,
            IPromptResponder? promptResponder = null,
            TestConfig? config = null)
        {
            logLine ??= Console.WriteLine;
            config ??= TestConfig.Default;

            IDisposable appInfo = config.AppInfoOverride is null
                ? new DisposableAction(() => { })
                : AppInfo.OverrideInstance(config.AppInfoOverride);

            IDisposable logProvider = config.PrintCommandDotNetLogs
                ? TestToolsLogProvider.InitLogProvider(logLine)
                : new DisposableAction(() => { });

            using (appInfo)
            using (logProvider)
            {
                var testConsole = new TestConsole(
                    onReadLine,
                    pipedInput,
                    promptResponder is null
                        ? (Func<TestConsole, ConsoleKeyInfo>?) null
                        : promptResponder.OnReadKey);
                runner.Configure(c => c.Console = testConsole);

                CommandContext? context = null;
                Task<int> CaptureCommandContext(CommandContext commandContext, ExecutionDelegate next)
                {
                    context = commandContext;
                    return next(commandContext);
                }
                runner.Configure(c => c.UseMiddleware(CaptureCommandContext, MiddlewareSteps.DebugDirective - 100));

                try
                {
                    var exitCode = runner.Run(args);
                    return new AppRunnerResult(exitCode, runner, context!, testConsole, config)
                        .LogResult(logLine);
                }
                catch (Exception e)
                {
                    var result = new AppRunnerResult(1, runner, context!, testConsole, config, e);
                    if (config.OnError.CaptureAndReturnResult)
                    {
                        testConsole.Error.WriteLine(e.Message);
                        logLine(e.Message);
                        logLine(e.StackTrace);
                        return result.LogResult(logLine, onError: true);
                    }

                    result.LogResult(logLine, onError: true);
                    throw;
                }
            }
        }

        internal static AppRunnerResult LogResult(this AppRunnerResult result, Action<string?> logLine, bool onError = false)
        {
            var print = onError || result.EscapedException != null 
                ? result.Config.OnError.Print 
                : result.Config.OnSuccess.Print;

            if (print.ConsoleOutput)
            {
                var consoleAll = result.Console.AllText();
                logLine($"{NewLine}Console output <begin> ------------------------------");
                logLine(consoleAll.IsNullOrWhitespace() ? "<no output>" : consoleAll);
                logLine($"Console output <end> ------------------------------{NewLine}");
            }

            var context = result.CommandContext;
            if (print.CommandContext)
            {
                logLine("");
                logLine(context?.ToString(new Indent(), includeOriginalArgs: true));
            }

            if (print.ParseReport && context?.ParseResult != null)
            {
                logLine("");
                logLine($"{NewLine}Parse report <begin> ------------------------------");
                ParseReporter.Report(context, writeln: logLine);
                logLine($"Parse report <end> ------------------------------{NewLine}");
            }

            if (print.AppConfig)
            {
                logLine("");
                logLine(result.Runner.ToString());
            }

            return result;
        }
    }
}