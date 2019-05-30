using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.BddTests.Apps;
using CommandDotNet.Tests.BddTests.Framework;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class UsageAppNameStylesScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<WithAppMetadataName>("Adaptive style uses GlobalTool style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.Adaptive}}},
                    WhenArgs = "-h",
                    Then = { HelpContainsTexts = { "Usage: AppName" }}
                },
                new Given<WithAppMetadataName>("GlobalTool style uses GlobalTool style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.GlobalTool}}},
                    WhenArgs = "-h",
                    Then = { HelpContainsTexts = { "Usage: AppName" }}
                },
                new Given<WithAppMetadataName>("DotNet style uses DotNet style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.DotNet}}},
                    WhenArgs = "-h",
                    Then = { HelpContainsTexts = { "Usage: dotnet testhost.dll" } }
                },
                new Given<WithAppMetadataName>("Executable style uses Executable style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.Executable}}},
                    WhenArgs = "-h",
                    Then = { HelpContainsTexts = { "Usage: testhost.dll" } }
                },

                new Given<WithoutAppMetadatName>("Adaptive style falls back to DotNet style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.Adaptive}}},
                    WhenArgs = "-h",
                    Then = { HelpContainsTexts = { "Usage: dotnet testhost.dll" } }
                },
                new Given<WithoutAppMetadatName>("GlobalTool style throws configuration exception")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.GlobalTool}}},
                    WhenArgs = "-h",
                    Then =
                    {
                        ExitCode = 1,
                        HelpContainsTexts = { "Invalid configuration: ApplicationMetadataAttribute.Name is required for the root command when UsageAppNameStyle.GlobalTool is specified." }
                    }
                },
                new Given<WithoutAppMetadatName>("DotNet style uses DotNet style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.DotNet}}},
                    WhenArgs = "-h",
                    Then = { HelpContainsTexts = { "Usage: dotnet testhost.dll" } }
                },
                new Given<WithoutAppMetadatName>("Executable style uses Executable style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.Executable}}},
                    WhenArgs = "-h",
                    Then = { HelpContainsTexts = { "Usage: testhost.dll" } }
                },
            };

        [ApplicationMetadata(Name = "AppName")]
        public class WithAppMetadataName
        {

        }

        public class WithoutAppMetadatName
        {

        }
    }
}