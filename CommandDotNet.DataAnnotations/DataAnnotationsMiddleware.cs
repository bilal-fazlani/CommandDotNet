using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.ConsoleOnly;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.DataAnnotations
{
    public static class DataAnnotationsMiddleware
    {
        public static AppRunner UseDataAnnotationValidations(this AppRunner appRunner, bool showHelpOnError = false)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(DataAnnotationsValidation, MiddlewareSteps.DataAnnotations);
                c.Services.Add(new Config(showHelpOnError));
            });
        }

        private class Config
        {
            public bool ShowHelpOnError { get; }

            public Config(bool showHelpOnError)
            {
                ShowHelpOnError = showHelpOnError;
            }
        }

        private static Task<int> DataAnnotationsValidation(CommandContext ctx, ExecutionDelegate next)
        {
            var errors = ctx.InvocationPipeline.All
                .SelectMany(ValidateStep)
                .ToList();

            if (errors.Any())
            {
                var console = ctx.Console;
                errors.ForEach(error =>
                {
                    console.Error.WriteLine(error);
                });

                ctx.ShowHelpOnExit = ctx.AppConfig.Services.GetOrThrow<Config>().ShowHelpOnError;

                if (ctx.ShowHelpOnExit)
                {
                    console.Error.WriteLine();
                }

                return ExitCodes.ValidationError;
            }
            
            return next(ctx);
        }

        private static IEnumerable<string> ValidateStep(InvocationStep step)
        {
            var propertyArgumentErrors = step.Invocation.Arguments
                .Select(GetParameterArgumentErrors);

            var argumentModelErrors = step.Invocation.FlattenedArgumentModels
                .Select(m => GetArgumentModelErrors(m, step.Invocation.Arguments));

            return propertyArgumentErrors
                .Concat(argumentModelErrors)
                .SelectMany(e => e);
        }

        private static IEnumerable<string> GetArgumentModelErrors(IArgumentModel model, IReadOnlyCollection<IArgument> arguments)
        {
            var validationContext = new ValidationContext(model);
            var results = new List<ValidationResult>();
            if (Validator.TryValidateObject(model, validationContext, results, validateAllProperties: true))
            {
                return Enumerable.Empty<string>();
            }

            IArgument? GetArgument(string propertyName)
            {
                return arguments.FirstOrDefault(a =>
                {
                    var info = a.Services.GetOrDefault<PropertyInfo>();
                    return info != null
                           && info.DeclaringType.IsInstanceOfType(model)
                           && propertyName.Equals(info.Name);
                });
            }

            string SanitizedErrorMessage(ValidationResult validationResult)
            {
                var errorMessage = validationResult.ErrorMessage;
                foreach (var memberName in validationResult.MemberNames)
                {
                    var argument = GetArgument(memberName);
                    if (argument is { })
                    {
                        errorMessage = errorMessage.RemoveFieldTerminology(memberName, argument);
                    }
                }

                return errorMessage;
            }

            return results.Select(SanitizedErrorMessage);
        }

        private static IEnumerable<string> GetParameterArgumentErrors(IArgument argument)
        {
            var parameterInfo = argument.Services.GetOrDefault<ParameterInfo>();
            if (parameterInfo is null)
            {
                return Enumerable.Empty<string>();
            }
            
            var validationAttributes = argument.CustomAttributes
                .GetCustomAttributes(true)
                .OfType<ValidationAttribute>()
                .OrderBy(a => a is RequiredAttribute ? 0 : 1)
                .ToList();
            
            List<string>? errors = null;

            foreach (var validationAttribute in validationAttributes)
            {
                if (!validationAttribute.IsValid(argument.Value))
                {
                    // the user expects the name to map to an argument, not a field.
                    // update the terminology. This is naive and will need to change
                    // when we handle localization
                    var message = validationAttribute
                        .FormatErrorMessage(argument.Name)
                        .RemoveFieldTerminology(parameterInfo.Name, argument);
                    (errors ??= new List<string>()).Add(message);

                    if (validationAttribute is RequiredAttribute)
                    {
                        // If the value is not provided and it is required, no other validation needs to be performed.
                        // this is why the RequiredAttribute is first.
                        break;
                    }
                }
            }
            
            return errors ?? Enumerable.Empty<string>();
        }

        /// <summary>the user expects the name to map to an argument, not a field.</summary>
        /// <remarks>
        /// This is naive and will need to change when we handle localization
        /// </remarks>
        private static string RemoveFieldTerminology(this string error, string memberName, IArgument argument)
        {
            memberName = argument.GetCustomAttribute<DisplayAttribute>()?.Name ?? memberName;
            return error
                .Replace($"The {memberName} field", $"'{argument.Name}'")
                .Replace($"The field {memberName}", $"'{argument.Name}'")
                .Replace($"The {memberName} property", $"'{argument.Name}'")
                .Replace($"The property {memberName}", $"'{argument.Name}'");
        }
    }
}