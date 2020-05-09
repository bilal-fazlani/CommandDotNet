using System;
using System.Linq;
using System.Reflection;
using CommandDotNet.Builders;
using CommandDotNet.Extensions;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests.Extensions
{
    public class CustomAttributeContainerExtensionsTests
    {
        [Fact]
        public void GetCustomAttributes_ShouldReturnAll()
        {
            var attrProvider = new AttrProvider();
            attrProvider.CustomAttributes.Should().NotBeNull();
            attrProvider.GetCustomAttributes<MyAttribute>().Count().Should().Be(2);
        }

        [Fact]
        public void GetCustomAttribute_ShouldFail_WhenMoreThanOne_WithActionalError()
        {
            Assert.Throws<AppRunnerException>(
                    () => new AttrProvider().GetCustomAttribute<MyAttribute>())
                .Message.Should().Be("attempted to get a single MyAttribute from " +
                                     "CommandDotNet.Tests.UnitTests.Extensions.CustomAttributeContainerExtensionsTests" +
                                     "+AttrProvider but multiple exist");
        }

        private class AttrProvider : ICustomAttributesContainer
        {
            public ICustomAttributeProvider CustomAttributes { get; }

            public AttrProvider()
            {
                CustomAttributes = GetType()
                    .GetMethod(nameof(MyMethod), BindingFlags.NonPublic | BindingFlags.Instance)
                    !.GetParameters().Single();
            }

            private void MyMethod([My] [My] string myParam)
            {

            }
        }

        [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
        private class MyAttribute : Attribute
        {

        }

    }
}