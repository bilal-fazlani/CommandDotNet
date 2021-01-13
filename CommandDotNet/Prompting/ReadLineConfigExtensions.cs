using System;

namespace CommandDotNet.Prompting
{
    public static class ReadLineConfigExtensions
    {
        public static ReadLineConfig With(this ReadLineConfig config, Action<ReadLineConfig> alter)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (alter == null)
            {
                throw new ArgumentNullException(nameof(alter));
            }

            alter(config);
            return config;
        }

        public static ReadLineConfig UseAllDefaultKeyHandlers(this ReadLineConfig config)
        {
            return config
                .UseDefaultNavigation()
                .UseDefaultEditing()
                .UseDefaultHistory();
        }

        public static ReadLineConfig UseAllReadLineKeyHandlers(this ReadLineConfig config)
        {
            return config
                .UseReadLineNavigation()
                .UseReadLineEditing()
                .UseReadLineHistory();
        }
    }
}