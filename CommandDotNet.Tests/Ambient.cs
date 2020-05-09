using System;
using System.Threading;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public static class Ambient
    {
        private static readonly AsyncLocal<ITestOutputHelper> TestOutputHelper = new AsyncLocal<ITestOutputHelper>();

        public static ITestOutputHelper? Output
        {
            get => TestOutputHelper.Value;
            set => TestOutputHelper.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static Action<string?>? WriteLine
        {
            get
            {
                var output = Output;
                if (output == null)
                {
                    throw new InvalidOperationException($"{nameof(Ambient)}.{nameof(Output)} has not been set for the current test");
                }

                return msg =>
                {
                    // XUnit does not like null values
                    if (msg == null)
                    {
                        msg = "";
                    }
                    output.WriteLine(msg);
                };
            }
        }
    }
}