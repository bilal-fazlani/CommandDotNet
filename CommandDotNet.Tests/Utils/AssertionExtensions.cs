using System;
using CommandDotNet.TestTools;
using FluentAssertions;
using FluentAssertions.Collections;

namespace CommandDotNet.Tests.Utils
{
    public static class AssertionExtensions
    {
        /// <summary>Should be same order</summary>
        public static AndConstraint<StringCollectionAssertions> BeEquivalentSequenceTo(
            this StringCollectionAssertions assertions,
            params string[] expectedValues)
        {
            return assertions.BeEquivalentTo(expectedValues, c => c.WithStrictOrderingFor(s => s));
        }

        /// <summary>Should be same order</summary>
        public static AndConstraint<GenericCollectionAssertions<object?>> BeEquivalentSequenceTo(
            this GenericCollectionAssertions<object?> assertions,
            params object?[] expectedValues)
        {
            return assertions.BeEquivalentTo(expectedValues, c => c.WithStrictOrderingFor(s => s));
        }

        /// <summary>Being explicit brings clarity in tests</summary>
        public static void ParamValuesShouldBeEmpty(this CommandContext ctx) => ctx.ParamValuesShouldBe();

        public static void ParamValuesShouldBe(this CommandContext ctx, params object?[] values)
        {
            var invocation = ctx.GetCommandInvocationInfo();
            InvocationParamValuesShouldBe(values, invocation);
        }

        /// <summary>Being explicit brings clarity in tests</summary>
        public static void ParamValuesShouldBeEmpty<TInterceptorClass>(this CommandContext ctx) 
            where TInterceptorClass : class => 
            ctx.ParamValuesShouldBe<TInterceptorClass>();

        public static void ParamValuesShouldBe<TInterceptorClass>(this CommandContext ctx, params object[] values)
            where TInterceptorClass : class
        {
            var invocation = ctx.GetInterceptorInvocationInfo<TInterceptorClass>();
            InvocationParamValuesShouldBe(values, invocation);
        }

        private static void InvocationParamValuesShouldBe(object?[] values, InvocationInfo invocation)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            invocation.ArgumentParameterValues?.Should().BeEquivalentSequenceTo(values);
        }
    }
}