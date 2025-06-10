namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Provides some basic implementations for <see cref="IParserImplementation"/> implementations.
/// </summary>
/// <typeparam name="TParser">The type of the corresponding parser expression.</typeparam>
/// <typeparam name="TResult">The result type being parsed.</typeparam>
internal abstract class ParserImplementationBase<TParser, TResult> : IParserImplementation<TParser, TResult>
    where TParser : IParser<TResult>
{
    /// <inheritdoc />
    public TParser ParserExpression { get; private set; } = default!;

    /// <inheritdoc />
    IParser<TResult> IParserImplementation<TResult>.ParserExpression => ParserExpression;

    /// <inheritdoc />
    IParser IParserImplementation.ParserExpression => ParserExpression;

    /// <inheritdoc />
    public abstract UnsafeParseResult Apply(IRecursiveParseContext context, int position);

    /// <inheritdoc />
    public abstract void Apply(IIterativeParseContext context, int position);

    /// <inheritdoc cref="Initialize(TParser, IReadOnlyDictionary{IParser, IParserImplementation})" />
    protected abstract void InitializeInternal(TParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup);

    /// <inheritdoc />
    public void Initialize(TParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        ParserExpression = parser;
        InitializeInternal(parser, parserLookup);
    }

    /// <inheritdoc />
    public void Initialize(IParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        if (parser is not TParser typedParser)
        {
            throw new ArgumentException("Parser is of wrong type.", nameof(parser));
        }

        Initialize(typedParser, parserLookup);
    }
}
