namespace Legacy.Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser that doesn't take any arguments and always succeeds.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class VoidParser<T> : Parser<T>
    {
        /// <summary>
        /// The instance of the void parser of the given return type.
        /// </summary>
        public static readonly VoidParser<T> Instance = new VoidParser<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VoidParser{T}"/> class.
        /// </summary>
        private VoidParser()
        {
        }

        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position, bool collectTraces)
            => new ParseResult<T>(this, default, input, position, position, EmptyResults);

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Void()";
        }
    }
}
