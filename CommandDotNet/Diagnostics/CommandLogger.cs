using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Builders;
using CommandDotNet.Diagnostics.Parse;
using CommandDotNet.Extensions;

namespace CommandDotNet.Diagnostics
{
    public static class CommandLogger
    {
        public static bool HasLoggedFor(CommandContext context)
        {
            return context?.Services.GetOrDefault<CommandLoggerHasLoggedMarker>() != null;
        }

        public static void Log(
            CommandContext context,
            Action<string?>? writer = null,
            bool includeSystemInfo = true,
            bool includeAppConfig = false)
        {
            var config = context.AppConfig.Services.GetOrDefault<CommandLoggerConfig>();
            if (config == null)
            {
                throw new InvalidConfigurationException($"{nameof(CommandLoggerMiddleware)} has not been registered. " +
                                                        $"Try `appRunner.{nameof(AppRunnerConfigExtensions.UseCommandLogger)}()`");
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Services.AddOrUpdate(new CommandLoggerHasLoggedMarker());

            var sb = new StringBuilder(Environment.NewLine);

            sb.AppendLine("***************************************");

            var originalArgs = RemovePasswords(context, context.Original.Args.ToCsv(" "));
            sb.AppendLine("Original input:");
            sb.AppendLine($"  {originalArgs}");
            sb.AppendLine();

            var indent = new Indent();
            ParseReporter.Report(context, writeln: s => sb.AppendLine(s), indent: indent);

            var additionalHeaders = config.AdditionalHeadersCallback?.Invoke(context);
            var otherConfigEntries = GetOtherConfigInfo(context, includeSystemInfo, additionalHeaders).ToList();
            if (!otherConfigEntries.IsNullOrEmpty())
            {
                sb.AppendLine();
                var maxName = otherConfigEntries.Max(e => e.name.Length);
                foreach (var entry in otherConfigEntries)
                {
                    sb.AppendFormat($"{{0, -{maxName + 1}}} = {{1}}", entry.name, entry.text);
                    sb.AppendLine();
                }
            }

            if (includeAppConfig)
            {
                sb.AppendLine();
                sb.AppendLine(context.AppConfig.ToString(indent.Increment()));
            }

            sb.Append("***************************************");
            writer ??= context.Console.Out.Write;
            writer(sb.ToString());
        }

        private static string RemovePasswords(CommandContext commandContext, string originalArgs)
        {
            commandContext.ParseResult?.TargetCommand?
                .AllArguments(includeInterceptorOptions: true)
                .Where(a => a.IsObscured())
                .ForEach(a => a.InputValues
                    .Where(iv => iv.Source == Constants.InputValueSources.Argument)
                    .SelectMany(iv => iv.Values)
                    .ForEach(pwd => originalArgs = originalArgs.Replace(pwd, Password.ValueReplacement)));
            return originalArgs;
        }

        private static IEnumerable<(string name, string text)> GetOtherConfigInfo(
            CommandContext commandContext,
            bool includeSystemInfo,
            IEnumerable<(string key, string value)>? additionalHeaders)
        {
            if (includeSystemInfo)
            {
                var appInfo = AppInfo.GetAppInfo(commandContext);
                yield return ("Tool version", $"{appInfo.FileName} {appInfo.Version}");
                yield return (".Net version",
                    System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.Trim());
                yield return ("OS version", System.Runtime.InteropServices.RuntimeInformation.OSDescription.Trim());
                yield return ("Machine", Environment.MachineName);
                yield return ("Username", $"{Environment.UserDomainName}\\{Environment.UserName}");
            }

            if (additionalHeaders is { })
            {
                foreach (var header in additionalHeaders)
                {
                    yield return (header.key, header.value);
                }
            }
        }

        private class CommandLoggerHasLoggedMarker { }
    }
}