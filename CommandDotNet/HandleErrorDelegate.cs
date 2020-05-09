using System;

namespace CommandDotNet
{
    public delegate int HandleErrorDelegate(CommandContext? ctx, Exception exception);
}