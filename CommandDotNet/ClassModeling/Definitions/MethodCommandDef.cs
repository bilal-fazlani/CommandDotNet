using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class MethodCommandDef : ICommandDef
    {
        private readonly MethodBase _method;

        public string Name { get; }
        public string SourcePath => _method.FullName(includeNamespace: true);
        public Type CommandHostClassType { get; }
        public ICustomAttributeProvider CustomAttributes => _method;
        public bool IsExecutable => true;
        public bool HasInterceptor => false;
        public IReadOnlyCollection<ICommandDef> SubCommands { get; } = new List<ICommandDef>().AsReadOnly();
        public IMethodDef? InterceptorMethodDef { get; } = null;
        public IMethodDef InvokeMethodDef { get; }
        
        public MethodCommandDef(MethodInfo method, Type commandHostClassType, AppConfig appConfig)
        {
            _method = method;

            Name = method.BuildName(CommandNodeType.Command, appConfig);
            CommandHostClassType = commandHostClassType;
            InvokeMethodDef = new MethodDef(method, appConfig);
        }

        public override string ToString()
        {
            return $"Method:{SourcePath} > {Name}";
        }
    }
}