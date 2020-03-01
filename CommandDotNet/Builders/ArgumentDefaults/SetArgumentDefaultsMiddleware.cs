﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Builders.ArgumentDefaults
{
    internal static class SetArgumentDefaultsMiddleware
    {
        internal static AppRunner SetArgumentDefaultsFrom(AppRunner appRunner, Func<IArgument, string>[] getDefaultValueCallbacks)
        {
            // run before help command so help will display the updated defaults
            return appRunner.Configure(c =>
            {
                var config = new Config
                {
                    GetDefaultValueCallbacks = getDefaultValueCallbacks
                };
                c.Services.AddOrUpdate(config);

                // run before help so the default values can be displayed in the help text 
                c.UseMiddleware(SetDefaults, 
                    MiddlewareSteps.SetArgumentDefaults.Stage,
                    MiddlewareSteps.SetArgumentDefaults.Order);
            });
        }

        private static Task<int> SetDefaults(CommandContext context, ExecutionDelegate next)
        {
            if (context.ParseResult.ParseError != null)
            {
                return next(context);
            }

            var config = context.AppConfig.Services.Get<Config>();
            var command = context.ParseResult.TargetCommand;
            
            foreach (var argument in command.AllArguments(true))
            {
                var value = config.GetDefaultValueCallbacks.Select(func => func(argument)).FirstOrDefault();
                if (value != null)
                {
                    if (argument.Arity.AllowsMany())
                    {
                        argument.DefaultValue = value.Split(',');
                    }
                    else
                    {
                        argument.DefaultValue = value;
                    }
                }
            }

            return next(context);
        }

        private class Config
        {
            public Func<IArgument, string>[] GetDefaultValueCallbacks { get; set; }
        }
    }
}
