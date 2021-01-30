using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommandDotNet.DataAnnotations;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.DataAnnotations
{
    public class DataAnnotationsValidationTests
    {
        /* Tests:
            - DisplayAttribute name replaced with argument name 
            - RequiredAttribute handled first
        */

        public DataAnnotationsValidationTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void Help_DoesNotIncludeValidation()
        {
            new AppRunner<App>()
                .UseDataAnnotationValidations()
                .Verify(new Scenario
                {
                    When = {Args = "Save -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Save [options] <Id> <Name>

Arguments:

  Id    <NUMBER>

  Name  <TEXT>

Options:

  --nick-name  <TEXT>

  --email      <TEXT>

  --role       <TEXT>
"
                    }
                });
        }

        [Fact]
        public void Exec_WithInvalidData_PrintsValidationError()
        {
            // includes tests for argument models & nested argument models
            new AppRunner<App>()
                .UseDataAnnotationValidations()
                .Verify(new Scenario
                {
                    When = {Args = "Save --role manager --email lala"},
                    Then =
                    {
                        ExitCode = 2,
                        Output = @"'role' must be a string or array type with a maximum length of '5'.
'Id' is required.
'Name' is required.
'email' is not a valid e-mail address.
"
                    }
                });
        }

        [Fact]
        public void Exec_Validates_InterceptorOptions()
        {
            // includes tests for argument models & nested argument models
            new AppRunner<Calc>()
                .UseDataAnnotationValidations()
                .Verify(new Scenario
                {
                    When = { Args = "Add 1 7" },
                    Then =
                    {
                        ExitCode = 2,
                        Output = @"'note' is required.
'y' must be between 1 and 5.
"
                    }
                });
        }

        [Fact]
        public void Exec_WithValidData_Succeeds()
        {
            new AppRunner<App>()
                .UseDataAnnotationValidations()
                .Verify(new Scenario
                {
                    When = {Args = "Save 1 john --email john@doe.com --role star"},
                    Then =
                    {
                        AssertContext = ctx =>
                            ctx.ParamValuesShouldBe(new Person
                            {
                                Id = 1,
                                Name = "john",
                                ContactInfo =
                                {
                                    Email = "john@doe.com"
                                }
                            }, "star")
                    }
                });
        }

        [Fact]
        public void Exec_IfShowHelpOnError_ShowsHelpAfterValidationMessage()
        {
            new AppRunner<App>()
                .UseDataAnnotationValidations(showHelpOnError: true)
                .Verify(new Scenario
                {
                    When = {Args = "Save"},
                    Then =
                    {
                        ExitCode = 2,
                        OutputContainsTexts =
                        {
                            @"'Id' is required.
'Name' is required.

Usage: dotnet testhost.dll Save [options] <Id> <Name>"
                        }
                    }
                });
        }

        [Fact]
        public void Exec_CanReplace_DisplayAttributeName_WithArgumentName()
        {
            new AppRunner<App>()
                .UseDataAnnotationValidations()
                .Verify(new Scenario
                {
                    When = { Args = "Save 1 john --nick-name bigbadjohn" },
                    Then =
                    {
                        ExitCode = 2,
                        Output = @"'nick-name' must be a string or array type with a maximum length of '5'.
"
                    }
                });
        }

        public class App
        {
            public void Save(
                Person person, 
                [Option, MaxLength(5)] string role)
            {
            }
        }

        public class Calc
        {
            public Task<int> Interceptor(InterceptorExecutionDelegate next, [Required] string note)
            {
                return next();
            }

            public int Add([Required] int? x, [Range(1, 5)] int? y)
            {
                return x.GetValueOrDefault() + y.GetValueOrDefault();
            }
        }

        public class Person : IArgumentModel
        {
            [Operand, Required, Range(1, int.MaxValue)]
            public int? Id { get; set; }
            
            [Operand, Required, MaxLength(5)] 
            public string Name { get; set; } = null!;

            [Option(LongName = "nick-name"), MaxLength(5), Display(Name="MyFriendsCallMe")]
            public string NickName { get; set; } = null!;

            [OrderByPositionInClass]
            public ContactInfo ContactInfo { get; set; } = new ContactInfo();
        }

        public class ContactInfo : IArgumentModel
        {
            [Option(LongName = "email"), EmailAddress] 
            public string Email { get; set; } = null!;
        }
    }
}
