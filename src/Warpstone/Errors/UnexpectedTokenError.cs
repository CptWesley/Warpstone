namespace Warpstone.Errors
{
    /// <summary>
    /// Represents an error that occurs during parsing when an expected token is not found.
    /// </summary>
    public sealed class UnexpectedTokenError : ParseError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedTokenError"/> class.
        /// </summary>
        /// <param name="context">The context of the error.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="position">The position in the input string.</param>
        /// <param name="length">The length of the error.</param>
        /// <param name="expected">The expected input.</param>
        public UnexpectedTokenError(
            IReadOnlyParseContext context,
            IParserImplementation parser,
            int position,
            int length,
            string expected)
            : this(
                  context,
                  parser,
                  position,
                  length,
                  ImmutableSortedSet.Create(expected),
                  ImmutableSortedSet<UnexpectedTokenError>.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedTokenError"/> class.
        /// </summary>
        /// <param name="context">The context of the error.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="position">The position in the input string.</param>
        /// <param name="length">The length of the error.</param>
        /// <param name="expected">The expected input.</param>
        /// <param name="innerErrors">The errors that caused this error to occur.</param>
        public UnexpectedTokenError(
            IReadOnlyParseContext context,
            IParserImplementation parser,
            int position,
            int length,
            IEnumerable<string> expected,
            IEnumerable<UnexpectedTokenError> innerErrors)
            : this(
                  context,
                  parser,
                  position,
                  length,
                  expected.ToImmutableSortedSet(),
                  innerErrors.ToImmutableHashSet())
        {
        }

        private UnexpectedTokenError(
            IReadOnlyParseContext context,
            IParserImplementation parser,
            int position,
            int length,
            ImmutableSortedSet<string> expected,
            ImmutableHashSet<UnexpectedTokenError> innerErrors)
            : base(
                  context,
                  parser,
                  position,
                  length,
                  GetMessage(context, position, expected),
                  null)
        {
            Expected = expected;
            InnerErrors = innerErrors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedTokenError"/> class.
        /// </summary>
        /// <param name="context">The context of the error.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="position">The position in the input string.</param>
        /// <param name="length">The length of the error.</param>
        /// <param name="expected">The expected input.</param>
        /// <param name="innerErrors">The errors that caused this error to occur.</param>
        /// <param name="message">The custom message.</param>
        public UnexpectedTokenError(
            IReadOnlyParseContext context,
            IParserImplementation parser,
            int position,
            int length,
            IEnumerable<string> expected,
            IEnumerable<UnexpectedTokenError> innerErrors,
            string? message)
            : this(context, parser, position, length, expected, innerErrors, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedTokenError"/> class.
        /// </summary>
        /// <param name="context">The context of the error.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="position">The position in the input string.</param>
        /// <param name="length">The length of the error.</param>
        /// <param name="expected">The expected input.</param>
        /// <param name="innerErrors">The errors that caused this error to occur.</param>
        /// <param name="message">The custom message.</param>
        /// <param name="innerException">The inner exception that caused this exception.</param>
        public UnexpectedTokenError(
            IReadOnlyParseContext context,
            IParserImplementation parser,
            int position,
            int length,
            IEnumerable<string> expected,
            IEnumerable<UnexpectedTokenError> innerErrors,
            string? message,
            Exception? innerException)
            : base(context, parser, position, length, message, innerException)
        {
            Expected = expected.ToImmutableSortedSet();
            InnerErrors = innerErrors.ToImmutableHashSet();
        }

        /// <summary>
        /// Gets the expected string.
        /// </summary>
        public IImmutableSet<string> Expected { get; }

        /// <summary>
        /// Gets the inner errors.
        /// </summary>
        public IImmutableSet<UnexpectedTokenError> InnerErrors { get; }

        /// <summary>
        /// Gets the found string.
        /// </summary>
        public string Found => Find(Context, Position);

        private static string Find(IReadOnlyParseContext context, int position)
        {
            if (position < 0)
            {
                return "BOF";
            }

            var input = context.Input.Content;

            if (position >= input.Length)
            {
                return "EOF";
            }

            return $"'{input[position]}'";
        }

        private static string GetMessage(IReadOnlyParseContext context, int position, IReadOnlyList<string> expected)
        {
            var sb = new StringBuilder()
                .Append("Expected ");

            if (expected.Count > 1)
            {
                sb.Append("one of ");
            }

            sb.Append(string.Join(", ", expected));

            var found = Find(context, position);
            sb.Append(" but found ")
                .Append(found)
                .Append(" at ")
                .Append(context.Input.GetPosition(position).ToString());

            if (context.Input.Source is not FromMemorySource)
            {
                sb.Append(" in ")
                    .Append(context.Input.Source.ToString());
            }

            sb.Append('.');
            return sb.ToString();
        }

        /// <inheritdoc />
        public override IParseError Retarget(IParserImplementation parser)
            => new UnexpectedTokenError(Context, parser, Position, Length, Expected, InnerErrors, Message, InnerException);
    }
}
