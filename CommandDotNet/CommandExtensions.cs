using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public static class CommandExtensions
    {
        public static bool IsRootCommand(this Command command) => command.Parent is null;

        /// <summary>Get the root command</summary>
        public static Command GetRootCommand(this Command command) => command.GetParentCommands(true).Last();

        /// <summary> Return all <see cref="Operand"/>s and <see cref="Option"/>s for the command</summary>
        /// <param name="command">The command</param>
        /// <param name="includeInterceptorOptions">When true, includes options from interceptors of all parent commands</param>
        /// <param name="excludeHiddenOptions">When true, hidden options like Help are not included</param>
        public static IEnumerable<IArgument> AllArguments(this Command command, 
            bool includeInterceptorOptions = false, bool excludeHiddenOptions = false)
        {
            return command.Operands.Cast<IArgument>()
                .Concat(command.AllOptions(includeInterceptorOptions, excludeHiddenOptions));
        }

        /// <summary>Return all <see cref="Option"/>s for the command.</summary>
        /// <param name="command">The command</param>
        /// <param name="includeInterceptorOptions">When true, includes options from interceptors of all parent commands</param>
        /// <param name="excludeHiddenOptions">when true, excludes hidden options</param>
        public static IEnumerable<Option> AllOptions(this Command command, 
            bool includeInterceptorOptions = false, bool excludeHiddenOptions = false)
        {
            var options = includeInterceptorOptions
                ? command.Options
                    .Concat(command
                        .GetParentCommands()
                        .Reverse()
                        .SelectMany(c => c.Options.Where(o => o.IsInterceptorOption)))
                : command.Options;
            if (excludeHiddenOptions)
            {
                options = options.Where(o => !o.Hidden);
            }
            return options;
        }

        /// <summary>returns the parents of the current command, sorted from closest to farthest parent</summary>
        public static IEnumerable<Command> GetParentCommands(this Command command, bool includeCurrent = false)
        {
            var startingCommand = includeCurrent ? command : command.Parent;
            for (var c = startingCommand; c is { }; c = c.Parent)
            {
                yield return c;
            }
        }

        /// <summary>Returns the path to the given command</summary>
        public static string GetPath(this Command command, string separator = " ") =>
            command.GetParentCommands(true)
                .Reverse().Skip(1).Select(c => c.Name)
                .ToCsv(separator);

        /// <summary>Returns all subcommands for the given command.</summary>
        public static IEnumerable<Command> GetDescendentCommands(this Command command, bool includeCurrent = false, int? depth = null)
        {
            if (includeCurrent)
            {
                yield return command;
            }

            if (depth != null)
            {
                depth--;
            }

            foreach (var child in command.Subcommands)
            {
                yield return child;
                foreach (var grandChild in child.GetDescendentCommands(depth: depth))
                {
                    yield return grandChild;
                }
            }
        }

        /// <summary>Determines if help was requested for the specified command</summary>
        public static bool HelpWasRequested(this Command command) => 
            command.HasInputValues(Constants.HelpOptionName);

        /// <summary>Returns true if the command contains an argument with the given alias</summary>
        public static bool ContainsArgumentNode(this Command command, string alias) =>
            command.FindArgumentNode(alias) != null;

        /// <summary>Returns true if input values exist for the argument with the given alias</summary>
        public static bool HasInputValues(this Command command, string alias) => 
            command.FindInputValues(alias)?.Any() ?? false;

        /// <summary>Returns the input values for the argument with the given alias or null</summary>
        public static ICollection<InputValue>? FindInputValues(this Command command, string alias) => 
            command.Find<IArgument>(alias)?.InputValues;

        /// <summary>
        /// Return the effective IgnoreUnexpectedOperands using the default
        /// from <see cref="AppSettings.IgnoreUnexpectedOperands"/>
        /// when <see cref="Command.IgnoreUnexpectedOperands"/> is not set.
        /// </summary>
        public static bool GetIgnoreUnexpectedOperands(this Command command, AppSettings appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            return command?.IgnoreUnexpectedOperands ?? appSettings.IgnoreUnexpectedOperands;
        }

        /// <summary>
        /// Return the effective <see cref="ArgumentSeparatorStrategy"/> using the default
        /// from <see cref="AppSettings.DefaultArgumentSeparatorStrategy"/>
        /// when <see cref="Command.ArgumentSeparatorStrategy"/> is not set.
        /// </summary>
        public static ArgumentSeparatorStrategy GetArgumentSeparatorStrategy(this Command command, AppSettings appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            return command?.ArgumentSeparatorStrategy ?? appSettings.DefaultArgumentSeparatorStrategy;
        }
    }
}