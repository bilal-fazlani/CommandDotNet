using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandDotNet.Diagnostics.Parse;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Tokens;

namespace CommandDotNet.Diagnostics
{
    internal static class ParseDirective
    {
        internal static AppRunner UseParseDirective(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(ConfigureParseReportByTokenTransform, MiddlewareSteps.ParseDirective);
                c.UseMiddleware(ParseReportByArg, MiddlewareSteps.BindValues + 100);
            });
        }

        // adapted from https://github.com/dotnet/command-line-api directives
        private static Task<int> ConfigureParseReportByTokenTransform(CommandContext commandContext, ExecutionDelegate next)
        {
            if (!commandContext.Tokens.TryGetDirective("parse", out string? value))
            {
                return next(commandContext);
            }

            var parseContext = ParseContext.Parse(value!);
            commandContext.Services.AddOrUpdate(parseContext);
            CaptureTransformations(commandContext, parseContext);

            var writer = commandContext.Console.Out;

            try
            {
                // ParseReportByArg is run within this pipeline
                var result = next(commandContext);
                if (!parseContext.Reported)
                {
                    // in case ParseReportByArg wasn't run due to parsing errors,
                    // output this the transformations as a temporary aid
                    writer.WriteLine(null);
                    writer.WriteLine(commandContext.ParseResult!.HelpWasRequested() 
                        ? "Help requested. Only token transformations are available."
                        : "Unable to map tokens to arguments. Falling back to token transformations.");
                    writer.WriteLine(parseContext.Transformations.ToString(), avoidExtraNewLine: true);
                }
                else if (parseContext.IncludeTokenization || parseContext.IncludeRawCommandLine)
                {
                    if (parseContext.IncludeTokenization)
                    {
                        writer.WriteLine(parseContext.Transformations.ToString(), avoidExtraNewLine: true);
                    }
                    if (parseContext.IncludeRawCommandLine)
                    {
                        writer.WriteLine(Environment.CommandLine, avoidExtraNewLine: true);
                    }
                }
                else
                {
                    writer.WriteLine(null);
                    writer.WriteLine($"Parse usage: [parse:{ParseContext.IncludeTransformationsArgName}:{ParseContext.IncludeRawCommandLineArgName}]" +
                                     " to include token transformations.");
                    writer.WriteLine($" '{ParseContext.IncludeTransformationsArgName}'" +
                            " to include token transformations.");
                    writer.WriteLine($" '{ParseContext.IncludeRawCommandLineArgName}'" +
                                     " to include command line as passed to this process.");
                }

                return result;
            }
            catch (Exception) when (!parseContext.Reported)
            {
                // in case ParseReportByArg wasn't run due to parsing errors,
                // output this the transformations as a temporary aid
                writer.WriteLine(null);
                writer.WriteLine("Unable to map tokens to arguments. Falling back to token transformations.");
                writer.WriteLine(parseContext.Transformations.ToString(), avoidExtraNewLine: true);
                throw;
            }
        }

        private static Task<int> ParseReportByArg(CommandContext commandContext, ExecutionDelegate next)
        {
            var parseContext = commandContext.Services.GetOrDefault<ParseContext>();
            if (parseContext != null)
            {
                ParseReporter.Report(
                    commandContext, 
                    includeRawCommandLine: parseContext.IncludeRawCommandLine,
                    writeln: s => commandContext.Console.Out.WriteLine(s));
                parseContext.Reported = true;
                return ExitCodes.Success;
            }

            return next(commandContext);
        }

        private static void CaptureTransformations(CommandContext commandContext, ParseContext parseContext)
        {
            void WriteLine(string? ln) => parseContext.Transformations.AppendLine(ln);

            WriteLine(null);
            WriteLine("token transformations:");
            WriteLine(null);

            CaptureTransformation(WriteLine, commandContext.Tokens, ">>> from shell");

            commandContext.AppConfig.TokenizationEvents.OnTokenTransformation += args =>
            {
                if (args.PreTransformTokens.Count == args.PostTransformTokens.Count &&
                    Enumerable.Range(0, args.PreTransformTokens.Count)
                        .All(i => args.PreTransformTokens[i] == args.PostTransformTokens[i]))
                {
                    CaptureTransformation(WriteLine, null, $">>> after: {args.Transformation.Name} (no changes)");
                }
                else
                {
                    CaptureTransformation(WriteLine, args.PostTransformTokens,
                        $">>> after: {args.Transformation.Name}");
                }
            };
        }

        private static void CaptureTransformation(Action<string> writeLine, TokenCollection? args, string description)
        {
            writeLine(description);

            if (args is { })
            {
                var maxTokenTypeNameLength = Enum.GetNames(typeof(TokenType)).Max(n => n.Length);

                foreach (var arg in args)
                {
                    var outputFormat = $"  {{0, -{maxTokenTypeNameLength}}}: {{1}}";
                    writeLine(string.Format(outputFormat, arg.TokenType, arg.RawValue));
                }
            }
        }

        private class ParseContext
        {
            internal const string IncludeTransformationsArgName = "t";
            internal const string IncludeRawCommandLineArgName = "raw";

            internal bool IncludeTokenization;
            internal bool IncludeRawCommandLine;
            internal bool Reported;
            internal readonly StringBuilder Transformations = new StringBuilder();

            internal static ParseContext Parse(string value)
            {
                var parts = value.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length <= 1)
                {
                    return new ParseContext();
                }

                var settings = parts[1].Split(';')
                    .Select(p => p.Split('='))
                    .ToDictionary(p => p[0], p => p.Length > 1 ? p[1] : null, StringComparer.OrdinalIgnoreCase);

                return new ParseContext
                {
                    IncludeTokenization = settings.ContainsKey(IncludeTransformationsArgName),
                    IncludeRawCommandLine = settings.ContainsKey(IncludeRawCommandLineArgName)
                };
            }
        }
    }
}