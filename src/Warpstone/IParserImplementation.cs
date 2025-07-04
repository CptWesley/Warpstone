using System.Collections.Generic;

namespace Warpstone
{
    /// <summary>
    /// Interface for parser implementations.
    /// </summary>
    public interface IParserImplementation
    {
        /// <summary>
        /// The parser expression that represents this implementation.
        /// </summary>
        public IParser ParserExpression { get; }

        /// <summary>
        /// Applies the parser recursively.
        /// </summary>
        /// <param name="context">The parsing context.</param>
        /// <param name="position">The position to apply it to.</param>
        /// <returns>The found result.</returns>
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position);

        /// <summary>
        /// Applies the parser iteratively.
        /// </summary>
        /// <param name="context">The parsing context.</param>
        /// <param name="position">The position to apply it to.</param>
        public void Apply(IIterativeParseContext context, int position);

        /// <summary>
        /// Initializes the parser implementation.
        /// </summary>
        /// <param name="parser">The parser to initialize from.</param>
        /// <param name="parserLookup">The lookup table for looking up referenced parsers.</param>
        public void Initialize(IParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup);
    }

    /// <summary>
    /// Interface for all typed parser implementations.
    /// </summary>
    /// <typeparam name="T">The result type being parsed.</typeparam>
    public interface IParserImplementation<out T> : IParserImplementation
    {
        /// <inheritdoc cref="IParserImplementation.ParserExpression" />
        public new IParser<T> ParserExpression { get; }
    }

    /// <summary>
    /// Interface for all typed parser implementations.
    /// </summary>
    /// <typeparam name="TParser">The type of the corresponding parser expression.</typeparam>
    /// <typeparam name="TResult">The result type being parsed.</typeparam>
    public interface IParserImplementation<TParser, out TResult> : IParserImplementation<TResult>
        where TParser : IParser<TResult>
    {
        /// <inheritdoc cref="IParserImplementation.ParserExpression" />
        public new TParser ParserExpression { get; }

        /// <inheritdoc cref="IParserImplementation.Initialize(IParser, IReadOnlyDictionary{IParser, IParserImplementation})" />
        public void Initialize(TParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup);
    }
}
