using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Warpstone.Internal.Input;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that matches regular expressions in the input.
    /// </summary>
    internal sealed class RegexParserImpl : ParserImplementationBase<RegexParser, string>
    {
        private readonly string expected;

        // Literal text that every match starts with, so most failing positions are rejected without running the regex.
        private readonly string prefix;

        // The first text this regex matched, reused for later matches with the same text (see StringParseInput.Match).
        private string? first;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexParserImpl"/> class.
        /// </summary>
        /// <param name="pattern">The pattern to be matched.</param>
        /// <param name="options">The options used by the regex engine.</param>
        /// <param name="timeout">The timeout for the regex engine.</param>
        public RegexParserImpl([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options, TimeSpan timeout)
        {
            var o = options | RegexOptions.ExplicitCapture;
            expected = Regex.Escape(pattern);
            prefix = FindLiteralPrefix(pattern, options);
            if (timeout == TimeSpan.Zero || timeout == TimeSpan.MaxValue)
            {
                Regex = new Regex(@$"\G({pattern})", o);
            }
            else
            {
                Regex = new Regex(@$"\G({pattern})", o, timeout);
            }
        }

        /// <summary>
        /// The regular expression engine.
        /// </summary>
        public Regex Regex { get; }

        private InputMatch Match(IParseInput input, int position, out string? match)
            => input is StringParseInput stringInput
            ? stringInput.Match(position, Regex, prefix, ref first, out match)
            : input.Match(position, Regex, out match);

        /// <summary>
        /// Finds the plain letters, digits and underscores a pattern starts with, such as <c>if</c> in <c>if\b</c>.
        /// Conservative: any alternation or option that changes how letters match gives no prefix.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="options">The regex options.</param>
        /// <returns>The literal prefix, or an empty string.</returns>
        private static string FindLiteralPrefix(string pattern, RegexOptions options)
        {
            if ((options & (RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.RightToLeft)) != 0
                || pattern.IndexOf('|') >= 0)
            {
                return string.Empty;
            }

            var end = 0;
            while (end < pattern.Length && pattern[end] is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '_')
            {
                end++;
            }

            // A quantifier applies to the last character only, so that one is optional.
            if (end > 0 && end < pattern.Length && pattern[end] is '*' or '+' or '?' or '{')
            {
                end--;
            }

            return pattern.Substring(0, end);
        }

        /// <inheritdoc />
        protected override void InitializeInternal(RegexParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            return Match(context.Input, position, out var match) == InputMatch.Success
                ? new(position, match!.Length, match)
                : ReportExpected(context, position, expected);
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            switch (Match(context.Input, position, out var match))
            {
                case InputMatch.Success:
                    context.ResultStack.Push(new(position, match!.Length, match));
                    break;
                case InputMatch.Pending:
                    context.ExecutionStack.Push((position, this));
                    break;
                default:
                    context.ResultStack.Push(ReportExpected(context, position, expected));
                    break;
            }
        }
    }
}
