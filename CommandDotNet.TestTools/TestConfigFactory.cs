using System;
using System.Linq;
using CommandDotNet.Extensions;
using static System.Environment;

namespace CommandDotNet.TestTools
{
    internal static class TestConfigFactory
    {
        internal static TestConfig? GetDefaultFromSubClass()
        {
            try
            {
                var configs = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(assembly => assembly.ExportedTypes)
                    .Where(t => t.IsClass && t.InheritsFrom<IDefaultTestConfig>())
                    .Select(t =>
                    {
                        var defaultTestConfig = (IDefaultTestConfig) Activator.CreateInstance(t);
                        var config = defaultTestConfig.Default;
                        if (config != null && config.Source.IsNullOrWhitespace())
                        {
                            config.Source = t.FullName;
                        }
                        return config;
                    })
                    .Where(c => c is { })
                    .OrderBy(c => c!.Priority ?? int.MaxValue)
                    .ToList();

                /*
                if (configs.Count > 1)
                {
                    var csv = configs.Select(c => $"{c.Priority}: {c.Source}").ToCsv(NewLine);
                    Console.WriteLine($"Found more than one {nameof(TestConfig)} subclass. Using the one with lowest priority: {NewLine}{csv}");
                }
                */

                return configs.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to load default test configs:{NewLine}{e}");
                return null;
            }
        }
    }
}