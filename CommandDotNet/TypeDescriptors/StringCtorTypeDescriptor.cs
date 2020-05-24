using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;

namespace CommandDotNet.TypeDescriptors
{    public class StringCtorTypeDescriptor : IArgumentTypeDescriptor
    {
        private static readonly Dictionary<Type, Converter> Cache = new Dictionary<Type, Converter>();

        public bool CanSupport(Type type)
        {
            return GetConverter(type).MethodBase is { };
        }

        public string GetDisplayName(IArgument argument)
        {
            return GetConverter(argument).MethodBase.GetParameters().Single().Name;
        }

        public object ParseString(IArgument argument, string value)
        {
            var converter = GetConverter(argument);
            return converter.StringConstructor is { } 
                ? converter.StringConstructor.Invoke(new object[] { value }) 
                : converter.ParseMethod!.Invoke(null, new object[] { value });
        }

        private static Converter GetConverter(IArgument argument)
        {
            return argument.Arity.AllowsMany()
                ? GetConverter(argument.TypeInfo.UnderlyingType)
                : GetConverter(argument.TypeInfo.Type);
        }

        private static Converter GetConverter(Type type)
        {
            static bool HasSingleStringArgument(MethodBase method)
            {
                var parameterInfos = method.GetParameters();
                return parameterInfos.Length == 1 && parameterInfos.First().ParameterType == typeof(string);
            }
            
            return Cache.GetOrAdd(type, t =>
            {
                var stringCtor = t.GetConstructors()
                    .FirstOrDefault(HasSingleStringArgument);

                if (stringCtor is { })
                {
                    return new Converter{StringConstructor = stringCtor};
                }

                // intentionally skipping TryParse because we want the error message if parse fails.
                var parseMethod = t
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(c =>
                        c.Name == "Parse"
                        && HasSingleStringArgument(c));

                return new Converter{ParseMethod = parseMethod};
            });
        }

        private class Converter
        {
            public MethodBase? MethodBase => (MethodBase?)StringConstructor ?? ParseMethod;
            
            public ConstructorInfo? StringConstructor { get; set; }
            
            public MethodInfo? ParseMethod { get; set; }
        }
    }

}