using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.ConsoleOnly;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Logging;

namespace CommandDotNet.Example
{
    internal static class Log2ConsoleDirective
    {
        /// <summary>
        /// Output internal CommandDotNet logs to the console when user specifies 'log' or 'log:{level}'.<br/>
        /// FYI: there aren't many logs at this time.
        /// </summary>
        internal static AppRunner UseLog2ConsoleDirective(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
                c.UseMiddleware(LogToConsole, new MiddlewareStep(MiddlewareStages.PreTokenize, short.MinValue)));
        }

        private static Task<int> LogToConsole(CommandContext context, ExecutionDelegate next)
        {
            if(context.Tokens.TryGetDirective("log", out var logDirective))
            {
                var parts = logDirective!.Split(':', '=');
                var level = parts.Length > 1
                    ? (LogLevel) Enum.Parse(typeof(LogLevel), parts[1], ignoreCase: true) 
                    : LogLevel.Trace;
                var dateTimeFormat = GetDateTimeFormat(parts);
                
                LogProvider.IsDisabled = false;
                LogProvider.SetCurrentLogProvider(new ConsoleLogProvider(context.Console, level, dateTimeFormat));
            }

            return next(context);
        }

        private static string? GetDateTimeFormat(string[] parts)
        {
            if (parts.Length < 3)
            {
                return null;
            }
            switch (parts[2].ToLower())
            {
                case "date":
                    return "yyyy/MM/dd";
                case "time":
                    return "HH:mm:ss";
                case "datetime":
                    return "yyyy/MM/dd HH:mm:ss";
                default:
                    return null;
            }
        }

        private class ConsoleLogProvider : ILogProvider
        {
            private readonly IConsole _console;
            private readonly LogLevel _level;
            private readonly string? _dateTimeFormat;

            public ConsoleLogProvider(IConsole console, LogLevel level, string? dateTimeFormat)
            {
                _console = console ?? throw new ArgumentNullException(nameof(console));
                _level = level;
                _dateTimeFormat = dateTimeFormat;
            }

            public Logger GetLogger(string name)
            {
                return (level, messageFunc, exception, parameters) =>
                {
                    if (level < _level)
                    {
                        return false;
                    }

                    var msg = messageFunc?.Invoke();

                    if (msg != null && (parameters?.Any() ?? false))
                    {
                        msg = string.Format(msg, parameters);
                    }

                    if (msg != null || exception != null)
                    {
                        var stream = level == LogLevel.Error || level == LogLevel.Fatal ? _console.Error : _console.Out;
                        msg = _dateTimeFormat != null
                        ? $"{level.ToString().First()} {DateTime.Now.ToString(_dateTimeFormat)} {name} > {msg} {exception}"
                        : $"{level.ToString().First()} {name} > {msg} {exception}";
                        stream.WriteLine(msg);
                    }

                    return true;
                };
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
            {
                throw new NotImplementedException();
            }
        }
    }
}