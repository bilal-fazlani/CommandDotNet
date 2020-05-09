using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.NewerReleasesAlerts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.NewerReleasesAlerts
{
    public class NewReleaseAlertOnGitHubTests
    {
        public NewReleaseAlertOnGitHubTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void ShouldAlertWhenNewerVersionExists()
        {
            var organizationName = "bilal-fazlani";
            var repositoryName = "commanddotnet";
            var version = "1.0.0";

            new AppRunner<App>()
                .Configure(c => c.Services.AddOrUpdate(BuildAppInfo(version)))
                .UseNewerReleaseAlertOnGitHub(organizationName, repositoryName,
                    overrideHttpRequestCallback: (client, uri) => Task.FromResult(BuildGitHubApiResponse("1.0.1")))
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then =
                    {
                        OutputContainsTexts =
                        {
                            $"A newer release exists. Current:{version} Latest:",
                            $"Download from https://github.com/{organizationName}/{repositoryName}/releases/tag/"
                        }
                    }
                });
        }

        [Fact]
        public void ShouldNotAlertWhenNewerVersionDoesNotExist()
        {
            var organizationName = "bilal-fazlani";
            var repositoryName = "commanddotnet";
            var version = "1.0.1";

            new AppRunner<App>()
                .Configure(c => c.Services.AddOrUpdate(BuildAppInfo(version)))
                .UseNewerReleaseAlertOnGitHub(organizationName, repositoryName, 
                    overrideHttpRequestCallback: (client, uri) => Task.FromResult(BuildGitHubApiResponse("1.0.0")))
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then =
                    {
                        OutputNotContainsTexts =
                        {
                            $"A newer release exists. Current:{version} Latest:",
                            $"Download from https://github.com/{organizationName}/{repositoryName}/releases/tag/"
                        }
                    }
                });
        }

        [Fact]
        public void ShouldNotAlertWhenTargetCommandIsSkipped()
        {
            var organizationName = "bilal-fazlani";
            var repositoryName = "commanddotnet";
            var version = "1.0.0";

            new AppRunner<App>()
                .Configure(c => c.Services.AddOrUpdate(BuildAppInfo(version)))
                .UseNewerReleaseAlertOnGitHub(organizationName, repositoryName,
                    overrideHttpRequestCallback: (client, uri) => Task.FromResult(BuildGitHubApiResponse("1.0.1")),
                    skipCommand:command => true)
                .Verify(new Scenario
                {
                    When = {Args = "Do"},
                    Then =
                    {
                        OutputNotContainsTexts =
                        {
                            $"A newer release exists. Current:{version} Latest:",
                            $"Download from https://github.com/{organizationName}/{repositoryName}/releases/tag/"
                        }
                    }
                });
        }

        public class App
        {
            public void Do()
            {
            }
        }

        private static AppInfo BuildAppInfo(string version) => new AppInfo(
            false, false, false,
            typeof(NewReleaseAlertOnGitHubTests).Assembly,
            "blah", "blah", version);

        public static string BuildGitHubApiResponse(string version) =>
            $"{{\"url\":\"https://api.github.com/repos/bilal-fazlani/commanddotnet/releases/14394703\",\"assets_url\":\"https://api.github.com/repos/bilal-fazlani/commanddotnet/releases/14394703/assets\",\"upload_url\":\"https://uploads.github.com/repos/bilal-fazlani/commanddotnet/releases/14394703/assets{{?name,label}}\",\"html_url\":\"https://github.com/bilal-fazlani/commanddotnet/releases/tag/{version}\",\"id\":14394703,\"node_id\":\"MDc6UmVsZWFzZTE0Mzk0NzAz\",\"tag_name\":\"{version}\",\"target_commitish\":\"1091720bc6dc301ad6da8e4041c7102567064ff8\",\"name\":\"{version}\",\"draft\":false,\"author\":{{\"login\":\"bilal-fazlani\",\"id\":3396271,\"node_id\":\"MDQ6VXNlcjMzOTYyNzE=\",\"avatar_url\":\"https://avatars0.githubusercontent.com/u/3396271?v=4\",\"gravatar_id\":\"\",\"url\":\"https://api.github.com/users/bilal-fazlani\",\"html_url\":\"https://github.com/bilal-fazlani\",\"followers_url\":\"https://api.github.com/users/bilal-fazlani/followers\",\"following_url\":\"https://api.github.com/users/bilal-fazlani/following{{/other_user}}\",\"gists_url\":\"https://api.github.com/users/bilal-fazlani/gists{{/gist_id}}\",\"starred_url\":\"https://api.github.com/users/bilal-fazlani/starred{{/owner}}{{/repo}}\",\"subscriptions_url\":\"https://api.github.com/users/bilal-fazlani/subscriptions\",\"organizations_url\":\"https://api.github.com/users/bilal-fazlani/orgs\",\"repos_url\":\"https://api.github.com/users/bilal-fazlani/repos\",\"events_url\":\"https://api.github.com/users/bilal-fazlani/events{{/privacy}}\",\"received_events_url\":\"https://api.github.com/users/bilal-fazlani/received_events\",\"type\":\"User\",\"site_admin\":false}},\"prerelease\":false,\"created_at\":\"2018-12-06T19:38:32Z\",\"published_at\":\"2018-12-06T19:41:16Z\",\"assets\":[{{\"url\":\"https://api.github.com/repos/bilal-fazlani/commanddotnet/releases/assets/10034919\",\"id\":10034919,\"node_id\":\"MDEyOlJlbGVhc2VBc3NldDEwMDM0OTE5\",\"name\":\"CommandDotNet.{version}.nupkg\",\"label\":\"\",\"uploader\":{{\"login\":\"bilal-fazlani\",\"id\":3396271,\"node_id\":\"MDQ6VXNlcjMzOTYyNzE=\",\"avatar_url\":\"https://avatars0.githubusercontent.com/u/3396271?v=4\",\"gravatar_id\":\"\",\"url\":\"https://api.github.com/users/bilal-fazlani\",\"html_url\":\"https://github.com/bilal-fazlani\",\"followers_url\":\"https://api.github.com/users/bilal-fazlani/followers\",\"following_url\":\"https://api.github.com/users/bilal-fazlani/following{{/other_user}}\",\"gists_url\":\"https://api.github.com/users/bilal-fazlani/gists{{/gist_id}}\",\"starred_url\":\"https://api.github.com/users/bilal-fazlani/starred{{/owner}}{{/repo}}\",\"subscriptions_url\":\"https://api.github.com/users/bilal-fazlani/subscriptions\",\"organizations_url\":\"https://api.github.com/users/bilal-fazlani/orgs\",\"repos_url\":\"https://api.github.com/users/bilal-fazlani/repos\",\"events_url\":\"https://api.github.com/users/bilal-fazlani/events{{/privacy}}\",\"received_events_url\":\"https://api.github.com/users/bilal-fazlani/received_events\",\"type\":\"User\",\"site_admin\":false}},\"content_type\":\"application/octet-stream\",\"state\":\"uploaded\",\"size\":31856,\"download_count\":0,\"created_at\":\"2018-12-06T19:41:15Z\",\"updated_at\":\"2018-12-06T19:41:15Z\",\"browser_download_url\":\"https://github.com/bilal-fazlani/commanddotnet/releases/download/{version}/CommandDotNet.{version}.nupkg\"}},{{\"url\":\"https://api.github.com/repos/bilal-fazlani/commanddotnet/releases/assets/10034920\",\"id\":10034920,\"node_id\":\"MDEyOlJlbGVhc2VBc3NldDEwMDM0OTIw\",\"name\":\"CommandDotNet.{version}.symbols.nupkg\",\"label\":\"\",\"uploader\":{{\"login\":\"bilal-fazlani\",\"id\":3396271,\"node_id\":\"MDQ6VXNlcjMzOTYyNzE=\",\"avatar_url\":\"https://avatars0.githubusercontent.com/u/3396271?v=4\",\"gravatar_id\":\"\",\"url\":\"https://api.github.com/users/bilal-fazlani\",\"html_url\":\"https://github.com/bilal-fazlani\",\"followers_url\":\"https://api.github.com/users/bilal-fazlani/followers\",\"following_url\":\"https://api.github.com/users/bilal-fazlani/following{{/other_user}}\",\"gists_url\":\"https://api.github.com/users/bilal-fazlani/gists{{/gist_id}}\",\"starred_url\":\"https://api.github.com/users/bilal-fazlani/starred{{/owner}}{{/repo}}\",\"subscriptions_url\":\"https://api.github.com/users/bilal-fazlani/subscriptions\",\"organizations_url\":\"https://api.github.com/users/bilal-fazlani/orgs\",\"repos_url\":\"https://api.github.com/users/bilal-fazlani/repos\",\"events_url\":\"https://api.github.com/users/bilal-fazlani/events{{/privacy}}\",\"received_events_url\":\"https://api.github.com/users/bilal-fazlani/received_events\",\"type\":\"User\",\"site_admin\":false}},\"content_type\":\"application/octet-stream\",\"state\":\"uploaded\",\"size\":83491,\"download_count\":0,\"created_at\":\"2018-12-06T19:41:16Z\",\"updated_at\":\"2018-12-06T19:41:16Z\",\"browser_download_url\":\"https://github.com/bilal-fazlani/commanddotnet/releases/download/{version}/CommandDotNet.{version}.symbols.nupkg\"}}],\"tarball_url\":\"https://api.github.com/repos/bilal-fazlani/commanddotnet/tarball/{version}\",\"zipball_url\":\"https://api.github.com/repos/bilal-fazlani/commanddotnet/zipball/{version}\",\"body\":\"\"}}";
    }
}
