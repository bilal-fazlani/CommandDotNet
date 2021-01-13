using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Logging.LogProviders;
using CommandDotNet.Rendering;

namespace CommandDotNet.Prompting
{
    public class Line
    {
        private readonly IConsole _console;
        private readonly bool _isSecret;
        private readonly StringBuilder _line = new StringBuilder();
        private readonly CursorPosition _startingPosition;

        private struct BufferRow
        {
            public int Top { get; }
            public int StartOfLine { get; }
            public int EndOfLine { get; }
            public int Length => EndOfLine - StartOfLine;

            public BufferRow(int top, int startOfLine, int endOfLine)
            {
                Top = top;
                StartOfLine = startOfLine;
                EndOfLine = endOfLine;
            }

            public static IEnumerable<BufferRow> GetBufferRows(IConsole console, CursorPosition startingPosition)
            {
                var currentTop = console.CursorTop;
                var currentLeft = console.CursorLeft;

                if (currentTop == startingPosition.Top)
                {
                    yield return new BufferRow(currentTop, startingPosition.Left, currentLeft);
                    yield break;
                }
                var bufferWidth = console.BufferWidth;

                yield return new BufferRow(startingPosition.Top, startingPosition.Left, bufferWidth);
                if (currentTop - startingPosition.Top > 1)
                {
                    for (int i = startingPosition.Top + 1; i < (currentTop - 1); i++)
                    {
                        yield return new BufferRow(startingPosition.Top, 0, bufferWidth);
                    }
                }
                yield return new BufferRow(currentTop, 0, currentLeft);
            }
        }

        public string Text => _line.ToString();

        public int PositionInText => DistanceFromStartOfLine();

        public Line(IConsole console, bool isSecret = false)
        {
            _console = console;
            _isSecret = isSecret;
            _startingPosition = CursorPosition.Snapshot(console);
        }

        private IEnumerable<BufferRow> GetBufferRows() => BufferRow.GetBufferRows(_console, _startingPosition);

        public int DistanceFromStartOfLine() => GetBufferRows().Sum(r => r.Length);

        public bool IsStartOfLine => PositionInText == 0;
        public bool IsEndOfLine => PositionInText == _line.Length;

        public void MoveToStartOfLine()
        {
            _startingPosition.Restore(_console);
        }

        public void MoveToEndOfLine()
        {
            MoveRight(int.MaxValue);
        }

        public void MoveLeft(int count = 1)
        {
            var distanceFromBol = PositionInText;
            _console.CursorLeft -= Math.Min(count, distanceFromBol);
        }

        public void MoveRight(int count = 1)
        {
            var distanceFromEol = _line.Length - PositionInText;
            _console.CursorLeft += Math.Min(count, distanceFromEol);
        }

        public int EndOfWordDistance()
        {
            var index = PositionInText;
            var text = Text;
            var i = index;

            while (i < text.Length && char.IsWhiteSpace(text[i]))
            {
                i++;
            }
            while (i < text.Length && !char.IsWhiteSpace(text[i]))
            {
                i++;
            }

            return i - index;
        }

        public int StartOfWordDistance()
        {
            var index = PositionInText;
            var text = Text;
            var i = index;

            if (i >= text.Length)
            {
                i--;
            }

            // already at the start of a word?
            if (i > 0 && !char.IsWhiteSpace(text[i]) && char.IsWhiteSpace(text[i - 1]))
            {
                i--;
            }

            while (i >= 0 && char.IsWhiteSpace(text[i]))
            {
                i--;
            }
            while (i >= 0 && !char.IsWhiteSpace(text[i]))
            {
                i--;
            }

            if (i < 0 || char.IsWhiteSpace(text[i]))
            {
                i++;
            }

            return index - i;
        }

        public void ClearLine()
        {
            MoveToStartOfLine();
            Delete(_line.Length);
        }

        public void Backspace(int count = 1)
        {
            if (IsStartOfLine) return;

            MoveLeft(count);
            Delete(count);
        }

        public void Delete(int count = 1)
        {
            if (IsEndOfLine) return;

            _line.Remove(PositionInText, count);

            using var _ = RestoreCursorAndAdjust();
            _console.Write(_line.ToString().Substring(PositionInText));
            // remove trailing characters
            _console.Write(new string(' ', count));
            _console.Write(new string('\b', count));
        }

        public void Write(ConsoleKeyInfo consoleKey) => Write(consoleKey.KeyChar);

        public void Write(char c)
        {
            Write(c.ToString());
        }

        public void Write(string text)
        {
            void WriteToConsole(string output)
            {
                if (!_isSecret)
                {
                    _console.Write(output);
                }
            }
            
            if (IsEndOfLine)
            {
                _line.Append(text);
                WriteToConsole(text);
            }
            else
            {
                _line.Insert(PositionInText, text);
                var output = _line.ToString().Substring(PositionInText);

                // windows moves cursor to EOL
                using var _ = RestoreCursorAndAdjust(() => MoveRight(text.Length));
                WriteToConsole(output);
                
            }
        }

        private IDisposable RestoreCursorAndAdjust(Action postAdjustment = null)
        {
            // windows moves cursor to EOL after edits
            var cursorPosition = CursorPosition.Snapshot(_console);
            return new DisposableAction(() =>
            {
                cursorPosition.Restore(_console);
                postAdjustment?.Invoke();
            });
        }

        public void ClearLineAndWrite(string newText)
        {
            ClearLine();
            Write(newText);
        }

        public void Notify(string notification, bool preserveText)
        {
            var value = _line.ToString();

            MoveToStartOfLine();
            Delete(_line.Length);
            _console.WriteLine(notification);

            if (preserveText && !value.IsNullOrEmpty())
            {
                Write(value);
            }
        }
    }
}