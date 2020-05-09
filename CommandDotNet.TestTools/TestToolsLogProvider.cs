using System;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.TestTools
{
    public class TestToolsLogProvider : ILogProvider
    {
        private Action<string>? _logLine;

        public static IDisposable InitLogProvider(Action<string> logLine)
        {
            if (logLine == null)
            {
                throw new ArgumentNullException(nameof(logLine));
            }

            if (LogProvider.CurrentLogProvider is TestToolsLogProvider provider)
            {
                provider._logLine = logLine;
            }
            else
            {
                provider = new TestToolsLogProvider { _logLine = logLine };
                LogProvider.SetCurrentLogProvider(provider);
            }

            LogProvider.IsDisabled = false;
            return new DisposableAction(() => LogProvider.IsDisabled = true);
        }

        public Logger GetLogger(string name)
        {
            return (level, messageFunc, exception, parameters) =>
            {
                var msg = messageFunc?.Invoke();

                if (msg != null && (parameters?.Any() ?? false))
                {
                    msg = string.Format(msg, parameters);
                }

                if (msg != null || exception != null)
                {
                    _logLine?.Invoke($"{level.ToString().First()} {name} > {msg} {exception}");
                }

                return true;
            };
        }

        public IDisposable OpenNestedContext(string message)
        {
            return new NullDisposable();
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            return new NullDisposable();
        }

        private class NullDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}