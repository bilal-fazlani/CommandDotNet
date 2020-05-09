using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests
{
    public class AppConfigBuilder_UseMiddleware_Tests
    {
        [Fact]
        public void AllowMultipleRegistrations_Defaults_False()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                new AppConfigBuilder(new AppSettings())
                    .UseMiddleware(Middleware1, MiddlewareStages.Tokenize)
                    .UseMiddleware(Middleware1, MiddlewareStages.Tokenize))
                .Message.Should().Be("middleware " +
                                     "'CommandDotNet.Tests.UnitTests.AppConfigBuilder_UseMiddleware_Tests.Middleware1' " +
                                     "has already been registered");
        }
        
        [Fact]
        public void AllowMultipleRegistrations_WhenFalse_StageDoesNotMatter()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new AppConfigBuilder(new AppSettings())
                        .UseMiddleware(Middleware1, MiddlewareStages.Tokenize)
                        .UseMiddleware(Middleware1, MiddlewareStages.ParseInput))
                .Message.Should().Be("middleware " +
                                     "'CommandDotNet.Tests.UnitTests.AppConfigBuilder_UseMiddleware_Tests.Middleware1' " +
                                     "has already been registered");
        }
        
        [Fact]
        public void AllowMultipleRegistrations_WhenTrue_AMethodCanBeRegisteredMultipleTimes()
        {
            var appConfig = new AppConfigBuilder(new AppSettings())
                .UseMiddleware(Middleware1, MiddlewareStages.Tokenize, allowMultipleRegistrations: true)
                .UseMiddleware(Middleware1, MiddlewareStages.Tokenize, allowMultipleRegistrations: true)
                .Build();

            appConfig.Should().NotBeNull();
        }

        private Task<int> Middleware1(CommandContext context, ExecutionDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}