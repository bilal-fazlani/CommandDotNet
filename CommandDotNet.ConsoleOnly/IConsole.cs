// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied from System.CommandLine 

using System;

namespace CommandDotNet.ConsoleOnly
{
    public interface IConsole :
        IStandardError, IStandardIn, IStandardOut,
        IConsoleBuffer, IConsoleColor, IConsoleCursor, IConsoleWindow
    {
        string Title { get; set; }
    }

    public interface IStandardError
    {
        IConsoleWriter Error { get; }

        bool IsErrorRedirected { get; }
    }

    public interface IStandardIn
    {
        IConsoleReader In { get; }

        bool IsInputRedirected { get; }
    }

    public interface IStandardOut
    {
        IConsoleWriter Out { get; }
        bool IsOutputRedirected { get; }
    }

    public interface IConsoleBuffer
    {
        int BufferWidth { get; set; }
        int BufferHeight { get; set; }
        void SetBufferSize(int width, int height);
        void Clear();
    }

    public interface IConsoleColor
    {
        ConsoleColor BackgroundColor { get; set; }
        ConsoleColor ForegroundColor { get; set; }
        void ResetColor();
    }

    public interface IConsoleCursor
    {
        int CursorSize { get; set; }
        bool CursorVisible { get; set; }
        int CursorLeft { get; set; }
        int CursorTop { get; set; }
        void SetCursorPosition(int left, int top);
    }

    public interface IConsoleWindow
    {
        int WindowLeft { get; set; }
        int WindowTop { get; set; }
        int WindowWidth { get; set; }
        int WindowHeight { get; set; }
        void SetWindowPosition(int left, int top);
        void SetWindowSize(int width, int height);
    }
}