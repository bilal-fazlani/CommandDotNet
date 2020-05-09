using System;
using System.Linq;
using System.Reflection;
using CommandDotNet.Help;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Tests
{
    public static class TestAppSettings
    {
        public static AppSettings BasicHelp => new AppSettings {Help = {TextStyle = HelpTextStyle.Basic}};
        public static AppSettings DetailedHelp => new AppSettings {Help = {TextStyle = HelpTextStyle.Detailed}};

        public static AppSettings Clone(this AppSettings original, Action<AppSettings> modify)
        {
            var clone = ShallowWiseClone(original);
            clone.Help = ShallowWiseClone(original.Help);

            clone.ArgumentTypeDescriptors = new ArgumentTypeDescriptors(original.ArgumentTypeDescriptors);

            modify(clone);
            return clone;
        }

        private static T ShallowWiseClone<T>(T original) where T : new()
        {
            var clone = new T();
            ShallowCopyProperties(original, clone);
            return clone;
        }

        private static void ShallowCopyProperties<T>(T original, T clone)
        {
            var props = typeof(T)
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var prop in props)
            {
                prop.SetValue(clone, prop.GetValue(original));
            }
        }
    }
}