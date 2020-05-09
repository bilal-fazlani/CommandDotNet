using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Builders.ArgumentDefaults
{
    internal static class SetArgumentDefaultsMiddleware
    {
        internal static AppRunner SetArgumentDefaultsFrom(AppRunner appRunner, 
            Func<IArgument, ArgumentDefault?>[] getDefaultValueCallbacks)
        {
            if (getDefaultValueCallbacks == null)
            {
                throw new ArgumentNullException(nameof(getDefaultValueCallbacks));
            }

            // run before help command so help will display the updated defaults
            return appRunner.Configure(c =>
            {
                var config = c.Services.GetOrDefault<Config>();
                if (config == null)
                {
                    // run before help so the default values can be displayed in the help text 
                    c.UseMiddleware(SetDefaults, MiddlewareSteps.SetArgumentDefaults);
                    c.Services.Add(config = new Config(getDefaultValueCallbacks));
                }
                else
                {
                    config.GetDefaultValueCallbacks =
                        config.GetDefaultValueCallbacks.Union(getDefaultValueCallbacks).ToArray();
                }
            });
        }

        private static Task<int> SetDefaults(CommandContext context, ExecutionDelegate next)
        {
            if (context.ParseResult!.ParseError != null)
            {
                return next(context);
            }

            var config = context.AppConfig.Services.GetOrThrow<Config>();
            var command = context.ParseResult.TargetCommand;
            
            foreach (var argument in command.AllArguments(true))
            {
                var value = config.GetDefaultValueCallbacks
                    .Select(func => func(argument))
                    .FirstOrDefault(v => v != null);
                if (value != null)
                {
                    argument.Default = value;
                }
            }

            return next(context);
        }

        private class Config
        {
            public Func<IArgument, ArgumentDefault?>[] GetDefaultValueCallbacks { get; set; }
            
            public Config(Func<IArgument, ArgumentDefault?>[] getDefaultValueCallbacks)
            {
                GetDefaultValueCallbacks = getDefaultValueCallbacks ?? throw new ArgumentNullException(nameof(getDefaultValueCallbacks));
            }
        }
    }
}
