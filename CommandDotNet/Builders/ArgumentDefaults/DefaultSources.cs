using System;
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
                var key = argument.GetCustomAttribute<EnvVarAttribute>()?.Key;
                if (key != null)
                {
                    yield return key;
                }
            }

            public static Func<IArgument, ArgumentDefault?> GetDefaultValue(
                IDictionary? envVars = null,
                params GetArgumentKeysDelegate[] getKeysDelegates)
            {
                envVars ??= Environment.GetEnvironmentVariables();
                getKeysDelegates ??= new GetArgumentKeysDelegate[]
                {
                    GetKeyFromAttribute
                };
                return GetValue(
                    "EnvVar", 
                    getKeysDelegates, 
                    key => envVars.Contains(key) ? (string)envVars[key] : null);
            }
        }


        public static class AppSetting
        {
            public static IEnumerable<string> GetKeyFromAttribute(IArgument argument)
            {
                var key = argument.GetCustomAttribute<AppSettingAttribute>()?.Key;
                if (key != null)
                {
                    yield return key;
                }
            }

            public static IEnumerable<string> GetKeysFromConvention(IArgument argument)
            {
                static IEnumerable<string> GetOptionKeys(Option option)
                {
                    if (!option.LongName.IsNullOrWhitespace()) yield return $"--{option.LongName}";
                    if (option.ShortName.HasValue) yield return $"-{option.ShortName}";
                }

                var keys = argument.SwitchFunc(o => o.Name.ToEnumerable(), GetOptionKeys);
                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        yield return $"{argument.Parent!.Name} {key}";
                        yield return key;
                    }
                }
            }

            public static Func<IArgument, ArgumentDefault?> GetDefaultValue(
                NameValueCollection appSettings,
                params GetArgumentKeysDelegate[] getKeysCallbacks)
            {
                getKeysCallbacks ??= new GetArgumentKeysDelegate[]
                {
                    GetKeyFromAttribute
                };
                return GetValue(
                    "AppSetting",
                    getKeysCallbacks, 
                    key => appSettings[key]);
            }
        }

        private static Func<IArgument, ArgumentDefault?> GetValue(
            string sourceName,
            GetArgumentKeysDelegate[] getKeys,
            Func<string, string?> getValueFromSource)
        {
            return argument =>
            {
                var value = getKeys
                    .SelectMany(cb => cb(argument))
                    .Select(key => (key, value:getValueFromSource(key)))
                    .Where(v => v.value != null)
                    .Select(v =>
                    {
                        object defaultValue = argument.Arity.AllowsMany()
                            ? v.value!.Split(',')
                            : (object)v.value!;
                        return new ArgumentDefault(sourceName, v.key, defaultValue);
                    })
                    .FirstOrDefault();

                if (value != null)
                {
                    if (Log.IsDebugEnabled())
                    {
                        Log.DebugFormat("default value found in {1} for {0}: {2}={3}",
                            argument,
                            value.Source,
                            value.Key,
                            value.Value.ValueToString(argument.IsObscured())
                        );
                    }
                }
                else
                {
                    Log.DebugFormat("default value not found in `{1}` for {0}", argument, sourceName);
                }

                return value;
            };
        }
    }
}