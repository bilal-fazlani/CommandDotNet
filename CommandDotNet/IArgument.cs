using System.Collections.Generic;
using CommandDotNet.Builders;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    /// <summary>An argument is either an <see cref="Option"/> or <see cref="Operand"/></summary>
    public interface IArgument: IArgumentNode
    {
        /// <summary>The <see cref="ITypeInfo"/> for this argument</summary>
        ITypeInfo TypeInfo { get; }

        /// <summary>The <see cref="IArgumentArity"/> for this argument, describing how many values are allowed.</summary>
        IArgumentArity Arity { get; set; }

        /// <summary>The default value for this argument</summary>
        ArgumentDefault? Default { get; set; }

        /// <summary>
        /// The allowed values for this argument, as defined by an <see cref="IAllowedValuesTypeDescriptor"/> for this type.
        /// i.e. enum arguments will list all values in the enum.
        /// </summary>
        IReadOnlyCollection<string> AllowedValues { get; set; }
        
        /// <summary>
        /// The text values provided as input to the application.
        /// Will be empty if no values were provided.<br/>
        /// Sources provided by this framework can be found at <see cref="Constants.InputValueSources"/>
        /// </summary>
        ICollection<InputValue> InputValues { get; }

        /// <summary>The parsed and converted value for the argument to be passed to a method</summary>
        object? Value { get; set; }
    }
}