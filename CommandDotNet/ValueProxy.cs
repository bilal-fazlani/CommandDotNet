using System;

namespace CommandDotNet
{
    /// <summary>Proxy to get and set the value of an argument</summary>
    public class ValueProxy
    {
        /// <summary>The function to get the value</summary>
        public Func<object?> Getter { get; }
        /// <summary>The function to set the value</summary>
        public Action<object?> Setter { get; }

        public ValueProxy(Func<object?> getter, Action<object?> setter)
        {
            Getter = getter;
            Setter = setter;
        }
    }
}