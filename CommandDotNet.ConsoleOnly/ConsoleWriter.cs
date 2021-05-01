// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied from System.CommandLine 

using System;
using System.IO;
using static System.Environment;

namespace CommandDotNet.ConsoleOnly
{
    public static class ConsoleWriter
    {
        // the WriteLine extension methods will be frequently used by developers
        // keep class in CommandDotNet namespace to avoid force reference to Rendering namespace

        public static IConsoleWriter Create(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            return new AnonymousConsoleWriter(writer.Write);
        }

        public static void WriteLine(this IConsoleWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            writer.Write(NewLine);
        }

        public static void WriteLine(this IConsoleWriter writer, object? value)
        {
            WriteLine(writer, value?.ToString());
        }

        public static void WriteLine(this IConsoleWriter writer, string? value)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.Write(value);
            writer.WriteLine();
        }

        public static void Write(this IConsoleWriter writer, object? value)
        {
            writer.Write(value?.ToString());
        }

        private class AnonymousConsoleWriter : IConsoleWriter
        {
            private readonly Action<string?> _write;

            public AnonymousConsoleWriter(Action<string?> write)
            {
                _write = write;
            }

            public void Write(string? value)
            {
                _write(value);
            }
        }
    }
}