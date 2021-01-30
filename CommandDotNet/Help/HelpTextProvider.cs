using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;

namespace CommandDotNet.Help
{
    public class HelpTextProvider : IHelpProvider
    {
        private readonly AppHelpSettings _appHelpSettings;
        private string? _appName;

        public HelpTextProvider(AppSettings appSettings, string? appName = null)
        {
            _appName = appName;
            _appHelpSettings = appSettings.Help;
        }
        
        public virtual string GetHelpText(Command command) =>
            JoinSections(
                (null, CommandDescription(command)),
                ("Usage", SectionUsage(command)),
                ("Arguments", SectionOperands(command)),
                ("Options", SectionOptions(command, false)),
                ("Options also available for subcommands", SectionOptions(command, true)),
                ("Commands", SectionSubcommands(command)),
                (null, ExtendedHelpText(command)));

        /// <summary>returns the body of the usage section</summary>
        protected virtual string? SectionUsage(Command command)
        {
            var usage = PadFront(command.Usage)
                           ?? $"{PadFront(AppName(command))}{PadFront(CommandPath(command))}"
                           + $"{PadFront(UsageSubcommand(command))}{PadFront(UsageOption(command))}{PadFront(UsageOperand(command))}"
                           + (command.ArgumentSeparatorStrategy == ArgumentSeparatorStrategy.PassThru ? " [[--] <arg>...]" : null);
            return CommandReplacements(command, usage);
        }

        protected virtual string AppName(Command command) =>
            _appName ??= command.GetAppName(_appHelpSettings);

        /// <summary>The current command and it's parents.  aka bread crumbs</summary>
        protected virtual string CommandPath(Command command) => command.GetPath();

        /// <summary>How operands are shown in the usage example</summary>
        protected virtual string? UsageOperand(Command command)
        {
            if (!command.Operands.Any())
            {
                return null;
            }

            if (!_appHelpSettings.ExpandArgumentsInUsage)
            {
                return "[arguments]";
            }

            if (command.Operands.Last().Arity.Minimum > 0)
            {
                return command.Operands.Select(o => $"<{o.Name}>").ToCsv(" ");
            }

            var sb = new StringBuilder("]");
            bool inOptionalRegion = true;
            foreach (var operand in command.Operands.Reverse())
            {
                if (inOptionalRegion && operand.Arity.Minimum > 0)
                {
                    // remove leading space
                    sb.Remove(0, 1);
                    sb.Insert(0, " [");
                    inOptionalRegion = false;
                }
                sb.Insert(0, $" <{operand.Name}>");
            }
            sb.Remove(0, 1);
            if (inOptionalRegion)
            {
                sb.Insert(0, "[");
            }
            return sb.ToString();
        }

        /// <summary>How options are shown in the usage example</summary>
        protected virtual string? UsageOption(Command command) =>
            command.Options.Any(o => !o.Hidden)
                ? "[options]"
                : null;

        /// <summary>How subcommands are shown in the usage example</summary>
        protected virtual string? UsageSubcommand(Command command) =>
            command.Subcommands.Any()
                ? "[command]"
                : null;

        protected virtual string? ExtendedHelpText(Command command) => 
            CommandReplacements(command, command.ExtendedHelpText);

        /// <summary>returns the body of the options section</summary> 
        protected virtual string? SectionOptions(Command command, bool includeInterceptorOptionsForExecutableCommands)
        {
            var options = command.Options
                .Where(o => !o.Hidden)
                .Where(o => includeInterceptorOptionsForExecutableCommands
                    ? (command.IsExecutable && o.IsInterceptorOption)
                    : !(command.IsExecutable && o.IsInterceptorOption))
                .OrderBy(o => o.IsMiddlewareOption)
                .ThenBy(o => o.IsInterceptorOption)
                .ToCollection();

            return SectionArguments(command, options);
        }

        /// <summary>returns the body of the operands section</summary>
        protected virtual string? SectionOperands(Command command) => 
            SectionArguments(command, command.Operands.ToCollection());

        /// <summary>returns the body of an arguments section</summary>
        protected virtual string? SectionArguments<T>(Command command, ICollection<T> arguments)
            where T : IArgument
        {
            if (!arguments.Any())
            {
                return null;
            }

            var helpValues = BuildArgumentHelpValues(arguments);
            var templateMaxLength = helpValues.Max(a => a.Template?.Length) ?? 0;
            var displayNameMaxLength = helpValues.Max(a => a.TypeName?.Length) ?? 0;

            var sb = new StringBuilder();
            foreach (var helpValue in helpValues)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                }

                new[]
                    {
                        Row(
                            (templateMaxLength, helpValue.Template),
                            (displayNameMaxLength, helpValue.TypeName),
                            (-1, helpValue.DefaultValue)),
                        Row(helpValue.Description),
                        Row(helpValue.AllowedValues)
                    }
                    .Where(r => !r.IsNullOrWhitespace())
                    .ForEach(r => sb.AppendLine(r));
            }

