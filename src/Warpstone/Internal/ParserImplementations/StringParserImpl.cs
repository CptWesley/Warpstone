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
        private readonly bool useValue;

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
            this.useValue = options is CompareOptions.Ordinal;
        }

        /// <inheritdoc />
        protected override void InitializeInternal(StringParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            var input = context.Input.Content;

            if (!Matches(input, position))
            {
                context.ResultStack.Push(new UnsafeParseResult(position, [new UnexpectedTokenError(context, this, position, 1, expected)]));
            }
            else
            {
                context.ResultStack.Push(new(position, value.Length, useValue ? value : input.Substring(position, value.Length)));
            }
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            var input = context.Input.Content;

            if (!Matches(input, position))
            {
                return new(position, [new UnexpectedTokenError(context, this, position, 1, expected)]);
            }
            else
            {
                return new(position, value.Length, useValue ? value : input.Substring(position, value.Length));
            }
        }

        private bool Matches(string input, int position)
        {
            var endPos = position + value.Length;

            if (endPos > input.Length)
            {
                return false;
            }

            var result = string.Compare(input, position, value, 0, value.Length, culture, options);
            return result == 0;
        }
    }
}
