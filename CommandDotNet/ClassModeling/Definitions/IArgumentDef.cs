using System;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface IArgumentDef: ISourceDef
    {
        string ArgumentDefType { get; }
        CommandNodeType CommandNodeType { get; }
        Type Type { get; }
        bool HasDefaultValue { get; }
        object DefaultValue { get; }
        IArgument? Argument { get; set; }
        ValueProxy ValueProxy { get; }
    }
}