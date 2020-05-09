using System;
using System.Runtime.CompilerServices;

namespace CommandDotNet
{
    /// <summary>
    /// Used to determine the position of <see cref="Operand"/>s, <see cref="Option"/>s and nested <see cref="IArgumentModel"/>s within the class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OrderByPositionInClassAttribute : Attribute
    {
        public int CallerLineNumber { get; }

        /// <summary>
        /// Used to determine the position of <see cref="Operand"/>s, <see cref="Option"/>s and nested <see cref="IArgumentModel"/>s within the class.
        /// </summary>
        /// <param name="__callerLineNumber">
        /// The value is defaulted by <see cref="CallerLineNumberAttribute"/>.  Leave blank to let the position of the property determine the order.
        /// </param>
        public OrderByPositionInClassAttribute([CallerLineNumber] int __callerLineNumber = 0)
        {
            CallerLineNumber = __callerLineNumber;
        }
    }
}