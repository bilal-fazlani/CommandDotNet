﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet.ClassModeling
{
    internal static class BindValuesMiddleware
    {
        internal static Task<int> BindValues(CommandContext commandContext, ExecutionDelegate next)
        {
            var console = commandContext.Console;
            var parserFactory = new ParserFactory(commandContext.AppConfig.AppSettings);

            bool SetFromStringInput(IArgument argument, IEnumerable<string> values)
            {
                try
                {
                    var parser = parserFactory.CreateInstance(argument);
                    var value = parser.Parse(argument, values);
                    argument.Value = value;
                    return true;
                }
                catch (ValueParsingException ex)
                {
                    console.Error.WriteLine($"Failure parsing value for {argument}.  values={values?.ToCsv()}");
                    console.Error.WriteLine(ex.Message);
                    console.Error.WriteLine();
                    return false;
                }
            }

            var arguments = commandContext.InvocationPipeline.All
                .SelectMany(ic => ic.Command.Options.Cast<IArgument>().Union(ic.Command.Operands))
                .Where(a => a.GetArgumentDef() != null);

            var bindFailure = Task.FromResult(2);

            foreach (var argument in arguments)
            {
                if (argument.InputValues.Any())
                {
                    if (!SetFromStringInput(argument, argument.InputValues.SelectMany(iv => iv.Values)))
                    {
                        return bindFailure;
                    }
                }
                else if (argument.Default != null)
                {
                    var defaultValue = argument.Default.Value;
                    // middleware could set the default to any type.
                    // string values could be assigned from attributes or config values
                    // so they are parsed as if submitted from the shell.
                    switch (defaultValue)
                    {
                        case string stringValue:
                            if (!SetFromStringInput(argument, stringValue.ToEnumerable()))
                            {
                                return bindFailure;
                            }

                            break;
                        case IEnumerable<string> multiString:
                            if (!SetFromStringInput(argument, multiString))
                            {
                                return bindFailure;
                            }

                            break;
                        default:
                            try
                            {
                                var argumentType = argument.TypeInfo.Type;
                                if (argumentType.IsInstanceOfType(defaultValue))
                                {
                                    argument.Value = defaultValue;
                                }
                                else
                                {
                                    // cover cases like converting int to long.
                                    var convertedValue = Convert.ChangeType(defaultValue, argumentType);
                                    argument.Value = convertedValue;
                                }
                            }
                            catch (Exception ex)
                            {
                                console.Error.WriteLine(
                                    $"Failure assigning value to {argument}. Value={defaultValue}");
                                console.Error.WriteLine(ex.Message);
                                console.Error.WriteLine();
                                return bindFailure;
                            }

                            break;
                    }
                }
            }

            return next(commandContext);
        }
    }
}