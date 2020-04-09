using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;
using CommandDotNet.Rendering;

namespace CommandDotNet.Parsing
{
    internal static class PipedInputMiddleware
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        internal static AppRunner AppendPipedInputToOperandList(AppRunner appRunner, 
            bool enablePipingToOptions = false, string targetIndicator = "%piped%")
        {
            // -1 to ensure this middleware runs before any prompting so the value won't appear null
            return appRunner.Configure(c =>
            {
                c.Services.Add(new Config(enablePipingToOptions, targetIndicator));
                c.UseMiddleware(InjectPipedInputToOperandList,
                    MiddlewareSteps.PipedInput.Stage, MiddlewareSteps.PipedInput.Order);
            });
        }

        private class Config
        {
            public readonly bool EnablePipingToOptions;
            public readonly string TargetIndicator;

            public Config(bool enablePipingToOptions, string targetIndicator)
            {
                EnablePipingToOptions = enablePipingToOptions;
                TargetIndicator = targetIndicator;
            }
        }

        private static Task<int> InjectPipedInputToOperandList(CommandContext ctx, ExecutionDelegate next)
        {
            if (ctx.Console.IsInputRedirected)
            {
                AssignPipedInput(ctx);
            }

            return next(ctx);
        }

        private static void AssignPipedInput(CommandContext ctx)
        {
            var config = ctx.AppConfig.Services.Get<Config>();
            var command = ctx.ParseResult.TargetCommand;

            IArgument pipeTarget = null;

            if (config.EnablePipingToOptions)
            {
                pipeTarget = GetTargetOption(command, config);
            }

            if (pipeTarget == null)
            {
                // supporting only the list operand for a command gives us a few benefits
                // 1. there can be only one list operand per command.
                //    no need to enforce this only one argument has EnablePipedInput=true
                // 2. no need to handle case where a single value operand has EnablePipedInput=true
                //    otherwise we either drop all but the first value or throw an exception
                //    both have pros & cons
                // 3. List operands are specified and provided last, avoiding awkward cases
                //    where piped input is provided for arguments positioned before others.
                //    We'd need to inject additional middleware to inject tokens in this case.
                // 4. piped values can be merged with args passed to the command.
                //    this can become an option passed into appBuilder.EnablePipedInput(...)
                //    if a need arises to throw instead of merge

                pipeTarget = command.Operands.FirstOrDefault(o => o.Arity.AllowsMany());
            }


            if (pipeTarget == null)
            {
                Log.DebugFormat($"No list operands or list options with value `{config.TargetIndicator}` found for {0}",
                    command.Name);
            }
            else
            {
                Log.DebugFormat("Piping input to {0}.{1}", command.Name, pipeTarget.Name);
                pipeTarget.InputValues.Add(
                    new InputValue(Constants.InputValueSources.Piped, true, GetPipedInput(ctx.Console)));
            }
        }

        private static IArgument GetTargetOption(Command command, Config config)
        {
            /* test
                   - only one option can have indicator
                   - indicator can be changed
                   - option has piped values *appended*
                 */
            var options = command
                .AllOptions(includeInterceptorOptions: true)
                .Where(o => o.Arity.AllowsMany()
                            && o.InputValues.Any(iv =>
                                iv.Source == Constants.InputValueSources.Argument
                                && iv.Values.Any(v => v == config.TargetIndicator)))
                .ToList();

            if (options.Count > 1)
            {
                // TODO: clear exception for user
                throw new Exception();
            }

            if (options.Count == 0)
            {
                return null;
            }

            var pipeTarget = options.Single();

            var inputValue = pipeTarget.InputValues
                .First(iv => iv.Source == Constants.InputValueSources.Argument);
            if (inputValue.Values.Count() == 1)
            {
                pipeTarget.InputValues.Remove(inputValue);
            }
            else
            {
                inputValue.Values = inputValue.Values.Where(v => v != config.TargetIndicator).ToList();
            }

            return pipeTarget;
        }

        public static IEnumerable<string> GetPipedInput(IConsole console)
        {
            Func<string> readLine = console.In.ReadLine;
            return readLine.EnumeratePipedInput();
        }
    }
}