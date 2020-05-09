using System;
using System.Collections.Generic;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface ICommandDef: ISourceDef
    {
        Type? CommandHostClassType { get; }
        bool IsExecutable { get; }
        bool HasInterceptor { get; }
        IReadOnlyCollection<ICommandDef> SubCommands { get; }
        IMethodDef? InterceptorMethodDef { get; }
        IMethodDef? InvokeMethodDef { get; }
    }
}