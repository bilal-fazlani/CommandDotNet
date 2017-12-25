﻿using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Exceptions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

[assembly: InternalsVisibleTo("CommandDotNet.Tests")]

namespace CommandDotNet
{
    /// <summary>
    /// Creates a new instance of AppRunner
    /// </summary>
    /// <typeparam name="T">Type of the application</typeparam>
    public class AppRunner<T> where T : class
    {
        private readonly AppSettings _settings;

        public AppRunner(AppSettings settings = null)
        {
            _settings = settings ?? new AppSettings();
        }

        /// <summary>
        /// Exceutes the specified command with given parameters
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>If target method returns int, this method will return that value. Else, 
        /// it will return 0 in case of successs and 1 in case of unhandled exception</returns>
        public int Run(params string[] args)
        {
            try
            {
                AppCreator appCreator = new AppCreator(_settings);

                CommandLineApplication app = appCreator.CreateApplication(typeof(T));

                var parsedArguments = ArgumentParser.SplitFlags(args).ToArray();

                return app.Execute(parsedArguments);
            }
            catch (AppRunnerException e)
            {
                Console.Error.WriteLine(e.Message + "\n");
#if DEBUG
                Console.Error.WriteLine(e.StackTrace);
#endif
                return 1;
            }
            catch (CommandParsingException e)
            {
                Console.Error.WriteLine(e.Message + "\n");
                e.Command.ShowHelp();

#if DEBUG
                Console.Error.WriteLine(e.StackTrace);
#endif

                return 1;
            }
            catch (AggregateException e)
            {
                foreach (var innerException in e.InnerExceptions)
                {
                    Console.Error.WriteLine(innerException.GetBaseException().Message + "\n");
#if DEBUG
                    Console.Error.WriteLine(innerException.GetBaseException().StackTrace);
#endif
                    Console.Error.WriteLine("-----------------------------------------------------------------");
                }
                return 1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message + "\n");
#if DEBUG
                Console.Error.WriteLine(e.StackTrace);
#endif
                return 1;
            }
        }
    }
}