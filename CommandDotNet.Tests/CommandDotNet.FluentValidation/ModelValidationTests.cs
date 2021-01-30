using CommandDotNet.Execution;
using CommandDotNet.FluentValidation;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentValidation;
using FluentValidation.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.FluentValidation
{
    public class ModelValidationTests
    {
        public ModelValidationTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void Help_DoesNotIncludeValidation()
        {
            new AppRunner<App>()
                .UseFluentValidation()
                .Verify(new Scenario
                {
                    When = {Args = "Save -h"},
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Save <Id> <Name> <Email>

Arguments:

  Id     <NUMBER>

  Name   <TEXT>

  Email  <TEXT>
"
                    }
                });
        }

        [Fact]
        public void Exec_WithInvalidData_PrintsValidationError()
        {
            new AppRunner<App>()
                .UseFluentValidation()
                .Verify(new Scenario
                {
                    When = {Args = "Save"},
                    Then =
                    {
                        ExitCode = 2,
                        Output = @"'Person' is invalid
  'Id' must be greater than '0'.
  'Name' should not be empty.
  'Email' should not be empty.
"
                    }
                });
        }

        [Fact]
        public void Exec_WithInvalidData_PrintsValidationError_UsingValidatorFromDI()
        {
            new AppRunner<App>()
                .UseFluentValidation()
                .UseDependencyResolver(
                    new TestDependencyResolver{new PersonValidator()}, 
                    commandClassResolveStrategy: ResolveStrategy.TryResolve)
                .Verify(new Scenario
                {
                    When = {Args = "Save"},
                    Then =
                    {
                        ExitCode = 2,
                        Output = @"'Person' is invalid
  'Id' must be greater than '0'.
  'Name' should not be empty.
  'Email' should not be empty.
"
                    }
                });
        }

        [Fact]
        public void Exec_WithValidData_Succeeds()
        {
            new AppRunner<App>()
                .UseFluentValidation()
                .UseDependencyResolver(
                    new TestDependencyResolver { new PersonValidator() },
                    commandClassResolveStrategy: ResolveStrategy.TryResolve)
                .Verify(new Scenario
                {
                    When = {Args = "Save 1 john john@doe.com"},
                    Then = {AssertContext = ctx => 
                            ctx.ParamValuesShouldBe(new Person { Id = 1, Name = "john", Email = "john@doe.com" }) }
                });
        }

        [Fact]
        public void Exec_WhenNoDependencyResolver_ValidatorIsCreated()
        {
            new AppRunner<App>()
                .UseFluentValidation()
                .Verify(new Scenario
                {
                    When = {Args = "Save"},
                    Then =
                    {
                        ExitCode = 2,
                        OutputContainsTexts = {"'Person' is invalid"}
                    }
                });
        }

        [Fact]
        public void Exec_WhenValidatorNotRegistered_ValidatorIsCreated()
        {
            new AppRunner<App>()
                .UseFluentValidation()
                .UseDependencyResolver(
                    new TestDependencyResolver(),
                    commandClassResolveStrategy: ResolveStrategy.TryResolve)
                .Verify(new Scenario
                {
                    When = {Args = "Save"},
                    Then =
                    {
                        ExitCode = 2,
                        OutputContainsTexts = {"'Person' is invalid"}
                    }
                });
        }

        [Fact]
        public void Exec_WhenInvalidValidator_PrintsError()
        {
            new AppRunner<App>()
                .UseFluentValidation()
                .UseDependencyResolver(
                    new TestDependencyResolver(),
                    commandClassResolveStrategy: ResolveStrategy.TryResolve)
                .Verify(new Scenario
                {
                    When = {Args = "InvalidSave"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts =
                        {
                            "CommandDotNet.FluentValidation.InvalidValidatorException: Could not create instance of InvalidPersonValidator. " +
                            "Please ensure it's injected via IoC or has a default constructor.",
                            "This exception could also occur if default constructor threw an exception",
                            // assert stack trace is printed
                            " ---> System.MissingMethodException: No parameterless constructor defined for type " +
                            "'CommandDotNet.Tests.CommandDotNet.FluentValidation.ModelValidationTests+InvalidPersonValidator'"
                        }
                    }
                });
        }

        [Fact]
        public void Exec_IfShowHelpOnError_ShowsHelpAfterValidationMessage()
        {
            new AppRunner<App>()
                .UseFluentValidation(showHelpOnError: true)
                .Verify(new Scenario
                {
                    When = {Args = "Save"},
                    Then =
                    {
                        ExitCode = 2,
                        OutputContainsTexts =
                        {
                            @"'Person' is invalid
  'Id' must be greater than '0'.
  'Name' should not be empty.
  'Email' should not be empty.

Usage: dotnet testhost.dll Save <Id> <Name> <Email>"
                        }
                    }
                });
        }

        public class App
        {
            public void Save(Person person)
            {
            }

            public void InvalidSave(InvalidPerson person)
            {
            }
        }

        [Validator(typeof(PersonValidator))]
        public class Person : IArgumentModel
        {
            [Operand] public int Id { get; set; }
            [Operand] public string Name { get; set; } = null!;
            [Operand] public string Email { get; set; } = null!;
        }

        public class PersonValidator : AbstractValidator<Person>
        {
            public PersonValidator()
            {
                RuleFor(x => x.Id).GreaterThan(0);
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
            }
        }

        [Validator(typeof(InvalidPersonValidator))]
        public class InvalidPerson : IArgumentModel
        {
            [Operand]
            public int Id { get; set; }
        }

        public class InvalidPersonValidator : AbstractValidator<Person>
        {
            public InvalidPersonValidator(bool nonDefaultCtor)
            {
                RuleFor(x => x.Id).GreaterThan(0);
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
            }
        }
    }
}
