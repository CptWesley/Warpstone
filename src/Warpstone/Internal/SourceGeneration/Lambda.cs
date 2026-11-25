using System;
using System.Collections.Generic;
using System.Linq;

namespace Warpstone.Internal.SourceGeneration
{
    /// <summary>
    /// Splits the source of a lambda expression into its parameters and body,
    /// so the generated source can declare it as a static method instead of calling it through a delegate.
    /// </summary>
    internal static class Lambda
    {
        /// <summary>
        /// Tries to split <paramref name="source"/> into parameters and a body.
        /// </summary>
        /// <param name="source">The source of the lambda.</param>
        /// <param name="count">The number of parameters the delegate has.</param>
        /// <param name="parameters">The parameters: names, or explicit declarations like <c>int x</c>. Discards get unique names.</param>
        /// <param name="body">The expression or block body.</param>
        /// <returns><see langword="true"/> if <paramref name="source"/> is a lambda with <paramref name="count"/> parameters.</returns>
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        public static bool TryParse(string source, int count, out string[] parameters, out string body)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            parameters = [];
            body = string.Empty;
            var text = source.Trim();
            if (text.StartsWith("static ", StringComparison.Ordinal))
            {
                text = text.Substring("static ".Length).TrimStart();
            }

            if (text.StartsWith("async ", StringComparison.Ordinal))
            {
                return false;
            }

            string rest;
            if (text.StartsWith("(", StringComparison.Ordinal))
            {
                var close = FindClose(text, 0);
                if (close < 0)
                {
                    return false;
                }

                var inside = text.Substring(1, close - 1).Trim();
                parameters = inside.Length == 0 ? [] : Split(inside).Select(p => p.Trim()).ToArray();
                rest = text.Substring(close + 1).TrimStart();
            }
            else
            {
                var length = 0;
                while (length < text.Length && (char.IsLetterOrDigit(text[length]) || text[length] is '_' or '@'))
                {
                    length++;
                }

                if (length == 0)
                {
                    return false;
                }

                parameters = [text.Substring(0, length)];
                rest = text.Substring(length).TrimStart();
            }

            if (!rest.StartsWith("=>", StringComparison.Ordinal) || parameters.Length != count || parameters.Any(p => p.Length == 0))
            {
                return false;
            }

            // Several discards are not parameters, so they get names the body can't use.
            if (parameters.Count(p => p == "_") > 1)
            {
                parameters = parameters.Select((p, i) => p == "_" ? $"__discard{i}" : p).ToArray();
            }

            body = rest.Substring(2).Trim();
            return body.Length > 0;
        }

        /// <summary>
        /// Gets whether <paramref name="source"/> looks like a method group, such as <c>int.Parse</c>, which can be called directly.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns><see langword="true"/> if it is a dotted name.</returns>
        public static bool IsMethodGroup(string source)
        {
            var text = source.Trim();

            // A single name may be a local delegate variable, which the generated source can't see; keep it a field so the error is clear.
            return text.IndexOf('.') >= 0
                && text.All(c => char.IsLetterOrDigit(c) || c is '_' or '.' or '@')
                && !text.EndsWith(".", StringComparison.Ordinal);
        }

        private static int FindClose(string text, int open)
        {
            var depth = 0;
            for (var i = open; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '(':
                        depth++;
                        break;
                    case ')':
                        depth--;
                        if (depth == 0)
                        {
                            return i;
                        }

                        break;
                }
            }

            return -1;
        }

        private static IEnumerable<string> Split(string parameters)
        {
            var depth = 0;
            var start = 0;
            for (var i = 0; i < parameters.Length; i++)
            {
                switch (parameters[i])
                {
                    case '(' or '<' or '[':
                        depth++;
                        break;
                    case ')' or '>' or ']':
                        depth--;
                        break;
                    case ',' when depth == 0:
                        yield return parameters.Substring(start, i - start);
                        start = i + 1;
                        break;
                }
            }

            yield return parameters.Substring(start);
        }
    }
}
