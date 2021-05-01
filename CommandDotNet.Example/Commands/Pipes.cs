using System;
using System.Collections.Generic;
using System.Threading;
using CommandDotNet.ConsoleOnly;

namespace CommandDotNet.Example.Commands
{
    [Command(
        Description = "demonstrates accepting piped input",
        Usage = "`pipes echo --times 10 hello | pipes echo` to see piped input printed again\n" +
                "`pipes echo --times 10 --sleep 10 hello | pipes echo` to see that piped input can be streamed as available")]
    public class Pipes
    {
        [Command(
            Usage = "`pipes echo --times 10 hello | pipes echo` to see piped input printed again\n"
                    + "`pipes echo --times 10 --sleep 10 hello | pipes echo` to see that piped input can be streamed")]
        public void Echo(
            IConsole console,
            CancellationToken cancellationToken,
            [Operand] IEnumerable<string> inputs, 
            [Option] int times = 1, 
            [Option(Description = "sleep N seconds between each echo")] int sleep = 0)
        {
            if (inputs == null)
            {
                return;
            }

            foreach (var input in inputs)
            {
                for (int i = 0; i < times; i++)
                {
                    console.Out.WriteLine(input);
                    if (sleep > 0)
                    {
                        cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(sleep));
                    }
                }
            }
        }
    }
}