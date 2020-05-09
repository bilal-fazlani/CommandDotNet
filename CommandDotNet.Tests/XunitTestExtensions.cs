using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Tests
{
    public static class XunitTestExtensions
    {
        public static IEnumerable<object[]> ToObjectArrays<T>(this IEnumerable<T> items)
        {
            return items.Select(item => new object[] { item! });
        }
    }
}