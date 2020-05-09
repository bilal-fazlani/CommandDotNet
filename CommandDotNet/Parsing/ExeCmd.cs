using System;
using System.Diagnostics;
using System.Text;
using CommandDotNet.Rendering;

namespace CommandDotNet.Parsing
{
    internal class ExeCmd
    {
        private readonly StringBuilder _stdOut = new StringBuilder();
        private readonly StringBuilder _stdErr = new StringBuilder();
        private readonly IConsole? _console;

        internal string FileName { get; }
        internal string Arguments { get; }
        internal int? ExitCode { get; private set; }
        internal Exception Error { get; private set; }
        internal bool Succeeded => ExitCode.GetValueOrDefault() == 0 && Error is null;

        private ExeCmd(string fileName, string arguments, 
            IConsole? console = null)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            _console = console;
        }

        public string StdOut => _stdOut.ToString();
        public string StdErr => _stdErr.ToString();

        public static bool TryExecute(
            string fileName, string arguments, out ExeCmd exeCmd, 
            IConsole console = null)
        {
            exeCmd = new ExeCmd(fileName, arguments, console);
            exeCmd.Execute();
            return exeCmd.Succeeded;
        }
        
        private void Execute()
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FileName,
                    Arguments = Arguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            
            process.OutputDataReceived += (sender, args) => _stdOut.Append(args.Data);
            process.ErrorDataReceived += (sender, args) => _stdErr.Append(args.Data);
            if (_console != null)
            {
                process.OutputDataReceived += (sender, args) => _console.Out.Write(args.Data);
                process.ErrorDataReceived += (sender, args) => _console.Error.Write(args.Data);
            }
            
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                ExitCode = process.ExitCode;
            }
            catch (Exception e)
            {
                Error = e;
            }
        }
    }
}