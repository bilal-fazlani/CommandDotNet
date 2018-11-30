﻿using System;
using CommandDotNet.Exceptions;
using CommandDotNet.HelpGeneration;

namespace CommandDotNet.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
            if(BooleanMode == BooleanMode.Unknown)
                throw new AppRunnerException("BooleanMode can not be set to BooleanMode.Unknown explicitly");
        }
        public BooleanMode BooleanMode { get; set; } = BooleanMode.Implicit;

        public bool ThrowOnUnexpectedArgument { get; set; } = true;
        
        public bool AllowArgumentSeparator { get; set; }

        public ArgumentMode MethodArgumentMode { get; set; } = ArgumentMode.Parameter;

        public Case Case { get; set; } = Case.DontChange;

        public bool EnableVersionOption { get; set; } = true;
        
        public bool PrompForArgumentsIfNotProvided { get; set; }

        public HelpTextStyle HelpTextStyle { get; set; } = HelpTextStyle.Detailed;

        public Func<RunContext, object> OnRun { get; set; }

        internal IHelpProvider CustomHelpProvider { get; set; }
    }
}