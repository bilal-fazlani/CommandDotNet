using System;
using System.Collections.Generic;

namespace CommandDotNet.Diagnostics
{
    internal class CommandLoggerConfig
    {
        public Func<CommandContext, Action<string?>?>? WriterFactory { get; }
        public bool IncludeSystemInfo { get; }
        public bool IncludeAppConfig { get; }
        public Func<CommandContext, IEnumerable<(string key, string value)>?>? AdditionalHeadersCallback { get; }

        public CommandLoggerConfig(
            Func<CommandContext, Action<string?>?>? writerFactory,
            bool includeSystemInfo,
            bool includeAppConfig,
            Func<CommandContext, IEnumerable<(string key, string value)>?>? additionalHeadersCallback)
        {
            WriterFactory = writerFactory;
            IncludeSystemInfo = includeSystemInfo;
            IncludeAppConfig = includeAppConfig;
            AdditionalHeadersCallback = additionalHeadersCallback;
        }
    }
}