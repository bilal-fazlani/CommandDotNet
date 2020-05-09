// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied from System.CommandLine 

using System;

namespace CommandDotNet.Rendering
{
    public interface IConsole :
        IStandardOut,
        IStandardError,
        IStandardIn
    {
        /// <summary>
        /// Read a key from the input
        /// </summary>
        /// <param name="intercept">When true, the key is not displayed in the output</param>
        /// <returns></returns>
        ConsoleKeyInfo ReadKey(bool intercept);

        /// <summary>
        /// Gets or Sets value determine whether Ctrl+C is treated as ordinary input.<br/>
        /// When using System.Console, set this to true before <see cref="ReadKey"/> or
        /// Ctrl+C is not captured and does not interrupt.
        /// </summary>
        bool TreatControlCAsInput { get; set; }
    }

    public interface IStandardOut
    {
        IStandardStreamWriter Out { get; }

        bool IsOutputRedirected { get; }
    }

    public interface IStandardError
    {
        IStandardStreamWriter Error { get; }

        bool IsErrorRedirected { get; }
    }

    public interface IStandardStream
    {
    }

    public interface IStandardIn : IStandardStream
    {
        IStandardStreamReader In { get; }

        bool IsInputRedirected { get; }
    }

    public interface IStandardStreamWriter : IStandardStream
    {
        void Write(string? value);
    }

    public interface IStandardStreamReader : IStandardStream
    {
        string? ReadLine();
        string? ReadToEnd();
    }
}