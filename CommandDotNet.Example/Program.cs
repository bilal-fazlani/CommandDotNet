﻿using System;
using System.Linq;
using CommandDotNet.Attributes;
using CommandDotNet.Example.Issues;
using CommandDotNet.Models;

namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.FirstOrDefault() == "echo-args")
            {
                foreach (var arg in args.Skip(1))
                {
                    Console.Out.WriteLine(arg);
                }
                return 0;
            }
            // return Run<GitApplication>(args);
            return Run<Examples>(args);
        }

        private static int Run<TApp>(string[] args) where TApp : class
        {
            AppRunner<TApp> appRunner = new AppRunner<TApp>(new AppSettings()
            {
                Case = Case.KebabCase,
                Help =
                {
                    TextStyle = HelpTextStyle.Detailed
                }
            });
            return appRunner.Run(args);
        }
    }

    public class Examples
    {
        [SubCommand]
        public GitApplication GitApplication { get; set; }
        
        [SubCommand]
        public ModelApp ModelApp { get; set; }
        
        [SubCommand]
        public MyApplication MyApplication { get; set; }

        [SubCommand]
        public PipedInputExampleApp PipedInputExampleApp { get; set; }

        [SubCommand]
        public IssueApps IssueApps { get; set; }
    }
}