using System;
using CommandDotNet.Execution;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    /// <summary>
    /// Events raised during the <see cref="MiddlewareStages.Tokenize"/> stage.<br/>
    /// Set in <see cref="AppRunner.Configure"/>
    /// </summary>
    public class TokenizationEvents
    {
        public event Action<OnTokenTransformationEventArgs>? OnTokenTransformation;

        public event Action<TokenizationCompletedEventArgs>? OnTokenizationCompleted;

        internal void TokenTransformation(CommandContext commandContext, TokenTransformation transformation, TokenCollection pre, TokenCollection post)
        {
            OnTokenTransformation?.Invoke(new OnTokenTransformationEventArgs(commandContext, transformation, pre, post));
        }

        internal void TokenizationCompleted(CommandContext commandContext)
        {
            OnTokenizationCompleted?.Invoke(new TokenizationCompletedEventArgs(commandContext));
        }

        public class TokenizationCompletedEventArgs : EventArgs
        {
            public CommandContext CommandContext { get; }
            
            public TokenizationCompletedEventArgs(CommandContext commandContext)
            {
                CommandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
            }
        }

        public class OnTokenTransformationEventArgs : EventArgs
        {
            public CommandContext CommandContext { get; }
            public TokenTransformation Transformation { get; }
            public TokenCollection PreTransformTokens { get; }
            public TokenCollection PostTransformTokens { get; }

            public OnTokenTransformationEventArgs(
                CommandContext commandContext,
                TokenTransformation transformation,
                TokenCollection pre,
                TokenCollection post)
            {
                CommandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
                Transformation = transformation ?? throw new ArgumentNullException(nameof(transformation));
                PreTransformTokens = pre ?? throw new ArgumentNullException(nameof(pre));
                PostTransformTokens = post ?? throw new ArgumentNullException(nameof(post));
            }
        }
    }
}