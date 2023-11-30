namespace Warpstone.Parsers;

/// <summary>
/// Interface for parser implementations.
/// </summary>
public interface IParser
{
    /// <summary>
    /// The type of the results that the parser may find.
    /// </summary>
    public Type ResultType { get; }

    /// <summary>
    /// Create a parse result for errors encountered due to the parser definition.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="position">The position in the input.</param>
    /// <returns>The newly created <see cref="IParseResult"/>.</returns>
    public IParseResult Fail(IReadOnlyParseContext context, int position);

    /// <summary>
    /// Create a parse result for errors encountered in the input.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="position">The position in the input.</param>
    /// <param name="errors">The encountered errors.</param>
    /// <returns>The newly created <see cref="IParseResult"/>.</returns>
    public IParseResult Mismatch(IReadOnlyParseContext context, int position, IEnumerable<IParseError> errors);

    /// <summary>
    /// Create a parse result for a successful match in the input.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="position">The position in the input.</param>
    /// <param name="length">The length of the match.</param>
    /// <param name="value">The value produced by the parser.</param>
    /// <returns>The newly created <see cref="IParseResult"/>.</returns>
    public IParseResult Match(IReadOnlyParseContext context, int position, int length, object value);

    /// <summary>
    /// Creates a new iterative evaluation step in the given <paramref name="context"/>
    /// at the given <paramref name="position"/>.
    /// </summary>
    /// <param name="context">The parse context in which the parser is invoked.</param>
    /// <param name="position">The position in the input for which the parser is invoked.</param>
    /// <param name="eval">The evaluation function that can be used for invoking other parsers.</param>
    /// <returns>The newly created <see cref="IterativeStep"/>.</returns>
    public IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval);

    /// <inheritdoc cref="object.ToString"/>
    public string ToString(int depth);
}

/// <summary>
/// Interface for parser implementations.
/// </summary>
/// <typeparam name="T">The type of values found by the parser.</typeparam>
public interface IParser<out T> : IParser
{
    /// <summary>
    /// Create a parse result for errors encountered due to the parser definition.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="position">The position in the input.</param>
    /// <returns>The newly created <see cref="IParseResult{T}"/>.</returns>
    public new IParseResult<T> Fail(IReadOnlyParseContext context, int position);

    /// <summary>
    /// Create a parse result for errors encountered in the input.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="position">The position in the input.</param>
    /// <param name="errors">The encountered errors.</param>
    /// <returns>The newly created <see cref="IParseResult{T}"/>.</returns>
    public new IParseResult<T> Mismatch(IReadOnlyParseContext context, int position, IEnumerable<IParseError> errors);
}
