using System;
using System.Linq;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests.Framework
{
    public class ServicesTests
    {
        [Fact]
        public void GivenNewInstance_GetAll_Should_ReturnEmpty()
        {
            new Services().GetAll().Should().BeEmpty();
        }

        [Fact]
        public void GivenAServiceIsAdded_GetAll_Should_ReturnTheAddedService()
        {
            var services = new Services();
            services.Add(this);
            var svcs = services.GetAll();
            svcs.Count.Should().Be(1);
            svcs.Single().Value.Should().Be(this);
        }

        [Fact]
        public void GivenAServiceIsAdded_Get_ForExistingType_Should_ReturnTheAddedService()
        {
            var services = new Services();
            services.Add(this);
            services.GetOrDefault<ServicesTests>().Should().Be(this);
        }

        [Fact]
        public void GivenAServiceIsAdded_GetWithType_ForExistingType_Should_ReturnTheAddedService()
        {
            var services = new Services();
            services.Add(this);
            services.GetOrDefault(this.GetType()).Should().Be(this);
        }

        [Fact]
        public void GivenAServiceIsAdded_Get_ForNonExistingType_Should_ReturnNull()
        {
            var services = new Services();
            services.Add(this);
            services.GetOrDefault(services.GetType()).Should().BeNull();
        }

        [Fact]
        public void GivenNewInstance_AddWithType_Should_AddService()
        {
            var services = new Services();
            services.GetOrDefault(this.GetType()).Should().BeNull();
            services.Add(this.GetType(), this);
            services.GetOrDefault(this.GetType()).Should().Be(this);
        }

        [Fact]
        public void GivenNewInstance_AddOrUpdate_Should_AddService()
        {
            var services = new Services();
            services.GetOrDefault(this.GetType()).Should().BeNull();
            services.AddOrUpdate(this);
            services.GetOrDefault(this.GetType()).Should().Be(this);
        }

        [Fact]
        public void GivenNewInstance_AddOrUpdateWithType_Should_AddService()
        {
            var services = new Services();
            services.GetOrDefault(this.GetType()).Should().BeNull();
            services.AddOrUpdate(this.GetType(), this);
            services.GetOrDefault(this.GetType()).Should().Be(this);
        }

        [Fact]
        public void GivenServiceOfTypeExists_Add_Should_ThrowException()
        {
            var services = new Services();
            services.Add(this);

            Assert.Throws<ArgumentException>(() => services.Add(this))
                .Message.Should().Be("service for type 'CommandDotNet.Tests.UnitTests.Framework.ServicesTests' " +
                                     "already exists 'CommandDotNet.Tests.UnitTests.Framework.ServicesTests'");
        }

        [Fact]
        public void GivenServiceOfTypeExists_AddOrUpdate_Should_UpdateService()
        {
            var uri1 = new Uri("http://google.com");
            var uri2 = new Uri("http://bing.com");

            var services = new Services();
            services.Add(uri1);
            services.GetOrDefault<Uri>().Should().Be(uri1);
            services.AddOrUpdate(uri2);
            services.GetOrDefault<Uri>().Should().Be(uri2);
        }

        [Fact]
        public void GivenServiceOfTypeExists_Add_UsingAnInterface_Should_AddService()
        {
            var operand = new Operand("lala", TypeInfo.Flag, ArgumentArity.ZeroOrOne);

            var services = new Services();
            services.Add(operand);
            services.Add<IArgument>(operand);

            var svcs = services.GetAll();
            svcs.Count.Should().Be(2);
            svcs.Should().Contain(kvp => kvp.Key == typeof(IArgument));
            svcs.Should().Contain(kvp => kvp.Key == typeof(Operand));
        }
    }
}