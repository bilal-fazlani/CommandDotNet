using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.Prompting
{
    public class ReadLineContext
    {
        public Line Line { get; }
        public ReadLineConfig Config { get; }
        public Dictionary<Type, object> State { get; }

        public ICollection<string>? History { get; set; }
        public Action? OnCtrlC { get; set; }
        public bool ShouldExitPrompt { get; set; }

        public ReadLineContext(Line line, ReadLineConfig? readLineConfig = null)
        {
            Line = line ?? throw new ArgumentNullException(nameof(line));
            Config = readLineConfig ?? ReadLineConfig.Factory();
            State = new Dictionary<Type, object>();

            History = Config.History?.GetValueOrDefault("common", () => new List<string>());
            OnCtrlC = Config.OnCtrlC;
        }
    }
}