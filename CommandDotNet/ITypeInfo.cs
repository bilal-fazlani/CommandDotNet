using System;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    /// <summary>The .Net type information for the given argument</summary>
    public interface ITypeInfo
    {
        /// <summary>The parameter or property type of the argument</summary>
        Type Type { get; }

        /// <summary>
        /// If the <see cref="ITypeInfo.Type"/> is nullable or collection, this is the generic argument type.
        /// Otherwise it will be the same value as <see cref="ITypeInfo.Type"/>
        /// </summary>
        Type UnderlyingType { get; }

        /// <summary>The DisplayName as determined by the <see cref="IArgumentTypeDescriptor"/> for this type</summary>
        string? DisplayName { get; set; }
    }
}