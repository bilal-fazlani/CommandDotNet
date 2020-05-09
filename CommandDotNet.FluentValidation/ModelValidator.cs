using System;
using System.Reflection;
using CommandDotNet.Builders;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Results;

namespace CommandDotNet.FluentValidation
{
    internal class ModelValidator
    {
        // TODO: move FluentValidation into a separate repo & nuget package?
        //       there are other ways to do validation that could also
        //       be applied to parameters
        private readonly IDependencyResolver? _dependencyResolver;

        internal ModelValidator(IDependencyResolver? dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        internal ValidationResult? ValidateModel(IArgumentModel model)
        {
            Type modelType = model.GetType();

            Type? declaredValidatorType = modelType.GetCustomAttribute<ValidatorAttribute>()?.ValidatorType;
            
            if (declaredValidatorType is { })
            {
                object? validator;   
                try
                {
                    if (_dependencyResolver is null || !_dependencyResolver.TryResolve(declaredValidatorType, out validator))
                    {
                        validator = Activator.CreateInstance(declaredValidatorType);
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidValidatorException($"Could not create instance of {declaredValidatorType.Name}. Please ensure it's injected via IoC or has a default constructor.\n" +
                                                 "This exception could also occur if default constructor threw an exception", e);
                }

                return ((IValidator)validator!).Validate(model);
            }

            return null;
        }
    }
}