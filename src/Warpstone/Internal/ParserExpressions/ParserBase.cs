namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Provides a base implementation for parsers.
/// </summary>
/// <typeparam name="T">The result type of the parser.</typeparam>
internal abstract class ParserBase<T> : IParser<T>
{
    /// <inheritdoc />
    public Type ResultType => typeof(T);

    /// <inheritdoc />
    public IParserImplementation<T> GetImplementation(ParseOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    IParserImplementation IParser.GetImplementation(ParseOptions options)
        => GetImplementation(options);
}
