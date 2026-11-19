using System.Collections.Generic;
using System.Globalization;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that parses a string.
    /// </summary>
    internal sealed class StringParserImpl : ParserImplementationBase<StringParser, string>
    {
        private readonly string value;
        private readonly CultureInfo culture;
        private readonly CompareOptions options;
        private readonly string expected;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringParserImpl"/> class.
        /// </summary>
        /// <param name="value">The string to be parsed.</param>
        /// <param name="culture">The culture used for comparing.</param>
        /// <param name="options">The options used for comparing.</param>
        public StringParserImpl(string value, CultureInfo culture, CompareOptions options)
        {
            this.value = value;
            this.culture = culture;
            this.options = options;
            this.expected = @$"""{value}""";
        }

        /// <inheritdoc />
        public override FirstChars? GetFirstChars(ISet<IParserImplementation> visiting)
        {
            // Culture comparisons can match other first characters (ignorable ones, for one), and so can a letter with ignore case.
            var ordinal = options == CompareOptions.Ordinal
                || (options == CompareOptions.OrdinalIgnoreCase && value.Length > 0 && !char.IsLetter(value[0]));
            return ordinal && value.Length > 0
                ? FirstChars.Leaf([value[0]], this, expected)
                : null;
        }

        /// <inheritdoc />
        protected override void InitializeInternal(StringParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            switch (context.Input.Match(position, value, culture, options, out var match))
            {
                case InputMatch.Success:
                    context.ResultStack.Push(new(position, value.Length, match));
                    break;
                case InputMatch.Pending:
                    context.ExecutionStack.Push((position, this));
                    break;
                default:
                    context.ResultStack.Push(ReportExpected(context, position, expected));
                    break;
            }
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            return context.Input.Match(position, value, culture, options, out var match) == InputMatch.Success
                ? new(position, value.Length, match)
                : ReportExpected(context, position, expected);
        }
    }
}
