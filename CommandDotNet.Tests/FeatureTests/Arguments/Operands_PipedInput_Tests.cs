using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_PipedInput_Tests
    {
        public Operands_PipedInput_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }
        
        [Fact]
        public void GivenPipedInputAndNoExplicitValue_PipedInputIsAppended()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.List)}",
                        PipedInput = new[] {"aaa", "bbb"}
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new List<string> {"aaa", "bbb"})
                    }
                });
        }

        [Fact]
        public void GivenPipedInputAndExplicitValue_AppendsPipedToExplicit()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.List)} aaa bbb",
                        PipedInput = new[] {"ccc", "ddd"}
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new List<string> {"aaa", "bbb", "ccc", "ddd"})
                    }
                });
        }

        [Fact]
        public void GivenNoListArg_PipedInputIsIgnored()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Single)} single",
                        PipedInput = new[] {"aaa"}
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("single")
                    }
                });
        }

        [Fact]
        public void GivenSingleAndListArg_AndNoArgValuesExplicitlyProvided_PipedInputAppendedToListArg()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.SingleAndList)} single",
                        PipedInput = new[] {"aaa", "bbb"}
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("single", new List<string> {"aaa", "bbb"})
                    }
                });
        }

        [Fact]
        public void GivenStreamingApp_PipedInput_IsOnlyEnumeratedWithinTheCommandMethod()
        {
            var stream = new PipedInputStream("aaa", "bbb");
            new AppRunner<StreamingApp>()
                .AppendPipedInputToOperandList()
                .Configure(c =>
                    c.UseMiddleware((context, next) =>
                        {
                            stream.EnumerationIsPremature = false;
                            return next(context);
                        },
                        MiddlewareStages.Invoke, -1))
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(StreamingApp.Stream)}",
                        PipedInput = stream
                    },
                    Then =
                    {
                        // stream is read-once so we can't evaluate it with a second time with ParamValuesShouldBe
                        AssertContext = ctx => ctx.GetCommandInvocationInfo<StreamingApp>()
                            .Instance!.StreamedInput.Should().BeEquivalentTo(new List<string> {"aaa", "bbb"})
                    }
                });
        }

        private class PipedInputStream : IEnumerable<string>
        {
            private readonly Queue<string> _queue;
            public bool EnumerationIsPremature = true;

            public PipedInputStream(params string[] inputs)
            {
                _queue = new Queue<string>(inputs);
            }

            public IEnumerator<string> GetEnumerator()
            {
                if (EnumerationIsPremature)
                    throw new Exception("premature enumeration");

                if (!_queue.Any())
                {
                    yield break;
                }

                yield return _queue.Dequeue();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class App
        {
            public void Single([Operand] string singleArg)
            {
            }

            public void List([Operand] List<string> listArgs)
            {
            }

            public void SingleAndList([Operand] string singleArg, [Operand] List<string> listArgs)
            {
            }
        }

        public class StreamingApp
        {
            public List<string> StreamedInput { get; set; } = null!;

            public void Stream(IEnumerable<string> input)
            {
                StreamedInput = input.ToList();
            }
        }
    }
}