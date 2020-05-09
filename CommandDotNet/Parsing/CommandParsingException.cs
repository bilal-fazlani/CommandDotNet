// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace CommandDotNet.Parsing
{
    internal class CommandParsingException : Exception
    {
        public Command Command { get; }

        public CommandParsingException(Command command, string message)
            : base(message)
        {
            Command = command;
        }
    }
}