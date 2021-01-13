using System.IO;
using System.Text;
using CommandDotNet.Rendering;

namespace CommandDotNet.TestTools
{
    public partial class TestConsole
    {
        private class TestConsoleWriter : TextWriter, IConsoleWriter
        {
            private readonly TestConsoleWriter? _inner;
            private readonly StringBuilder _stringBuilder = new StringBuilder();

            public TestConsoleWriter(
                TestConsoleWriter? inner = null)
            {
                _inner = inner;
            }

            public override void Write(char value)
            {
                _inner?.Write(value);
                if (value == '\b' && _stringBuilder.Length > 0)
                {
                    _stringBuilder.Length = _stringBuilder.Length - 1;
                }
                else
                {
                    _stringBuilder.Append(value);
                }
            }

            public override Encoding Encoding { get; } = Encoding.Unicode;

            public override string ToString() => _stringBuilder.ToString();
        }
    }
}