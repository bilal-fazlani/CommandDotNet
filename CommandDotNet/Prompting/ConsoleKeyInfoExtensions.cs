using System;
using System.Text;

namespace CommandDotNet.Prompting
{
    public static class ConsoleKeyInfoExtensions
    { 
        private const string ControlMod = "Ctrl+";
        private const string AltMod = "Alt+";
        private const string ShiftMod = "Shift+";

        public static string ToFriendlyName(this ConsoleKeyInfo info)
        {
            if (info.Modifiers == 0)
            {
                return info.Key.ToString();
            }

            var sb = new StringBuilder();
            if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                sb.Append(ControlMod);
            }

            if (info.Modifiers.HasFlag(ConsoleModifiers.Alt))
            {
                sb.Append(AltMod);
            }

            if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
            {
                sb.Append(ShiftMod);
            }

            sb.Append(info.Key);
            return sb.ToString();
        }
    }
}