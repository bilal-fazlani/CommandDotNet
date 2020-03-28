// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied from System.CommandLine 

using System;
using System.IO;
using CommandDotNet.Rendering;

namespace CommandDotNet
{
    public static class StandardStreamWriter
    {
        // the WriteLine extension methods will be frequently used by developers
        // keep class in CommandDotNet namespace to avoid force reference to Rendering namespace

        public static IStandardStreamWriter Create(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            return new AnonymousStandardStreamWriter(writer.Write);
        }

        public static void WriteLine(this IStandardStreamWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.Write(Environment.NewLine);
        }

        public static void WriteLine(this IStandardStreamWriter writer, object value)
        {
            writer.WriteLine(value?.ToString());
        }

        public static void WriteLine(this IStandardStreamWriter writer, string value)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.Write(value);
            writer.Write(Environment.NewLine);
        }

        private class AnonymousStandardStreamWriter : IStandardStreamWriter
        {
            private readonly Action<string> _write;

            public AnonymousStandardStreamWriter(Action<string> write)
            {
                _write = write;
            }

            public void Write(string value)
            {
                _write(value);
            }
        }
    }
}