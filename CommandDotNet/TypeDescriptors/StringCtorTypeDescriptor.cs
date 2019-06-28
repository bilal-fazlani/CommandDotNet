using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;
using CommandDotNet.Models;

namespace CommandDotNet.TypeDescriptors
{
    public class StringCtorTypeDescriptor : IArgumentTypeDescriptor
    {
        private static readonly Dictionary<Type, Converter> Cache = new Dictionary<Type, Converter>();

        public bool CanSupport(Type type)
        {
            return GetConverter(type).CanConvert;
        }

        public string GetDisplayName(ArgumentInfo argumentInfo)
        {
            return GetConverter(argumentInfo).StringConstructor.GetParameters().Single().Name;
        }

        public object ParseString(ArgumentInfo argumentInfo, string value)
        {
            return GetConverter(argumentInfo).StringConstructor.Invoke(new object[]{ value });
        }

        private static Converter GetConverter(ArgumentInfo argumentInfo)
        {
            return argumentInfo.Arity.AllowsZeroOrMore()
                ? GetConverter(argumentInfo.UnderlyingType)
                : GetConverter(argumentInfo.Type);
        }

        private static Converter GetConverter(Type type)
        {
            return Cache.GetOrAdd(type, t =>
            {
                var stringCtor = t.GetConstructors().FirstOrDefault(c =>
                {
                    var parameterInfos = c.GetParameters();
                    return parameterInfos.Length == 1 && parameterInfos.First().ParameterType == typeof(string);
                });

                return new Converter(stringCtor);
            });
        }

        private class Converter
        {
            public bool CanConvert => StringConstructor != null;
            public ConstructorInfo StringConstructor { get; }

            public Converter(ConstructorInfo stringConstructor)
            {
                StringConstructor = stringConstructor;
            }
        }
    }
}