using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.Tokens
{
    internal static class TokenizerPipeline
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static Task<int> TokenizeInputMiddleware(CommandContext commandContext, ExecutionDelegate next)
        {
            InsertSystemTransformations(commandContext.AppConfig);
            
            if (!ApplyTokenTransformations(commandContext))
            {
                return ExitCodes.Error;
            }
            
            commandContext.AppConfig.TokenizationEvents.TokenizationCompleted(commandContext);
            return next(commandContext);
        }

        private static bool ApplyTokenTransformations(CommandContext commandContext)
        {
            var tokens = commandContext.Tokens;
            var appConfig = commandContext.AppConfig;
            foreach (var transformation in appConfig.TokenTransformations)
            {
                try
                {
                    var tempArgs = transformation.Transformation(commandContext, tokens);
                    tempArgs.Where(t => t.SourceName.IsNullOrWhitespace()).ForEach(t => t.SourceName = transformation.Name);
                    appConfig.TokenizationEvents.TokenTransformation(commandContext, transformation, tokens, tempArgs);
                    tokens = tempArgs;
                }
                catch (Exception e)
                {
                    Log.Error($"Tokenizer error: {transformation} {e.Message}", e);
                    commandContext.Console.Error.WriteLine(e.Message);
                    return false;
                }
            }

            commandContext.Tokens = tokens;
            return true;
        }

        private static void InsertSystemTransformations(AppConfig appConfig)
        {
            // append system transformations to the end.
            // these are features we want to ensure are applied to all arguments
            // because parsing logic depends on these being processed
            appConfig.TokenTransformations = appConfig.TokenTransformations
                .OrderBy(t => t.Order)
                .Union(
                    new[]
                    {
                        new TokenTransformation(
                            "expand-clubbed-flags",
                            Int32.MaxValue,
                            ExpandClubbedOptionsTransformation.Transform),
                        new TokenTransformation(
                            "split-option-assignments",
                            Int32.MaxValue,
                            SplitOptionsTransformation.Transform)
                    })
                .ToArray();
        }
    }
}