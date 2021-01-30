using CommandDotNet.Help;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class UsageAppNameStylesTests
    {
        public UsageAppNameStylesTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }
        
        [Fact]
        public void DotNetStyleUsesDotNetStyle()
        {
            var appSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.DotNet } };
            new AppRunner<WithoutAppMetadatName>(appSettings).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then = { OutputContainsTexts = { "Usage: dotnet testhost.dll" } }
            });
        }

        [Fact]
        public void ExecutableStyleUsesExecutableStyle()
        {
            var appSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.Executable } };
            new AppRunner<WithAppMetadataName>(appSettings).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then = { OutputContainsTexts = { "Usage: testhost.dll" } }
            });
        }

        [Fact]
        public void AdaptiveStyleFallsBackToDotNetStyle()
        {
            var appSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.Adaptive } };
            new AppRunner<WithoutAppMetadatName>(appSettings).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then = { OutputContainsTexts = { "Usage: dotnet testhost.dll" } }
            });
        }

        [Fact]
        public void UsageAppNameSettingUsedWhenProvided()
        {
            var appSettings = new AppSettings { Help = { UsageAppName = "WhatATool" } };
            new AppRunner<WithAppMetadataName>(appSettings).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then = { OutputContainsTexts = { "Usage: WhatATool" } }
            });
        }

        [Command(Name = "AppName")]
        private class WithAppMetadataName
        {

        }

        private class WithoutAppMetadatName
        {

        }

        [Command(Usage = "use %AppName%")]
        private class UsageAppNameTemplate
        {

        }
    }
}
