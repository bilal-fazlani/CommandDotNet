using System;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    /// <summary>Arity describes how many values a user can or must provide for an argument</summary>
    public class ArgumentArity : IArgumentArity
    {
        public static readonly int Unlimited = int.MaxValue;

        // copied and modified from System.CommandLine
        // https://github.com/dotnet/command-line-api/blob/master/src/System.CommandLine/ArgumentArity.cs

        public ArgumentArity(int minimumNumberOfValues, int maximumNumberOfValues)
        {
            if (minimumNumberOfValues < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumNumberOfValues));
            }
            if (maximumNumberOfValues < minimumNumberOfValues)
            {
                throw new ArgumentException(
                    $"{nameof(maximumNumberOfValues)} must be greater than or equal to {nameof(minimumNumberOfValues)}");
            }

            Minimum = minimumNumberOfValues;
            Maximum = maximumNumberOfValues;
        }

        /// <summary>The minimum number of values the user must provide</summary>
        public int Minimum { get; }

        /// <summary>The maximum number of values the user must provide</summary>
        public int Maximum { get; }

        public static IArgumentArity Zero => new ArgumentArity(0, 0);

        public static IArgumentArity ZeroOrOne => new ArgumentArity(0, 1);

        public static IArgumentArity ExactlyOne => new ArgumentArity(1, 1);

        public static IArgumentArity ZeroOrMore => new ArgumentArity(0, Unlimited);

        public static IArgumentArity OneOrMore => new ArgumentArity(1, Unlimited);

        /// <summary>
        /// Calculates a default IArgumentArity for the given argument.<br/>
        /// Does NOT use the existing <see cref="IArgument.Arity"/>.
        /// </summary>
        public static IArgumentArity Default(IArgument argument)
        {
            var type = argument.TypeInfo.Type;
            var defaultValue = argument.Default?.Value;
            var hasDefaultValue = defaultValue != null && !defaultValue.IsDefaultFor(type);
            var booleanMode = argument.Services.GetOrAdd(() => new Box<BooleanMode>(BooleanMode.Unknown)).Value;
            return Default(type, hasDefaultValue, booleanMode);
        }

        /// <summary>Returns the default IArgumentArity for the given type</summary>
        public static IArgumentArity Default(Type type, bool hasDefaultValue, BooleanMode booleanMode)
        {
            if (type == typeof(bool) && booleanMode == BooleanMode.Unknown)
            {
                throw new ArgumentException($"{nameof(booleanMode)} cannot be {nameof(BooleanMode.Unknown)}");
            }

            bool isRequired = !(hasDefaultValue || type.IsNullableType());

            if (type != typeof(string) && type.IsEnumerable())
            {
                return isRequired ? OneOrMore : ZeroOrMore;
            }

            if (booleanMode == BooleanMode.Implicit && (type == typeof(bool) || type == typeof(bool?)))
            {
                return Zero;
            }

            return isRequired ? ExactlyOne : ZeroOrOne;
        }

        public override string ToString()
        {
            return $"{nameof(ArgumentArity)}:{Minimum}..{Maximum}";
        }

        public static bool operator ==(ArgumentArity x, ArgumentArity y) => (object)x == (object)y;

        public static bool operator !=(ArgumentArity x, ArgumentArity y) => !(x == y);

        protected bool Equals(ArgumentArity other)
        {
            return Minimum == other.Minimum && Maximum == other.Maximum;
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

            return Equals((ArgumentArity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Minimum * 397) ^ Maximum;
            }
        }
    }
}