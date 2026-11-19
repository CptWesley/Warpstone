using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that matches regular expressions in the input.
    /// </summary>
    internal sealed class RegexParser : ParserBase<string>
    {
        private static readonly int baseHash = typeof(RegexParser).GetHashCode() * 31;
        private readonly Lazy<RegexParserImpl> impl;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexParser"/> class.
        /// </summary>
        /// <param name="pattern">The pattern to be matched.</param>
        /// <param name="options">The options used by the regex engine.</param>
        /// <param name="timeout">The timeout of this regex engine.</param>
        public RegexParser([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options, TimeSpan timeout)
        {
            Pattern = pattern;
            Options = options;
            Timeout = timeout;
            impl = new(() => new RegexParserImpl(pattern, options, timeout));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexParser"/> class.
        /// </summary>
        /// <param name="pattern">The pattern to be matched.</param>
        /// <param name="options">The options used by the regex engine.</param>
        public RegexParser([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options)
            : this(pattern, options, TimeSpan.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexParser"/> class.
        /// </summary>
        /// <param name="pattern">The pattern to be matched.</param>
        public RegexParser([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
            : this(pattern, RegexOptions.Compiled)
        {
        }

        /// <summary>
        /// The expected pattern.
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// Gets the string comparison method.
        /// </summary>
        public RegexOptions Options { get; }

        /// <summary>
        /// The timeout of this regex engine.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <inheritdoc />
        public override IParserImplementation<string> CreateUninitializedImplementation()
            => impl.Value;

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge)
            => impl.Value.Regex.IsMatch(string.Empty);

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is RegexParser other
            && other.Pattern == Pattern
            && other.Options == Options;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = baseHash;
            hash = (hash * 31) + Pattern.GetHashCode();
            hash = (hash * 31) + Options.GetHashCode();
            return hash;
        }
    }
}
