using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Builders;

namespace CommandDotNet.Extensions
{
    public static class CustomAttributesContainerExtensions
    {
        public static bool HasAttribute<T>(this ICustomAttributesContainer container) where T : Attribute
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            return container.CustomAttributes?.HasAttribute<T>() ?? false;
        }

        public static T? GetCustomAttribute<T>(this ICustomAttributesContainer container) where T : Attribute
        {
            return container
                .GetCustomAttributes<T>()
                .SingleOrDefaultOrThrow(
                () => throw new AppRunnerException($"attempted to get a single {typeof(T).Name} from {container} but multiple exist"));
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributesContainer container) where T : Attribute
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            return container.CustomAttributes.GetCustomAttributes(typeof(T), false).Cast<T>();
        }
    }
}