using System;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using CommandDotNet.TypeDescriptors;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CustomArgumentTypeDescriptorTests
    {
        private static readonly AppSettings BasicHelpWithDescriptor = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelpWithDescriptor = TestAppSettings.DetailedHelp;

        public CustomArgumentTypeDescriptorTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
            var descriptor = new SquareTypeDescriptor();
            BasicHelpWithDescriptor.ArgumentTypeDescriptors.Add(descriptor);
            DetailedHelpWithDescriptor.ArgumentTypeDescriptors.Add(descriptor);
        }

        [Fact]
        public void BasicHelp_IncludesParam()
        {
            new AppRunner<App>(BasicHelpWithDescriptor).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do <square>

Arguments:
  square
"
                }
            });
        }

        [Fact]
        public void DetailedHelp_IncludesParamAndDisplayNameFromDescriptor()
        {
            new AppRunner<App>(DetailedHelpWithDescriptor).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do <square>

Arguments:

  square  <!!SQUARE!!>
"
                }
            });
        }

        [Fact]
        public void Exec_ParseArgumentUsingDescriptor()
        {
            new AppRunner<App>(BasicHelpWithDescriptor).Verify(new Scenario
            {
                When = {Args = "Do 2x3"},
                Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(new Square {Length = 2, Width = 3})}
            });
        }

        [Fact]
        public void Exec_WhenDescriptorIsNotRegistered_FailsWithActionableMessage()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Do "},
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts =
                    {
                        "type : CommandDotNet.Tests.FeatureTests.CustomArgumentTypeDescriptorTests+Square is not supported.",
                        "If it is an argument model, inherit from IArgumentModel.",
                        "Otherwise, to support this type, implement a TypeConverter or IArgumentTypeDescriptor or add a constructor with a single string parameter."
                    }
                }
            });
        }

        private class App
        {
            public void Do(Square square)
            {
            }
        }

        public class Square
        {
            public int Length { get; set; }
            public int Width { get; set; }

            public static Square Parse(string value)
            {
                var parts = value.Split('x');
                var length = int.Parse(parts[0]);
                var width = int.Parse(parts[1]);
                return new Square { Length = length, Width = width };
            }
        }

        public class SquareTypeDescriptor : IArgumentTypeDescriptor
        {
            public bool CanSupport(Type type)
            {
                return type == typeof(Square);
            }

            public string GetDisplayName(IArgument argument)
            {
                return $"!!{nameof(Square)}!!";
            }

            public object ParseString(IArgument argument, string value)
            {
                return Square.Parse(value);
            }
        }
    }
}
