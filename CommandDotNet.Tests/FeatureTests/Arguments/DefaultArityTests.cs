using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class DefaultArityTests
    {
        private static readonly ArgumentArity ZeroToOne = new ArgumentArity(0, 1);
        private static readonly ArgumentArity OneToOne = new ArgumentArity(1, 1);
        private static readonly ArgumentArity ZeroToMany = new ArgumentArity(0, int.MaxValue);
        private static readonly ArgumentArity OneToMany = new ArgumentArity(1, int.MaxValue);

        public DefaultArityTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void MethodParameters_WithoutDefaults_AreRequired_UnlessNullable()
        {
            var operands = GetOperands(nameof(App.Params));

            operands!.Boolean.Arity.Should().Be(OneToOne);
            operands.NullableBoolean.Arity.Should().Be(ZeroToOne);
            operands.Number.Arity.Should().Be(OneToOne);
            operands.NullableNumber.Arity.Should().Be(ZeroToOne);
            operands.Text.Arity.Should().Be(OneToOne);
            operands.Uri.Arity.Should().Be(OneToOne);
            operands.Texts.Arity.Should().Be(OneToMany);
        }

        [Fact]
        public void MethodParameters_WithDefaults_AreNotRequired()
        {
            var operands = GetOperands(nameof(App.DefaultParams));

            operands!.Boolean.Arity.Should().Be(ZeroToOne);
            operands.NullableBoolean.Arity.Should().Be(ZeroToOne);
            operands.Number.Arity.Should().Be(ZeroToOne);
            operands.NullableNumber.Arity.Should().Be(ZeroToOne);
            operands.Text.Arity.Should().Be(ZeroToOne);
            operands.Uri.Arity.Should().Be(ZeroToOne);
            operands.Texts.Arity.Should().Be(ZeroToMany);
        }

        [Fact]
        public void ModelProperties_WithoutDefaults_AreRequired_UnlessNullable()
        {
            var operands = GetOperands(nameof(App.Model));

            operands!.Boolean.Arity.Should().Be(OneToOne);
            operands.NullableBoolean.Arity.Should().Be(ZeroToOne);
            operands.Number.Arity.Should().Be(OneToOne);
            operands.NullableNumber.Arity.Should().Be(ZeroToOne);
            operands.Text.Arity.Should().Be(OneToOne);
            operands.Uri.Arity.Should().Be(OneToOne);
            operands.Texts.Arity.Should().Be(OneToMany);
        }

        [Fact]
        public void ModelProperties_WithDefaults_OfNonDefaultValues_AreNotRequired()
        {
            var operands = GetOperands(nameof(App.Model), new TestDependencyResolver
            {
                new ArgModel
                {
                    Boolean = true,
                    NullableBoolean = true,
                    Number = 1,
                    NullableNumber = 1,
                    Text = "lala",
                    Uri = new Uri("http://google.com"),
                    Texts = new[] {"one"}
                }
            });

            operands!.Boolean.Arity.Should().Be(ZeroToOne);
            operands.NullableBoolean.Arity.Should().Be(ZeroToOne);
            operands.Number.Arity.Should().Be(ZeroToOne);
            operands.NullableNumber.Arity.Should().Be(ZeroToOne);
            operands.Text.Arity.Should().Be(ZeroToOne);
            operands.Uri.Arity.Should().Be(ZeroToOne);
            operands.Texts.Arity.Should().Be(ZeroToMany);
        }

        [Fact]
        public void ModelProperties_WithDefaults_OfDefaultValues_AreRequired_UnlessNullable()
        {
            var operands = GetOperands(nameof(App.Model), new TestDependencyResolver
            {
                new ArgModel
                {
                    Boolean = false,
                    NullableBoolean = null,
                    Number = 0,
                    NullableNumber = null,
                    Text = null,
                    Uri = null,
                    Texts = null
                }
            });

            operands!.Boolean.Arity.Should().Be(OneToOne);
            operands.NullableBoolean.Arity.Should().Be(ZeroToOne);
            operands.Number.Arity.Should().Be(OneToOne);
            operands.NullableNumber.Arity.Should().Be(ZeroToOne);
            operands.Text.Arity.Should().Be(OneToOne);
            operands.Uri.Arity.Should().Be(OneToOne);
            operands.Texts.Arity.Should().Be(OneToMany);
        }

        private Operands? GetOperands(string methodName, IDependencyResolver? dependencyResolver = null)
        {
            var appRunner = new AppRunner<App>();
            if (dependencyResolver != null)
            {
                appRunner.UseDependencyResolver(dependencyResolver);
            }
            return appRunner.GetFromContext(methodName.SplitArgs(),
                    ctx => Operands.FromCommand(ctx.ParseResult!.TargetCommand),
                    middlewareStage: MiddlewareStages.PostParseInputPreBindValues);
        }

        class App
        {
            public void Params(
                bool boolean, bool? nullableBoolean,
                int number, int? nullableNumber,
                string text, Uri uri,
                IEnumerable<string> texts)
            { }

            public void DefaultParams(
                bool boolean = true, bool? nullableBoolean = null,
                int number = 1, int? nullableNumber = null,
                string? text = null, Uri? uri = null,
                IEnumerable<string>? texts = null)
            { }

            public void Model(ArgModel model) { }
        }

        class ArgModel : IArgumentModel
        {
            [Operand]
            public bool Boolean { get; set; }
            [Operand]
            public bool? NullableBoolean { get; set; }
            [Operand]
            public int Number { get; set; }
            [Operand]
            public int? NullableNumber { get; set; }
            [Operand]
            public string? Text { get; set; }
            [Operand]
            public Uri? Uri { get; set; }
            [Operand]
            public IEnumerable<string>? Texts { get; set; }
        }

        class Operands
        {
            public Operand Boolean { get; }
            public Operand NullableBoolean { get; }
            public Operand Number { get; }
            public Operand NullableNumber { get; }
            public Operand Text { get; }
            public Operand Uri { get; }
            public Operand Texts { get; }

            public static Operands FromCommand(Command command)
            {
                var operands = command.Operands.ToArray();
                return new Operands(operands[0], operands[1],
                    operands[2], operands[3],
                    operands[4], operands[5],
                    operands[6]);
            }

            public Operands(
                Operand boolean, Operand nullableBoolean, 
                Operand number, Operand nullableNumber, 
                Operand text, Operand uri,
                Operand texts)
            {
                Boolean = boolean;
                NullableBoolean = nullableBoolean;
                Number = number;
                NullableNumber = nullableNumber;
                Text = text;
                Uri = uri;
                Texts = texts;
            }
        }
    }
}