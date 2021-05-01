using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandDotNet.ConsoleOnly;
using CommandDotNet.Directives;
using CommandDotNet.Execution;

namespace CommandDotNet.Diagnostics
{
    internal static class CommandLoggerMiddleware
    {
        internal static AppRunner UseCommandLogger(AppRunner appRunner,
            Func<CommandContext, Action<string?>?>? writerFactory,
            bool includeSystemInfo,
            bool includeAppConfig,
            Func<CommandContext, IEnumerable<(string key, string value)>?>? additionalInfoCallback)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(CommandLogger, MiddlewareSteps.CommandLogger);
                c.Services.Add(new CommandLoggerConfig(
                    writerFactory,
                    includeSystemInfo,
                    includeAppConfig,
                    additionalInfoCallback));
            });
        }

        private static Action<string?>? DefaultIfDirectiveRequest(CommandContext ctx)
        {
            return ctx.Tokens.TryGetDirective("cmdlog", out _) 
                ? s => ctx.Console.Out.WriteLine(s)
                : (Action<string?>?)null;
        }

        private static Task<int> CommandLogger(CommandContext commandContext, ExecutionDelegate next)
        {
            var config = commandContext.AppConfig.Services.GetOrThrow<CommandLoggerConfig>();
            Action<string?>? writer = (config.WriterFactory ?? DefaultIfDirectiveRequest)(commandContext);
            if (writer != null)
            {
                Diagnostics.CommandLogger.Log(
                    commandContext, 
                    writer, 
                    config.IncludeSystemInfo, 
                    config.IncludeAppConfig);
            }

            return next(commandContext);
        }
    }
}
