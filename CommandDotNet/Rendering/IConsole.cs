// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied from System.CommandLine 

using System;
using System.Collections.Generic;
using CommandDotNet.Execution;
using CommandDotNet.Prompting;

namespace CommandDotNet.Rendering
{
    public interface IConsole :
        IStandardOut, IStandardError, IStandardIn,
        IConsoleColor, IConsoleBuffer, IConsoleWindow, IConsoleCursor
    {
        string Title { get; set; }
    }
}