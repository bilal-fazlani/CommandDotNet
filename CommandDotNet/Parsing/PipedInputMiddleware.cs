using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.ConsoleOnly;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.Parsing
{
    internal static class PipedInputMiddleware
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        internal static AppRunner AppendPipedInputToOperandList(AppRunner appRunner)
        {
            // -1 to ensure this middleware runs before any prompting so the value won't appear null
            return appRunner.Configure(c => 
                c.UseMiddleware(InjectPipedInputToOperandList, MiddlewareSteps.PipedInput));
        }

        private static Task<int> InjectPipedInputToOperandList(CommandContext ctx, ExecutionDelegate next)
        {
            if (ctx.Console.IsInputRedirected)
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
                var operand = ctx.ParseResult!.TargetCommand.Operands
                    .FirstOrDefault(o => o.Arity.AllowsMany());

                if (operand is null)
                {
                    Log.DebugFormat("No list operands found for {0}", ctx.ParseResult.TargetCommand.Name);
                }
                else
                {
                    Log.DebugFormat("Piping input to {0}.{1}", ctx.ParseResult.TargetCommand.Name, operand.Name);
                    operand.InputValues.Add(new InputValue(Constants.InputValueSources.Piped, true, GetPipedInput(ctx.Console)));
                }
            }

            return next(ctx);
        }

        public static IEnumerable<string> GetPipedInput(IConsole console)
        {
            Func<string?> readLine = console.In.ReadLine;
            return readLine.EnumeratePipedInput();
        }
    }
}