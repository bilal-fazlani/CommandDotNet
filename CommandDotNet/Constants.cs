namespace CommandDotNet
{
    public static class Constants
    {
        public static readonly string HelpOptionName = "help";
        public static readonly string VersionOptionName = "version";

        /// <summary>The <see cref="InputValue.Source"/>es defined by this framework</summary>
        public static class InputValueSources
        {
            /// <summary>Values provided in the shell by the user or script</summary>
            public static readonly string Argument = "argument";

            /// <summary>Values piped into the application</summary>
            public static readonly string Piped = "piped";

            /// <summary>Values provided via user prompt</summary>
            public static readonly string Prompt = "prompt";
        }

        internal static class TypeDisplayNames
        {
            public static readonly string Flag = "";

            public static readonly string Number = "Number";

            public static readonly string Boolean = "Boolean";

            public static readonly string DoubleNumber = "Double";

            public static readonly string DecimalNumber = "Decimal";

            public static readonly string Character = "Character";

            public static readonly string Text = "Text";
        }
    }
}