using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandDotNet.Tokens
{
    internal static class ExpandResponseFilesTransformation
    {
        internal static AppRunner UseResponseFiles(AppRunner appRunner, int order = 1)
        {
            return appRunner.Configure(c => c.UseTokenTransformation("expand-response-files", order, Transform));
        }

        private static TokenCollection Transform(CommandContext commandContext, TokenCollection tokenCollection)
        {
            return tokenCollection.Transform(ExpandResponseFile, skipDirectives: true, skipSeparated: true);
        }

        private static IEnumerable<Token> ExpandResponseFile(Token token)
        {
            var filePath = GetFilePath(token);
            if (filePath == null)
            {
                yield return token;
            }
            else
            {
                var fullPath = Path.GetFullPath(filePath);
                if (!File.Exists(fullPath))
                {
                    throw new FileNotFoundException($"File not found: {fullPath}");
                }

                var splitter = CommandLineStringSplitter.Instance;
                var tokens = File
                    .ReadAllLines(filePath)
                    .Where(l => !l.IsCommentOrNullOrWhitespace())
                    .SelectMany(l => splitter.Split(l))
                    .Tokenize(includeDirectives: false);

                foreach (var newToken in tokens)
                {
                    yield return newToken;
                }
            }
        }

        private static string? GetFilePath(Token token)
        {
            return token.TokenType == TokenType.Value && token.Value.StartsWith("@") 
                ? token.Value.Substring(1) 
                : null;
        }

        private static bool IsCommentOrNullOrWhitespace(this string l)
        {
            return l.IsNullOrWhitespace() || l.StartsWith("#");
        }
    }
}