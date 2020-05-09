using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests
{
    public class UseDefaultMiddleware_DuplicateRegistration_Tests
    {
        [Fact]
        public void UseDefaultMiddleware_GivesInformativeError()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new AppRunner<AppConfigBuilder_UseMiddleware_Tests>()
                        .UseDebugDirective()
                        .UseDefaultMiddleware())
                .Message.Should().Be(MsgFor(
                    "middleware", 
                    "CommandDotNet.Diagnostics.DebugDirective.AttachDebugger", 
                    "excludeDebugDirective"));
        }
        
        [Fact]
        public void UseDebugDirective_GivesInformativeError()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new AppRunner<AppConfigBuilder_UseMiddleware_Tests>()
                        .UseDefaultMiddleware()
                        .UseDebugDirective())
                .Message.Should().Be(MsgFor(
                    "middleware", 
                    "CommandDotNet.Diagnostics.DebugDirective.AttachDebugger", 
                    "excludeDebugDirective"));
        }
        
        [Fact]
        public void UseParseDirective_GivesInformativeError()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new AppRunner<AppConfigBuilder_UseMiddleware_Tests>()
                        .UseDefaultMiddleware()
                        .UseParseDirective())
                .Message.Should().Be(MsgFor(
                    "middleware", 
                    "CommandDotNet.Diagnostics.ParseDirective.ConfigureParseReportByTokenTransform", 
                    "excludeParseDirective"));
        }
        
        [Fact]
        public void UseCancellationHandlers_GivesInformativeError()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new AppRunner<AppConfigBuilder_UseMiddleware_Tests>()
                        .UseDefaultMiddleware()
                        .UseCancellationHandlers())
                .Message.Should().Be(MsgFor(
                    "middleware", 
                    "CommandDotNet.Execution.CancellationMiddleware.AddCancellationTokens", 
                    "excludeCancellationHandlers"));
        }
        
        [Fact]
        public void UsePrompting_GivesInformativeError()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new AppRunner<AppConfigBuilder_UseMiddleware_Tests>()
                        .UseDefaultMiddleware()
                        .UsePrompting())
                .Message.Should().Be(MsgFor(
                    "parameter resolver",
                    "CommandDotNet.Prompts.IPrompter",
                    "excludePrompting"));
        }
        
        [Fact]
        public void UseResponseFiles_GivesInformativeError()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new AppRunner<AppConfigBuilder_UseMiddleware_Tests>()
                        .UseDefaultMiddleware()
                        .UseResponseFiles())
                .Message.Should().Be(MsgFor(
                    "token transformation", 
                    "expand-response-files", 
                    "excludeResponseFiles"));
        }
        
        [Fact]
        public void UseVersionMiddleware_GivesInformativeError()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new AppRunner<AppConfigBuilder_UseMiddleware_Tests>()
                        .UseDefaultMiddleware()
                        .UseVersionMiddleware())
                .Message.Should().Be(MsgFor(
                    "middleware", 
                    "CommandDotNet.Builders.VersionMiddleware.DisplayVersionIfSpecified", 
                    "excludeVersionMiddleware"));
        }
        
        [Fact]
        public void AppendPipedInputToOperandList_GivesInformativeError()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new AppRunner<AppConfigBuilder_UseMiddleware_Tests>()
                        .UseDefaultMiddleware()
                        .AppendPipedInputToOperandList())
                .Message.Should().Be(MsgFor(
                    "middleware", 
                    "CommandDotNet.Parsing.PipedInputMiddleware.InjectPipedInputToOperandList", 
                    "excludeAppendPipedInputToOperandList"));
        }
        
        [Fact]
        public void UseTypoSuggestions_GivesInformativeError()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new AppRunner<AppConfigBuilder_UseMiddleware_Tests>()
                        .UseDefaultMiddleware()
                        .UseTypoSuggestions())
                .Message.Should().Be(MsgFor(
                    "middleware", 
                    "CommandDotNet.Parsing.Typos.TypoSuggestionsMiddleware.TypoSuggest", 
                    "excludeTypoSuggestions"));
        }

        private static string MsgFor(string componentType, string componentName, string excludeParamName)
        {
            return $"{componentType} '{componentName}' " +
                   "has already been registered via 'UseDefaultMiddleware'. " +
                   $"Try `.UseDefaultMiddleware({excludeParamName}:true)` " +
                   $"to register with other extension methods.";
        }
    }
}