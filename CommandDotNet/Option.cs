// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    public sealed class Option : IArgument
    {
        private Command? _parent;
        private object? _value;
        private readonly ValueProxy _valueProxy;

        private readonly HashSet<string> _aliases;

        public Option(
            string? longName,
            char? shortName,
            TypeInfo typeInfo,
            IArgumentArity arity,
            string? definitionSource = null,
            IEnumerable<string>? aliases = null,
            ICustomAttributeProvider? customAttributes = null,
            bool isInterceptorOption = false,
            bool assignToExecutableSubcommands = false,
            ValueProxy? valueProxy = null)
        {
            if (longName.IsNullOrWhitespace() && shortName.IsNullOrWhitespace())
            {
                throw new ArgumentException($"a long or short name is required. source:{definitionSource}");
            }

            if (isInterceptorOption && assignToExecutableSubcommands)
            {
                throw new ArgumentException($"{nameof(isInterceptorOption)} and {nameof(assignToExecutableSubcommands)} are mutually exclusive. " +
                                            $"They cannot both be true. source:{definitionSource}");
            }

            _valueProxy = valueProxy ?? new ValueProxy(() => _value, o => _value = o);

            TypeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            Arity = arity ?? throw new ArgumentNullException(nameof(arity));
            DefinitionSource = definitionSource;
            IsInterceptorOption = isInterceptorOption;
            AssignToExecutableSubcommands = assignToExecutableSubcommands;
            CustomAttributes = customAttributes ?? NullCustomAttributeProvider.Instance;
            LongName = longName;
            ShortName = shortName;

            _aliases = aliases == null
                ? new HashSet<string>()
                : new HashSet<string>(aliases);
            if (!LongName.IsNullOrWhitespace()) _aliases.Add(LongName!);
            if (!ShortName.IsNullOrWhitespace()) _aliases.Add(ShortName.ToString());
        }

        public string Name => LongName ?? ShortName.ToString();

        /// <summary>A single character that will be prefixed with a single hyphen.</summary>
        public char? ShortName { get; }

        /// <summary>The long name that will be prefixed with a double hyphen.</summary>
        public string? LongName { get; }

        /// <summary>The <see cref="Command"/> that hosts this <see cref="Option"/></summary>
        public Command? Parent
        {
            get => _parent;
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{nameof(Parent)} cannot be assigned to null");
                }
                if (_parent is null)
                {
                    _parent = value;
                }
                else if (_parent != value)
                {
                    throw new InvalidConfigurationException($"{nameof(Parent)} is already assigned for {this}.  Current={_parent} New={value}");
                }
            }
        }

        /// <summary>The aliases defined for this argument</summary>
        public IReadOnlyCollection<string> Aliases => _aliases;

        /// <summary>The source that defined this argument</summary>
        public string? DefinitionSource { get; }

        /// <summary>Describes the option</summary>
        public string? Description { get; set; }

        /// <summary>The <see cref="ITypeInfo"/> for this argument</summary>
        public ITypeInfo TypeInfo { get; set; }

        /// <summary>The <see cref="IArgumentArity"/> for this argument, describing how many values are allowed.</summary>
        public IArgumentArity Arity { get; set; }

        /// <summary>The default value for this argument</summary>
        public ArgumentDefault? Default { get; set; }

        /// <summary>
        /// The allowed values for this argument, as defined by an <see cref="IAllowedValuesTypeDescriptor"/> for this type.
        /// i.e. enum arguments will list all values in the enum.
        /// </summary>
        public IReadOnlyCollection<string> AllowedValues { get; set; } = EmptyCollection<string>.Instance;

        /// <summary>
        /// The text values provided as input.
        /// Will be empty if no values were provided.<br/>
        /// Sources provided by this framework can be found at <see cref="Constants.InputValueSources"/><br/>
        /// Flag options will contain a bool value.
        /// </summary>
        public ICollection<InputValue> InputValues { get; } = new List<InputValue>();

        /// <summary>The parsed and converted value for the argument to be passed to a method</summary>
        public object? Value
        {
            get => _valueProxy.Getter();
            set => _valueProxy.Setter(value);
        }

        /// <summary>
        /// When true, this option is an Inherited option, defined by a command interceptor method
        /// and assigned to executable subcommands of the interceptor.<br/>
        /// Note: The Parent will still be the defining command, not the target command.
        /// </summary>
        public bool AssignToExecutableSubcommands { get; }

        /// <summary>
        /// True when option is not defined in class model
        /// but is instead added via middleware.<br/>
        /// eg. Help and Version
        /// </summary>
        public bool IsMiddlewareOption { get; set; }

        /// <summary>
        /// True when the option is defined in an interceptor method,
        /// making it available for use when subcommands are executed.<br/>
        /// This helps distinguish an interceptor option from a default command option
        /// for parent commands.
        /// </summary>
        public bool IsInterceptorOption { get; }

        /// <summary>
        /// When true, the option should be shown in help.<br/>
        /// Default: true
        /// </summary>
        [Obsolete("Use Hidden instead. ShowInHelp == !Hidden")]
        public bool ShowInHelp { get => !Hidden; set => Hidden = !value; }

        /// <summary>
        /// When true, the option is hidden and should not be shown
        /// to the user in help or suggestions or anywhere else.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>True when the option is a bool with an arity of exactly zero</summary>
        public bool IsFlag => ArgumentArity.Zero.Equals(Arity) && TypeInfo.UnderlyingType == typeof(bool);

        /// <summary>The attributes defined on the parameter or property that define this argument</summary>
        public ICustomAttributeProvider CustomAttributes { get; }

        /// <summary>The services used by middleware for this argument</summary>
        public IServices Services { get; } = new Services();

        public override string ToString()
        {
            return $"Option: {Name} ({DefinitionSource})";
        }

        public static bool operator ==(Option x, Option y) => (object)x == (object)y;

        public static bool operator !=(Option x, Option y) => !(x == y);

        private bool Equals(Option other)
        {
            return string.Equals(LongName, other.LongName) 
                   && Equals(ShortName, other.ShortName);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Option) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (LongName != null ? LongName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ShortName != null ? ShortName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}