            return sb.ToString();
        }

        /// <summary>returns the body of the subcommands section</summary>
        protected virtual string? SectionSubcommands(Command command)
        {
            var commands = command.Subcommands.ToCollection();

            if (!commands.Any())
            {
                return null;
            }

            var helpValues = BuildCommandHelpValues(commands);
            var maxCmdLen = helpValues.Max(c => c.Name?.Length) ?? 0;

            var sb = new StringBuilder();
            foreach (var helpValue in helpValues.OrderBy(c => c.Name))
            {
                sb.AppendLine(Row((maxCmdLen, helpValue.Name), (-1, helpValue.Description)));
            }

            var subcommandHelpHint = SubcommandHelpHint(command);
            if (!subcommandHelpHint.IsNullOrWhitespace())
            {
                sb.AppendLine();
                sb.Append(subcommandHelpHint);
            }

            return sb.ToString();
        }

        /// <summary>Hint displayed in the subcommands section for getting help for a subcommand.</summary>
        protected virtual string? SubcommandHelpHint(Command command) =>
            $"Use \"{AppName(command)}{PadFront(CommandPath(command))} " +
            $"[command] --{Constants.HelpOptionName}\" " +
            "for more information about a command.";

        protected virtual string CommandName(Command command) => command.Name;

        protected virtual string? CommandDescription(Command command) =>
            CommandReplacements(command, command.Description.UnlessNullOrWhitespace());

        protected virtual string? ArgumentName<T>(T argument) where T : IArgument =>
            argument.SwitchFunc(
                operand => operand.Name,
                option => OptionTemplate.Build(option.LongName, option.ShortName));

        protected virtual string? ArgumentTypeName<T>(T argument) where T : IArgument => 
            argument.TypeInfo.DisplayName.UnlessNullOrWhitespace(n => $"<{n.ToUpperInvariant()}>");

        protected virtual string? ArgumentDescription<T>(T argument) where T : IArgument => 
            argument.Description.UnlessNullOrWhitespace();

        protected virtual string? ArgumentArity<T>(T argument) where T : IArgument => 
            (argument.Arity.AllowsMany() ? " (Multiple)" : "");

        /// <summary>Returns a comma-separated list of the allowed values</summary>
        protected virtual string? ArgumentAllowedValues<T>(T argument) where T : IArgument =>
            argument.AllowedValues?.ToCsv(", ").UnlessNullOrWhitespace(v => $"Allowed values: {v}");

        /// <summary></summary>
        protected virtual string? ArgumentDefaultValue(IArgument argument)
        {
            object? defaultValue = argument.Default?.Value;

            return defaultValue.IsNullValue() 
                ? null 
                : $"[{defaultValue!.ValueToString(argument)}]";
        }

        /// <summary>Row with default indent of 2 spaces</summary>
        protected virtual string? Row(string? cell, int indent = 2) => $"{new string(' ', indent)}{cell}".TrimEnd();

        /// <summary>Indents the row and aligns the cells</summary>
        protected virtual string? Row(params (int maxLength, string? value)[] cells) =>
            Row(cells.Select(c => c.maxLength > 0
                    ? string.Format($"{{0, -{c.maxLength + 2}}}", c.value)
                    : c.value)
                .ToCsv(""));

        /// <summary>Formats a section header.  Default appends line endings except for Usage</summary>
        protected virtual string? FormatSectionHeader(string header)
            => "usage".Equals(header, StringComparison.OrdinalIgnoreCase)
                    ? $"{header}:"
                    : $"{header}:{Environment.NewLine}{Environment.NewLine}";

        /// <summary>Joins the content into a single string, with headers and sections</summary>
        protected virtual string JoinSections(params (string? header, string? body)[] sections) =>
            sections
                .Where(s => !s.body.IsNullOrWhitespace())
                .Select(s => $"{(s.header.IsNullOrWhitespace() ? null : FormatSectionHeader(s.header!))}{s.body!.Trim('\n', '\r')}")
                .ToCsv($"{Environment.NewLine}{Environment.NewLine}");

        private static string? PadFront(string? value) =>
            value.IsNullOrWhitespace() ? null : " " + value;

        private class CommandHelpValues
        {
            public readonly string Name;
            public readonly string? Description;

            public CommandHelpValues(string name, string? description)
            {
                Name = name;
                Description = description;
            }
        }

        private ICollection<CommandHelpValues> BuildCommandHelpValues(IEnumerable<Command> commands) =>
            commands.Select(c => new CommandHelpValues(CommandName(c), CommandDescription(c))).ToCollection();

        private string? CommandReplacements(Command command, string? text) => text?
            .Replace("%UsageAppName%", AppName(command))
            .Replace("%AppName%", AppName(command))
            .Replace("%CmdPath%", command.GetPath());

        private class ArgumentHelpValues
        {
            public readonly string Template;
            public readonly string? TypeName;
            public readonly string? DefaultValue;
            public readonly string? Description;
            public readonly string? AllowedValues;

            public ArgumentHelpValues(
                string template,
                string? typeName,
                string? defaultValue,
                string? description,
                string? allowedValues)
            {
                Template = template;
                TypeName = typeName;
                DefaultValue = defaultValue;
                Description = description;
                AllowedValues = allowedValues;
            }
        }

        private ICollection<ArgumentHelpValues> BuildArgumentHelpValues<T>(IEnumerable<T> arguments) where T : IArgument =>
            arguments.Select(a => new ArgumentHelpValues
            ($"{ArgumentName(a)}{ArgumentArity(a)}",
                ArgumentTypeName(a),
                ArgumentDefaultValue(a),
                ArgumentDescription(a),
                ArgumentAllowedValues(a)
            )).ToCollection();
    }
}