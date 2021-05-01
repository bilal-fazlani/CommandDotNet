// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied from System.CommandLine 

using System;

namespace CommandDotNet.ConsoleOnly
{
    public class SystemConsole : IConsole
    {
        public string Title
        {
            get => Console.Title;
            set => Console.Title = value;
        }

        #region IStandardError

        public IConsoleWriter Error { get; } = ConsoleWriter.Create(Console.Error);

        public bool IsErrorRedirected => Console.IsErrorRedirected;

        #endregion

        #region IStandardOut

        public IConsoleWriter Out { get; } = ConsoleWriter.Create(Console.Out);

        public bool IsOutputRedirected => Console.IsOutputRedirected;

        #endregion

        #region IStandardIn

        public IConsoleReader In { get; } = new SystemConsoleReader();

        public bool IsInputRedirected => Console.IsInputRedirected;

        #endregion

        #region IConsoleColor

        public ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public void ResetColor() => Console.ResetColor();

        #endregion

        #region IConsoleBuffer

        public int BufferWidth
        {
            get => Console.BufferWidth;
            set => Console.BufferWidth = value;
        }

        public int BufferHeight
        {
            get => Console.BufferHeight;
            set => Console.BufferHeight = value;
        }

        public void SetBufferSize(int width, int height) => Console.SetBufferSize(width, height);

        public void Clear() => Console.Clear();

        #endregion

        #region IConsoleWindow

        public int WindowLeft
        {
            get => Console.WindowLeft;
            set => Console.WindowLeft = value;
        }

        public int WindowTop
        {
            get => Console.WindowTop;
            set => Console.WindowTop = value;
        }

        public int WindowWidth
        {
            get => Console.WindowWidth;
            set => Console.WindowWidth = value;
        }

        public int WindowHeight
        {
            get => Console.WindowHeight;
            set => Console.WindowHeight = value;
        }

        public void SetWindowPosition(int left, int top) => Console.SetWindowPosition(left, top);

        public void SetWindowSize(int width, int height) => Console.SetBufferSize(width, height);

        #endregion

        #region IConsoleCursor

        public int CursorSize
        {
            get => Console.CursorSize;
            set => Console.CursorSize = value;
        }

        public bool CursorVisible
        {
            get => Console.CursorVisible;
            set => Console.CursorVisible = value;
        }

        public int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        public void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);

        #endregion
    }
}