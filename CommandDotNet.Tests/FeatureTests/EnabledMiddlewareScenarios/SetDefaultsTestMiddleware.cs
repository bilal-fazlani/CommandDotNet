using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Tests.FeatureTests.EnabledMiddlewareScenarios
{
    public static class SetDefaultsTestMiddleware
    {
        private const int BeforeHelpMiddleware = -1;

        public static AppRunner SetDefaults(this AppRunner appRunner, IDictionary<string, object> defaults)
        {
            // run before help command so help will display the updated defaults
            return appRunner.Configure(c =>
            {
                c.UseMiddleware((ctx, next) => SetDefaults(ctx, next, defaults), 
                    MiddlewareStages.PostParseInputPreBindValues, 
                    BeforeHelpMiddleware);
            });
        }

        private static Task<int> SetDefaults(
            CommandContext context,
            ExecutionDelegate next, 
            IDictionary<string, object> defaults)
        {
            if (context.ParseResult!.ParseError != null)
            {
                return next(context);
            }

            var command = context.ParseResult.TargetCommand;
            var interceptor = command.Parent;

            var arguments = command.Options.Cast<IArgument>().Union(command.Operands);
            if (interceptor is { })
            {
                arguments = arguments.Union(interceptor.Options).Union(interceptor.Operands);
            }

            foreach (var argument in arguments)
            { 
                if (defaults.TryGetValue(argument.Name, out var value))
                {
                    argument.Default = new ArgumentDefault("tests", "", value);
                }
            }

            return next(context);
        }
    }
}