namespace CommandDotNet.Execution
{
    public static class MiddlewareSteps
    {
        /// <summary>Runs first in the <see cref="MiddlewareStages.PreTokenize"/> stage</summary>
        public static class DebugDirective
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.PreTokenize;
            public static readonly int Order = -1000;
        }

        public static class DependencyResolver
        {
            public static class BeginScope
            {
                public static readonly MiddlewareStages Stage = MiddlewareStages.PreTokenize;
                public static readonly int Order = -900;
            }
        }

        public static class Tokenize
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.Tokenize;
            public static readonly int Order = -1000;
        }

        public static class CreateRootCommand
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.Tokenize;
            public static readonly int Order = int.MaxValue - 1000;
        }

        public static class ParseInput
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.ParseInput;
            public static readonly int Order = 0;
        }

        public static class AutoSuggest
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.ParseInput;
            public static readonly int Order = ParseInput.Order + 100;
        }

        public static class TypoSuggest
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.ParseInput;
            public static readonly int Order = AutoSuggest.Order + 100;
        }

        public static class AssembleInvocationPipeline
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.ParseInput;
            public static readonly int Order = ParseInput.Order + 1000;
        }

        /// <summary>Runs before <see cref="Help"/> to ensure default values are included in the help output</summary>
        public static class Version
        {
            public static readonly MiddlewareStages Stage = Help.Stage;
            public static readonly int Order = Help.Order - 2000;
        }

        /// <summary>Runs before <see cref="Help"/> to ensure default values are included in the help output</summary>
        public static class SetArgumentDefaults
        {
            public static readonly MiddlewareStages Stage = Help.Stage;
            public static readonly int Order = Help.Order - 1000;
        }

        /// <summary>Runs at the end of the <see cref="MiddlewareStages.ParseInput"/> stage</summary>
        public static class Help
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.ParseInput;
            public static readonly int Order = int.MaxValue;
        }

        public static class PipedInput
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.PostParseInputPreBindValues;
            public static readonly int Order = -1;
        }

        /// <summary>
        /// Runs late in the <see cref="MiddlewareStages.PostParseInputPreBindValues"/>
        /// stage to enable other middleware to populate arguments
        /// </summary>
        public static class ValuePromptMissingArguments
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.PostParseInputPreBindValues;
            public static readonly int Order = int.MaxValue - 100;
        }

        public static class BindValues
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.BindValues;
            public static readonly int Order = 0;
        }

        public static class ResolveCommandClasses
        {
            public static readonly MiddlewareStages Stage = MiddlewareStages.BindValues;
            public static readonly int Order = BindValues.Order + 100;
        }
    }
}