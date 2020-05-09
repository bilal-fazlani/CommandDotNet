using CommandDotNet.Extensions;

namespace CommandDotNet.Help
{
    public class AppHelpSettings : IIndentableToString
    {
        /// <summary>When true, the help option will be included in the help for every command</summary>
        public bool PrintHelpOption { get; set; }

        /// <summary>Specify whether to use Basic or Detailed help mode. Default is Detailed.</summary>
        public HelpTextStyle TextStyle { get; set; } = HelpTextStyle.Detailed;

        /// <summary>Specify what AppName to use in the 'Usage:' example</summary>
        public UsageAppNameStyle UsageAppNameStyle { get; set; } = UsageAppNameStyle.Adaptive;

        /// <summary>
        /// The application name used in the Usage section of help documentation.<br/>
        /// When specified, <see cref="UsageAppNameStyle"/> is ignored.
        /// </summary>
        public string? UsageAppName { get; set; }

        /// <summary>
        /// When true, the usage section will expand arguments so the names of all arguments are shown.
        /// </summary>
        public bool ExpandArgumentsInUsage { get; set; } = true;

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            return this.ToStringFromPublicProperties(indent);
        }
    }
}