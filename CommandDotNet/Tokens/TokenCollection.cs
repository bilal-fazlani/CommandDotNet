using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Tokens
{
    /// <summary>A collection of tokens parsed from command line arguments</summary>
    public class TokenCollection : IReadOnlyCollection<Token>
    {
        private readonly List<Token> _directives;
        private readonly List<Token> _arguments;
        private readonly List<Token> _separated;
        private readonly List<Token> _combined = new List<Token>();

        /// <summary>The total count of all tokens.  Includes argument separators if they exist.</summary>
        public int Count => _combined.Count;

        /// <summary>The Directives in this collection</summary>
        public IReadOnlyCollection<Token> Directives => _directives.AsReadOnly();
        /// <summary>The Arguments in this collection</summary>
        public IReadOnlyCollection<Token> Arguments => _arguments.AsReadOnly();
        /// <summary>The Arguments located after the first argument separator "--"</summary>
        public IReadOnlyCollection<Token> Separated => _separated.AsReadOnly();

        /// <summary>The indexer for this collection.  Includes argument separators if they exist.</summary>
        public Token this[int index] => _combined[index];

        private TokenCollection(IEnumerable<Token> directives, IEnumerable<Token> arguments, IEnumerable<Token> separated)
        {
            _directives = directives.ToList();
            if (_directives.Any())
            {
                _combined.AddRange(_directives);
            }

            _arguments = arguments.ToList();
            if (_arguments.Any())
            {
                _combined.AddRange(_arguments);
            }

            _separated = separated.ToList();
            if (_separated.Any())
            {
                _combined.Add(Tokenizer.SeparatorToken);
                _combined.AddRange(_separated);
            }
        }

        internal TokenCollection(IEnumerable<Token> tokens)
        {
            _directives = new List<Token>();
            _arguments = new List<Token>();
            _separated = new List<Token>();

            var directives = _directives;
            var arguments = _arguments;
            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    case TokenType.Directive:
                        directives.Add(token);
                        _combined.Add(token);
                        break;
                    case TokenType.Option:
                        arguments.Add(token);
                        _combined.Add(token);
                        break;
                    case TokenType.Value:
                        arguments.Add(token);
                        _combined.Add(token);
                        break;
                    case TokenType.Separator:
                        if (directives != _separated)
                        {
                            // all additional arguments will be
                            // added to the separated bucket
                            directives = _separated;
                            arguments = _separated;
                        }
                        else
                        {
                            // if multiple separators are provided
                            //   assume nested command execution and 
                            //   retain the additional separators
                            _separated.Add(token);
                        }
                        _combined.Add(token);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>A convenience method making it easier to transform a subset of the tokens by type</summary>
        public TokenCollection Transform(
            Func<Token, IEnumerable<Token>> transformation,
            bool skipDirectives = false,
            bool skipArguments = false,
            bool skipSeparated = false)
        {
            Func<Token, IEnumerable<Token>> AssignSourceTokenToNewTokens()
            {
                return token => transformation(token)
                    .Select(t =>
                    {
                        if (t.SourceToken == null && t != token)
                        {
                            t.SourceToken = token;
                        }

                        return t;
                    });
            }

            return new TokenCollection(
                skipDirectives ? _directives : _directives.SelectMany(AssignSourceTokenToNewTokens()).ToList(),
                skipArguments ? _arguments : _arguments.SelectMany(AssignSourceTokenToNewTokens()).ToList(),
                skipSeparated ? _separated : _separated.SelectMany(AssignSourceTokenToNewTokens()).ToList()
            );
        }

        public IEnumerator<Token> GetEnumerator()

        {
            return _combined.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}