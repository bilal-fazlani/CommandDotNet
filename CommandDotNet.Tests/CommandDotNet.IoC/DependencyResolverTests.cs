using System;
using CommandDotNet.Builders;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.CommandDotNet.IoC
{
    public class DependencyResolverTests
    {
        private readonly IDependencyResolver _resolver = new TestDependencyResolver {new Password("lala")};

        [Fact]
        public void GenericResolve_GivenRegisteredType_ReturnsInstance()
        {
            _resolver.Resolve<Password>()!.GetPassword().Should().Be("lala");
        }

        [Fact]
        public void GenericTryResolve_GivenRegisteredType_ReturnsTrueAndInstance()
        {
            _resolver.TryResolve<Password>(out var password).Should().BeTrue();
            password!.GetPassword().Should().Be("lala");
        }

        [Fact]
        public void GenericTryResolve_GivenUnregisteredType_ReturnsTrueAndInstance()
        {
            _resolver.TryResolve<Uri>(out var password).Should().BeFalse();
            password.Should().BeNull();
        }

        [Fact]
        public void GenericResolveOrDefault_GivenRegisteredType_ReturnsInstance()
        {
            _resolver.ResolveOrDefault<Password>()!.GetPassword().Should().Be("lala");
        }

        [Fact]
        public void GenericResolveOrDefault_GivenUnregisteredType_ReturnsNull()
        {
            _resolver.ResolveOrDefault<Uri>().Should().BeNull();
        }
    }
}