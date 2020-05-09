using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools.Prompts
{
    public static class ConsoleKeyInfos
    {
        public static ConsoleKeyInfo BackSpaceKey = new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false);
        public static ConsoleKeyInfo EnterKey = new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false);
        public static ConsoleKeyInfo CtrlCKey = new ConsoleKeyInfo('c', ConsoleKey.C, false, false, true);
        public static ConsoleKeyInfo EscapeKey = new ConsoleKeyInfo(' ', ConsoleKey.Escape, false, false, false);

        public static IEnumerable<ConsoleKeyInfo> AppendEnterKey(this IEnumerable<ConsoleKeyInfo> consoleKeyInfos) =>
            consoleKeyInfos.Concat(EnterKey.ToEnumerable());

        public static IEnumerable<ConsoleKeyInfo> AppendCtrlCKey(this IEnumerable<ConsoleKeyInfo> consoleKeyInfos) =>
            consoleKeyInfos.Concat(CtrlCKey.ToEnumerable());

        public static IEnumerable<ConsoleKeyInfo> AppendEscapeKey(this IEnumerable<ConsoleKeyInfo> consoleKeyInfos) =>
            consoleKeyInfos.Concat(EscapeKey.ToEnumerable());
    }
}