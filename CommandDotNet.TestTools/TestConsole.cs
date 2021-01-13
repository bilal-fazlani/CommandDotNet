// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied & adapted from System.CommandLine 

using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Logging;
using CommandDotNet.Rendering;

namespace CommandDotNet.TestTools
{
    /// <summary>
    /// A test console that can be used to <br/>
    /// - capture output <br/>
    /// - provide piped input <br/>
    /// - handle ReadLine and ReadToEnd requests
    /// </summary>
    public partial class TestConsole : IConsole
    {
        private readonly Action<TestConsole> _onClear;
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static class Default
        {
            public static ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
            public static ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;

            public static int WindowLeft { get; set; } = 0;
            public static int WindowTop { get; set; } = 0;
            public static int WindowWidth { get; set; } = 120;
            public static int WindowHeight { get; set; } = 30;

            public static int BufferWidth { get; set; } = WindowWidth;
            public static int BufferHeight { get; set; } = WindowHeight;
        }

        public TestConsole(
            Func<TestConsole, string?>? onReadLine = null,
            IEnumerable<string>? pipedInput = null,
            Func<TestConsole, ConsoleKeyInfo>? onReadKey = null,
            Action<TestConsole> onClear = null)
        {
            _onClear = onClear;
            IsInputRedirected = pipedInput != null;

            if (pipedInput != null)
            {
                if (onReadLine != null)
                {
                    throw new Exception($"{nameof(onReadLine)} and {nameof(pipedInput)} cannot both be specified. " +
                                        "Windows will throw 'System.IO.IOException: The handle is invalid' on an attempt to ");
                }

                if (pipedInput is ICollection<string> inputs)
                {
                    var queue = new Queue<string>(inputs);
                    onReadLine = console => queue.Count == 0 ? null : queue.Dequeue();
                }
                else
                {
                    onReadLine = console => pipedInput.Take(1).FirstOrDefault();
                }
            }

            var all = new TestConsoleWriter();
            All = all;
            Out = new TestConsoleWriter(all);
            Error = new TestConsoleWriter(all);
            In = new TestConsoleReader(Out,
                () =>
                {
                    var input = onReadLine?.Invoke(this);
                    Log.Info($"IConsole.ReadLine > {input}");
                    return input;
                },
                onReadKey switch
                {
                    { } _ => () => onReadKey!.Invoke(this),
                    _ => null
                });
        }
        public string Title { get; set; }

        #region IStandardError

        public IConsoleWriter Error { get; }

        public bool IsErrorRedirected { get; } = false;

        #endregion

        #region IStandardOut

        public IConsoleWriter Out { get; }

        public bool IsOutputRedirected { get; } = false;

        #endregion

        #region IStandardIn

        public IConsoleReader In { get; }

        public bool IsInputRedirected { get; }

        #endregion

        /// <summary>
        /// This is the combined output for <see cref="Error"/> and <see cref="Out"/> in the order the lines were output.
        /// </summary>
        public IConsoleWriter All { get; }

        /// <summary>
        /// The combination of <see cref="Console.Error"/> and <see cref="Console.Out"/>
        /// in the order they were written from the app.<br/>
        /// This is how the output would appear in the shell.
        /// </summary>
        public string AllText() => All.ToString();

        /// <summary>
        /// The accumulated text of the <see cref="Console.Out"/> stream.
        /// </summary>
        public string OutText() => Out.ToString();

        /// <summary>
        /// The accumulated text of the <see cref="Console.Error"/> stream.
        /// </summary>
        public string ErrorText() => Error.ToString();

        public string OutLastLine => Out.ToString().SplitIntoLines().Last();

        #region IConsoleColor

        public ConsoleColor BackgroundColor { get; set; } = Default.BackgroundColor;
        public ConsoleColor ForegroundColor { get; set; } = Default.ForegroundColor;

        public void ResetColor()
        {
            BackgroundColor = Default.BackgroundColor;
            ForegroundColor = Default.ForegroundColor;
        }

        #endregion

        #region IConsoleBuffer

        public int BufferWidth { get; set; } = Default.BufferWidth;
        public int BufferHeight { get; set; } = Default.BufferHeight;

        public void SetBufferSize(int width, int height)
        {
            BufferWidth = width;
            BufferHeight = height;
        }

        public void Clear()
        {
            _onClear?.Invoke(this);
        }

        #endregion

        #region IConsoleWindow

        public int WindowLeft { get; set; } = Default.WindowLeft;
        public int WindowTop { get; set; } = Default.WindowTop;
        public int WindowWidth { get; set; } = Default.WindowWidth;
        public int WindowHeight { get; set; } = Default.WindowHeight;

        public void SetWindowPosition(int left, int top)
        {
            WindowLeft = left;
            WindowTop = top;
        }

        public void SetWindowSize(int width, int height)
        {
            WindowWidth = width;
            WindowHeight = height;
        }

        #endregion

        #region IConsoleCursor

        public int CursorSize { get; set; }
        public bool CursorVisible { get; set; }
        public int CursorLeft { get; set; }
        public int CursorTop { get; set; }

        public void SetCursorPosition(int left, int top)
        {
            CursorLeft = left;
            CursorTop = top;
        }

        #endregion
    }
}