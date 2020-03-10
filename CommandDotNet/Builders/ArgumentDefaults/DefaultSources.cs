﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.Builders.ArgumentDefaults
{
    public static class DefaultSources
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static class EnvVar
        {
            public static IEnumerable<string> GetKeyFromAttribute(IArgument argument)
            {
                var key = argument.CustomAttributes.GetCustomAttribute<EnvVarAttribute>()?.Key;
                if (key != null)
                {
                    yield return key;
                }
            }

            public static Func<IArgument, DefaultValue> GetDefaultValue(
                IDictionary envVars = null,
                params GetArgumentKeysDelegate[] getKeysDelegates)
            {
                envVars = envVars ?? Environment.GetEnvironmentVariables();
                getKeysDelegates = getKeysDelegates ?? new GetArgumentKeysDelegate[]
                {
                    GetKeyFromAttribute
                };
                return GetValue(
                    "EnvVar", 
                    getKeysDelegates, 
                    key => envVars.Contains(key) ? (string) envVars[key] : null);
            }
        }


        public static class AppSetting
        {
            public static IEnumerable<string> GetKeyFromAttribute(IArgument argument)
            {
                var key = argument.CustomAttributes.GetCustomAttribute<AppSettingAttribute>()?.Key;
                if (key != null)
                {
                    yield return key;
                }
            }

            public static IEnumerable<string> GetKeysFromConvention(IArgument argument)
            {
                IEnumerable<string> GetOptionKeys(Option option)
                {
                    if (!option.LongName.IsNullOrWhitespace()) yield return $"--{option.LongName}";
                    if (option.ShortName.HasValue) yield return $"-{option.ShortName}";
                }

                var keys = argument.SwitchFunc(o => o.Name.ToEnumerable(), GetOptionKeys);
                foreach (var key in keys)
                {
                    yield return $"{argument.Parent.Name} {key}";
                    yield return key;
                }
            }

            public static Func<IArgument, DefaultValue> GetDefaultValue(
                NameValueCollection appSettings,
                params GetArgumentKeysDelegate[] getKeysCallbacks)
            {
                getKeysCallbacks = getKeysCallbacks ?? new GetArgumentKeysDelegate[]
                {
                    GetKeyFromAttribute
                };
                return GetValue(
                    "AppSetting",
                    getKeysCallbacks, 
                    key => appSettings[key]);
            }
        }

        private static Func<IArgument, DefaultValue> GetValue(
            string sourceName,
            GetArgumentKeysDelegate[] getKeys, 
            Func<string, string> getValueFromSource)
        {
            return argument =>
            {
                var value = getKeys
                    .SelectMany(cb => cb(argument))
                    .Select(key => (key, value:getValueFromSource(key)))
                    .Where(v => v.value != null)
                    .Select(v =>
                    {
                        object f = argument.Arity.AllowsMany()
                            ? v.value.Split(',')
                            : (object)v.value;
                        return new DefaultValue(sourceName, v.key, f);
                    })
                    .FirstOrDefault();
                
                if (value != null)
                {
                    // do not include value in case it's a password
                    Log.Debug($"found default value `{value}` for `{argument}`");
                }

                return value;
            };
        }
    }
}