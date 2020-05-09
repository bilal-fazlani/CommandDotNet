using System;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class PropertyArgumentDef : IArgumentDef
    {
        private readonly PropertyInfo _propertyInfo;

        public string ArgumentDefType => "Property";
        
        public CommandNodeType CommandNodeType { get; }

        public string Name { get; }

        public string SourcePath => _propertyInfo.FullName(includeNamespace: true);

        public Type Type => _propertyInfo.PropertyType;

        public bool HasDefaultValue { get; }

        public object DefaultValue { get; }

        public IArgument? Argument { get; set; }

        public ICustomAttributeProvider CustomAttributes => _propertyInfo;

        public ValueProxy ValueProxy { get; }

        public PropertyArgumentDef(
            PropertyInfo propertyInfo,
            CommandNodeType commandNodeType,
            AppConfig appConfig,
            object modelInstance)
        {
            if (modelInstance == null)
            {
                throw new ArgumentNullException(nameof(modelInstance));
            }

            _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            CommandNodeType = commandNodeType;
            Name = propertyInfo.BuildName(commandNodeType, appConfig);
            ValueProxy = new ValueProxy(
                () => _propertyInfo.GetValue(modelInstance),

                value => _propertyInfo.SetValue(modelInstance, value)
            );
            DefaultValue = propertyInfo.GetValue(modelInstance);
            HasDefaultValue = propertyInfo.PropertyType.IsClass
                ? !DefaultValue.IsNullValue()
                : !DefaultValue.IsDefaultFor(propertyInfo.PropertyType);
        }

        public override string ToString()
        {
            return $"Property:{SourcePath} > {Name}({Type})";
        }
    }
}