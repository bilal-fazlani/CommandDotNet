using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet
{
    /// <summary>The text values provided as input to the application</summary>
    public class InputValue
    {
        /// <summary>
        /// The source of these values<br/>
        /// Sources provided by this framework can be found at <see cref="Constants.InputValueSources"/>
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// True if the value is from a stream and should only be enumerated in within the command.
        /// </summary>
        public bool IsStream { get; }

        private IEnumerable<string>? _values;

        /// <summary>The text values</summary>
        public IEnumerable<string>? Values
        {
            get => _values ?? ValuesFromTokens?.Select(v => v.Value);
            set => _values = value;
        }

        /// <summary>The values with tokens of origin</summary>
        public IEnumerable<ValueFromToken>? ValuesFromTokens { get; set; }

        public InputValue(string source, bool isStream, IEnumerable<string> values)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            IsStream = isStream;
            Values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public InputValue(string source, bool isStream, IEnumerable<ValueFromToken> values)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            IsStream = isStream;
            ValuesFromTokens = values ?? throw new ArgumentNullException(nameof(values));
        }
    }
}