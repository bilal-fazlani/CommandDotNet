﻿using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Models;
using CommandDotNet.Parsing;

namespace CommandDotNet
{
    internal class ValueMachine
    {
        private readonly AppSettings _appSettings;
        private ParserFactory _parserFactory;

        public ValueMachine(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _parserFactory = new ParserFactory(_appSettings);
        }

        public object GetValue(ArgumentInfo argumentInfo)
        {
            PromptForValue(argumentInfo);
            
            //when user has provided a value
            if (argumentInfo.ValueInfo.HasValue && argumentInfo.ValueInfo.Value != null)
            {
                //parse value
                IParser parser = _parserFactory.CreateInstance(argumentInfo);
                return parser.Parse(argumentInfo);
            }

            //when value not present but method parameter has a default value defined
            if (argumentInfo.DefaultValue != DBNull.Value && argumentInfo.DefaultValue != null)
            {
                //use default paramter or property value
                return argumentInfo.DefaultValue;
            }

            //when there no value from inut and no default value, return default value of the type
            return argumentInfo.Type.GetDefaultValue();
        }

        private void PromptForValue(ArgumentInfo argumentInfo)
        {
            if (!_appSettings.PrompForArgumentsIfNotProvided
                || !(argumentInfo is CommandParameterInfo parameterInfo)
                || parameterInfo.ValueInfo.HasValue
                || parameterInfo.DefaultValue != DBNull.Value)
            {
                return;
            }

            List<string> inputs = new List<string>();
            if (parameterInfo.IsMultipleType)
            {
                Console.Write($"{parameterInfo.Name} ({parameterInfo.TypeDisplayName}) [separate values by space]: ");
                inputs = Console.ReadLine().Split(' ').ToList();
            }
            else
            {
                Console.Write($"{parameterInfo.Name} ({parameterInfo.TypeDisplayName}): ");
                inputs.Add(Console.ReadLine());
            }

            parameterInfo.ValueInfo.Values = inputs;
        }
    }
}