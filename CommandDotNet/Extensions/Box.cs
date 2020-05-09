using System.Diagnostics.CodeAnalysis;

namespace CommandDotNet.Extensions
{
    internal class Box<T>
    {
        [MaybeNull]
        public T Value { get; }

        public Box(T value)
        {
            Value = value;
        }

        public static Box<T> CreateDefault() => new Box<T>(default!);
    }
}