using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.ConsoleOnly;
using CommandDotNet.Execution;
using Semver;

namespace CommandDotNet.NewerReleasesAlerts
{
    public static class AlertOnNewerReleaseMiddleware
    {
        public delegate string? ParseSemanticVersionFromResponseBodyDelegate(string? responseBody);
        public delegate string PostfixAlertMessageDelegate(string latestReleaseVersion);

        /// <summary>
        /// This middleware will call the <see cref="latestReleaseUrl"/> and
        /// use <see cref="parseSemanticVersionFromResponseBodyCallback"/> to parse a
        /// semantic version from the response.<br/>
        /// If the version is greater than the file version of the assembly,
        /// an alert is output to the console.<br/>
        /// See <see cref="GitHubMiddleware"/> for an example of how to use this method.
        /// </summary>
        /// <param name="appRunner">the <see cref="AppRunner"/> to run the middleware</param>
        /// <param name="latestReleaseUrl">the url for metadata about the latest release</param>
        /// <param name="parseSemanticVersionFromResponseBodyCallback">callback to get the semantic version for response from the <see cref="latestReleaseUrl"/></param>
        /// <param name="postfixAlertMessageCallback">the results of this callback will be post-fixed to the alert message.  i.e. download link.</param>
        /// <param name="overrideHttpRequestCallback">use this callback to append headers and auth info for tests. Also useful for mocking requests for unit tests.</param>
        /// <param name="skipCommand">
        /// Use this to skip the alert for commands where the alert would result in bad output.
        /// i.e. Command output that could be piped to another command.
        /// </param>
        public static AppRunner UseNewerReleaseAlert(this AppRunner appRunner, 
            string latestReleaseUrl,
            ParseSemanticVersionFromResponseBodyDelegate parseSemanticVersionFromResponseBodyCallback,
            PostfixAlertMessageDelegate? postfixAlertMessageCallback = null,
            OverrideHttpRequestCallback? overrideHttpRequestCallback = null,
            Predicate<Command>? skipCommand = null)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(AlertOnNewVersion, MiddlewareSteps.NewerReleaseAlerts);
                c.Services.Add(new NewerReleaseConfig(
                    latestReleaseUrl,
                    parseSemanticVersionFromResponseBodyCallback,
                    postfixAlertMessageCallback,
                    overrideHttpRequestCallback,
                    skipCommand
                ));
            });
        }

        private class NewerReleaseConfig
        {
            public string LatestReleaseUrl { get; }
            public ParseSemanticVersionFromResponseBodyDelegate ParseSemanticVersionFromResponseBodyCallback { get; }
            public PostfixAlertMessageDelegate? PostfixAlertMessageCallback { get; }
            public OverrideHttpRequestCallback? OverrideHttpRequestCallback { get; }
            public Predicate<Command>? SkipCommand { get; }

            public NewerReleaseConfig(string latestReleaseUrl, 
                ParseSemanticVersionFromResponseBodyDelegate semanticVersionFromResponseBodyCallback, 
                PostfixAlertMessageDelegate? postfixAlertMessageCallback, 
                OverrideHttpRequestCallback? overrideHttpRequestCallback, 
                Predicate<Command>? skipCommand)
            {
                LatestReleaseUrl = latestReleaseUrl;
                ParseSemanticVersionFromResponseBodyCallback = semanticVersionFromResponseBodyCallback;
                PostfixAlertMessageCallback = postfixAlertMessageCallback;
                OverrideHttpRequestCallback = overrideHttpRequestCallback;
                SkipCommand = skipCommand;
            }
        }

        private static Task<int> AlertOnNewVersion(CommandContext context, ExecutionDelegate next)
        {
            var config = context.AppConfig.Services.GetOrThrow<NewerReleaseConfig>();

            var skipCommand = config.SkipCommand?.Invoke(context.ParseResult!.TargetCommand) ?? false;
            if (!skipCommand)
            {
                SemVersion? latestReleaseVersion = null;

                var shouldAlert = TryGetCurrentVersion(out var currentVersion)
                                  && TryGetLatestReleaseVersion(context, config, out latestReleaseVersion)
                                  && currentVersion < latestReleaseVersion;
                if (shouldAlert)
                {
                    var message = $"A newer release exists. Current:{currentVersion} Latest:{latestReleaseVersion}";
                    if (config.PostfixAlertMessageCallback != null)
                    {
                        message += $" {config.PostfixAlertMessageCallback?.Invoke(latestReleaseVersion!.ToString())}";
                    }

                    context.Console.Out.WriteLine(message);
                    context.Console.Out.WriteLine();
                }
            }

            return next(context);
        }

        private static bool TryGetCurrentVersion(out SemVersion semVersion)
        {
            return SemVersion.TryParse(AppInfo.Instance.Version, out semVersion);

        }

        private static bool TryGetLatestReleaseVersion(CommandContext context, NewerReleaseConfig config, out SemVersion? semVersion)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");

                var requestUri = new Uri(config.LatestReleaseUrl);

                var response = config.OverrideHttpRequestCallback?.Invoke(httpClient, requestUri).Result
                               ?? httpClient.GetStringAsync(requestUri).Result;

                var version = config.ParseSemanticVersionFromResponseBodyCallback(response);

                return SemVersion.TryParse(version, out semVersion);
            }
            catch(Exception e)
            {
                // don't fail operation just for this
                context.Console.Error.WriteLine($"failed to retrieve latest release info. {e.Message}");
                semVersion = null;
                return false;
            }
        }
    }
}
