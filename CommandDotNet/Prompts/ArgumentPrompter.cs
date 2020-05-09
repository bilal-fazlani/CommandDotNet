using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.Prompts
{
    public class ArgumentPrompter : IArgumentPrompter
    {
        private readonly Func<CommandContext, IArgument, string>? _getPromptTextCallback;
        private readonly IPrompter _prompter;

        public ArgumentPrompter(
            IPrompter prompter,
            Func<CommandContext, IArgument, string>? getPromptTextCallback = null)
        {
            _prompter = prompter;
            _getPromptTextCallback = getPromptTextCallback;
        }

        public virtual ICollection<string> PromptForArgumentValues(
            CommandContext commandContext, IArgument argument, out bool isCancellationRequested)
        {
            var argumentName = _getPromptTextCallback?.Invoke(commandContext, argument) ?? argument.Name;
            var promptText = $"{argumentName} ({argument.TypeInfo.DisplayName})";
            var isPassword = argument.TypeInfo.UnderlyingType == typeof(Password);

            ICollection<string> inputs = new List<string>();
            
            if (argument.Arity.AllowsMany())
            {
                if (_prompter.TryPromptForValues(promptText, out var values, out isCancellationRequested, isPassword: isPassword))
                {
                    inputs.AddRange(values);
                }
            }
            else
            {
                if (_prompter.TryPromptForValue(promptText, out var value, out isCancellationRequested, isPassword: isPassword))
                {
                    inputs.Add(value);
                }
            }

            return inputs;
        }
    }
}