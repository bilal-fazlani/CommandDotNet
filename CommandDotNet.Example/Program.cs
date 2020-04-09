using System.Collections.Specialized;
using System.Linq;
using CommandDotNet.Diagnostics;
using CommandDotNet.FluentValidation;
using CommandDotNet.NameCasing;
using CommandDotNet.NewerReleasesAlerts;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            Debugger.AttachIfDebugDirective(args);

            var appSettings = new NameValueCollection{{ "notify.--retry-count", "2"}};

            return new AppRunner<Examples>()
                .UseDefaultMiddleware(excludeAppendPipedInputToOperandList: true)
                .AppendPipedInputToOperandList(true)
                .UseLog2ConsoleDirective()
                .UseNameCasing(Case.KebabCase)
                .UseFluentValidation()
                .UseDefaultsFromAppSetting(appSettings, includeNamingConventions: true)
                .UseNewerReleaseAlertOnGitHub("bilal-fazlani", "commanddotnet", 
                    skipCommand: command => command.GetParentCommands(true).Any(c => c.Name == "pipes"))
                .Run(args);
        }
    }
}