using System;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class ParameterArgumentDef : IArgumentDef
    {
        private readonly ParameterInfo _parameterInfo;

        public string ArgumentDefType => "Parameter";

        public CommandNodeType CommandNodeType { get; }

        public string Name { get; }

        public string SourcePath => _parameterInfo.FullName(includeNamespace: true);

        public Type Type => _parameterInfo.ParameterType;

        public bool HasDefaultValue => _parameterInfo.HasDefaultValue;

        public object DefaultValue => _parameterInfo.DefaultValue;

        public IArgument? Argument { get; set; }

        public ICustomAttributeProvider CustomAttributes => _parameterInfo;

        public ValueProxy ValueProxy { get; }

        public ParameterArgumentDef(
            ParameterInfo parameterInfo,
            CommandNodeType commandNodeType,
            AppConfig appConfig,
            object?[] parameterValues)
        {
            if (parameterValues == null)
            {
                throw new ArgumentNullException(nameof(parameterValues));
            }

            _parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));
            CommandNodeType = commandNodeType;

            Name = parameterInfo.BuildName(commandNodeType, appConfig);

            ValueProxy = new ValueProxy(
                () => parameterValues[parameterInfo.Position],

                value => parameterValues[parameterInfo.Position] = value
            );
        }

        public override string ToString()
        {
            return $"Parameter:{SourcePath} > {Name}({Type})";
        }
    }
}