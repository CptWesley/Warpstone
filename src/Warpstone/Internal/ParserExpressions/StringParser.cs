using System;
using System.Collections.Generic;
using System.Globalization;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that parses a string.
    /// </summary>
    internal sealed class StringParser : ParserBase<string>
    {
        private static readonly int baseHash = typeof(StringParser).GetHashCode() * 31;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringParser"/> class.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="culture">The culture used for comparing.</param>
        /// <param name="options">The options used for comparing.</param>
        public StringParser(string str, CultureInfo culture, CompareOptions options)
        {
            String = str;
            Culture = culture;
            Options = options;
        }

        /// <summary>
        /// The string to be parsed.
        /// </summary>
        public string String { get; }

        /// <summary>
        /// The culture used for comparing.
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        /// The options used for comparing.
        /// </summary>
        public CompareOptions Options { get; }

        /// <inheritdoc />
        public override IParserImplementation<string> CreateUninitializedImplementation()
            => new StringParserImpl(String, Culture, Options);

        /// <inheritdoc />
        public override void EmitSource(SourceEmitter emitter)
        {
            var literal = emitter.Literal(String);
            if (Options == CompareOptions.Ordinal)
            {
                emitter.Open($"if (input.Length - pos >= {String.Length} && string.CompareOrdinal(input, pos, {literal}, 0, {String.Length}) == 0)");
                emitter.Value(literal);
            }
            else
            {
                var culture = Culture.Equals(CultureInfo.InvariantCulture)
                    ? "global::System.Globalization.CultureInfo.InvariantCulture"
                    : emitter.Static("global::System.Globalization.CultureInfo", $"global::System.Globalization.CultureInfo.GetCultureInfo({emitter.Literal(Culture.Name)})");
                emitter.Open($"if (input.Length - pos >= {String.Length} && string.Compare(input, pos, {literal}, 0, {String.Length}, {culture}, (global::System.Globalization.CompareOptions){(int)Options}) == 0)");
                emitter.Value($"input.Substring(pos, {String.Length})");
            }

            emitter.Line($"end = pos + {String.Length};");
            emitter.Line("return true;");
            emitter.Close();
            emitter.Fail(@$"""{String}""");
        }

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge)
        {
            return String.Length == 0;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is StringParser other
            && other.String == String
            && Equals(other.Culture, Culture)
            && other.Options == Options;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = baseHash;
            hash = (hash * 31) + String.GetHashCode();
            hash = (hash * 31) + Culture.GetHashCode();
            hash = (hash * 31) + Options.GetHashCode();
            return hash;
        }
    }
}
