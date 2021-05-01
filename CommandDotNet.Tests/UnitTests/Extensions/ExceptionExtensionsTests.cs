using CommandDotNet.ConsoleOnly;
using CommandDotNet.Diagnostics;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests.Extensions
{
    public class ExceptionExtensionsTests
    {
        private static readonly TestException TestEx = TestException.Instance;

        public class Print
        {
            [Fact]
            public void Default_prints_only_the_exception_message()
            {
                TestEx.Print().Should().Be(
                    "CommandDotNet.Tests.UnitTests.Extensions.TestException: I'm a test exception");
            }
            
            [Fact]
            public void When_IncludeProperties_output_includes_properties_except_those_defined_on_exception()
            {
                TestEx.Print(includeProperties:true).Should().Be(
                    @"CommandDotNet.Tests.UnitTests.Extensions.TestException: I'm a test exception
Properties:
  SomeProperty: Some property value");
            }
            
            [Fact]
            public void When_IncludeData_output_includes_values_from_Data_collection()
            {
                TestEx.Print(includeData:true).Should().Be(
                    @"CommandDotNet.Tests.UnitTests.Extensions.TestException: I'm a test exception
Data:
  data-key: data value");
            }
            
            [Fact]
            public void When_IncludeStackTrace_output_includes_stack_trace()
            {
                TestEx.Print(includeStackTrace:true).Should().StartWith(
                    @"CommandDotNet.Tests.UnitTests.Extensions.TestException: I'm a test exception
StackTrace:
  at CommandDotNet.Tests.UnitTests.Extensions.TestException.ThrowEx() in CommandDotNet.Tests/UnitTests/Extensions/TestException.cs:line 37
  at CommandDotNet.Tests.UnitTests.Extensions.TestException.Build() in CommandDotNet.Tests/UnitTests/Extensions/TestException.cs:line 23");
            }
            
            [Fact]
            public void When_include_all_output_includes_stack_trace()
            {
                TestEx.Print(includeProperties:true, includeData:true, includeStackTrace:true)
                    .Should().StartWith(
                    @"CommandDotNet.Tests.UnitTests.Extensions.TestException: I'm a test exception
Properties:
  SomeProperty: Some property value
Data:
  data-key: data value
StackTrace:
  at CommandDotNet.Tests.UnitTests.Extensions.TestException.ThrowEx() in CommandDotNet.Tests/UnitTests/Extensions/TestException.cs:line 37
  at CommandDotNet.Tests.UnitTests.Extensions.TestException.Build() in CommandDotNet.Tests/UnitTests/Extensions/TestException.cs:line 23");
            }

            [Fact]
            public void Honors_indent_when_provided()
            {
                TestEx.Print(indent: new Indent(depth: 2), includeProperties:true, includeData:true, includeStackTrace:true)
                    .Should().StartWith(
                        @"    CommandDotNet.Tests.UnitTests.Extensions.TestException: I'm a test exception
    Properties:
      SomeProperty: Some property value
    Data:
      data-key: data value
    StackTrace:
      at CommandDotNet.Tests.UnitTests.Extensions.TestException.ThrowEx() in CommandDotNet.Tests/UnitTests/Extensions/TestException.cs:line 37
      at CommandDotNet.Tests.UnitTests.Extensions.TestException.Build() in CommandDotNet.Tests/UnitTests/Extensions/TestException.cs:line 23");
            }

            [Fact]
            public void Prints_to_console_when_provided()
            {
                var testConsole = new TestConsole();
                TestEx.Print(testConsole, includeProperties: true, includeData: true, includeStackTrace: true);
                testConsole.AllText().Should().StartWith(
                    @"CommandDotNet.Tests.UnitTests.Extensions.TestException: I'm a test exception
Properties:
  SomeProperty: Some property value
Data:
  data-key: data value
StackTrace:
  at CommandDotNet.Tests.UnitTests.Extensions.TestException.ThrowEx() in CommandDotNet.Tests/UnitTests/Extensions/TestException.cs:line 37
  at CommandDotNet.Tests.UnitTests.Extensions.TestException.Build() in CommandDotNet.Tests/UnitTests/Extensions/TestException.cs:line 23");
            }
        }
    }
}