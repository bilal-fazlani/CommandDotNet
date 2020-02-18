﻿using System;

namespace CommandDotNet.Extensions
{
    public static class ArgumentExtensions
    {
        /// <summary>Returns true if argument name is <see cref="Constants.HelpOptionName"/></summary>
        public static bool IsHelpOption(this IArgument argument)
        {
            if (argument == null) throw new ArgumentNullException(nameof(argument));

            return argument.Name == Constants.HelpOptionName;
        }

        /// <summary>Returns true if argument name is <see cref="Constants.VersionOptionName"/> and is on the root command.</summary>
        public static bool IsAppVersionOption(this IArgument argument)
        {
            if (argument == null) throw new ArgumentNullException(nameof(argument));

            return argument.Name == Constants.VersionOptionName && argument.Parent.IsRootCommand();
        }

        /// <summary>
        /// For the given <see cref="argument"/>,
        /// execute <see cref="operandAction"/> when <see cref="Operand"/>
        /// and <see cref="optionAction"/> when <see cref="Option"/>
        /// </summary>
        public static void SwitchAct(
            this IArgument argument,
            Action<Operand> operandAction,
            Action<Option> optionAction)
        {
            switch (argument)
            {
                case null:
                    throw new ArgumentNullException(nameof(argument));
                case Operand operand:
                    operandAction?.Invoke(operand);
                    break;
                case Option option:
                    optionAction?.Invoke(option);
                    break;
                default:
                    throw new ArgumentException(BuildExMessage(argument));
            }
        }

        /// <summary>
        /// For the given <see cref="argument"/>,
        /// execute <see cref="operandFunc"/> when <see cref="Operand"/>
        /// and <see cref="optionFunc"/> when <see cref="Option"/>
        /// </summary>
        public static TResult SwitchFunc<TResult>(
            this IArgument argument,
            Func<Operand, TResult> operandFunc,
            Func<Option, TResult> optionFunc)
        {
            switch (argument)
            {
                case null:
                    throw new ArgumentNullException(nameof(argument));
                case Operand operand:
                    return operandFunc == null ? default : operandFunc(operand);
                case Option option:
                    return optionFunc == null ? default : optionFunc(option);
                default:
                    throw new ArgumentException(BuildExMessage(argument));
            }
        }

        private static string BuildExMessage(IArgument argument)
        {
            return $"argument type must be `{typeof(Operand)}` or `{typeof(Option)}` but was `{argument.GetType()}`. " +
                   $"If `{argument.GetType()}` was created for extensibility, " +
                   $"consider using {nameof(IArgument)}.{nameof(IArgument.Services)} to store service classes instead.";
        }
    }
}