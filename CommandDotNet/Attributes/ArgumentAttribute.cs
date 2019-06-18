﻿using System;

namespace CommandDotNet.Attributes
{
    [Obsolete("Use OperandAttribute instead")]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class ArgumentAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}