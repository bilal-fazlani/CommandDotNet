using System;
using CommandDotNet.Extensions;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests.Extensions
{
    public class ArgumentExtensionTests
    {
        private static readonly Option AnOption;
        private static readonly Operand AnOperand;

        static ArgumentExtensionTests()
        {
            var command = new Command("cmd");
            AnOption = new Option("option", null, TypeInfo.Flag, ArgumentArity.ExactlyOne);
            AnOperand = new Operand("operand", TypeInfo.Single<int>(), ArgumentArity.ExactlyOne);
        }

        [Fact]
        public void SwitchAct_ForNullArg_Should_ThrowNullRef()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ((IArgument) null!).SwitchAct(
                    o => Assert.True(false, "operandAction should not be called for operand"),
                    o => Assert.True(false, "optionAction should not be called for operand"))
            );
        }

        [Fact]
        public void SwitchAct_ForOperand_Should_ExecuteOnlyOperandAction()
        {
            bool actionCalled = false;

            AnOperand.SwitchAct(
                o => actionCalled = true,
                o => Assert.True(false, "optionAction should not be called for operand"));

            actionCalled.Should().BeTrue();
        }

        [Fact]
        public void SwitchAct_ForOption_Should_ExecuteOnlyOptionAction()
        {
            bool actionCalled = false;

            AnOption.SwitchAct(
                o => Assert.True(false, "operandAction should not be called for option"),
                o => actionCalled = true);

            actionCalled.Should().BeTrue();
        }

        [Fact]
        public void SwitchFunc_ForNullArg_Should_ThrowNullRef()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ((IArgument)null!).SwitchFuncStruct(
                    o => 0,
                    o => 0)
            );
        }

        [Fact]
        public void SwitchFunc_ForOperand_Should_ExecuteOnlyOperandFuncion()
        {
            var result = AnOperand.SwitchFuncStruct(
                o => 1,
                o => 0);

            result.Should().Be(1);
        }

        [Fact]
        public void SwitchFunc_ForOption_Should_ExecuteOnlyOptionFuncion()
        {
            var result = AnOption.SwitchFuncStruct(
                o => 0,
                o => 1);

            result.Should().Be(1);
        }
    }
}