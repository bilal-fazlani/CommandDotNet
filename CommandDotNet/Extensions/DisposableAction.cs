using System;

namespace CommandDotNet.Extensions
{
    internal class DisposableAction : IDisposable
    {
        internal static DisposableAction Null { get; } = new DisposableAction();

        private readonly Action? _action;

        private DisposableAction()
        {
        }

        public DisposableAction(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Dispose()
        {
            _action?.Invoke();
        }
    }
}