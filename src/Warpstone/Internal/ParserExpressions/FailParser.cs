namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that always fails.
/// </summary>
/// <typeparam name="T">The result type of the parser.</typeparam>
internal sealed class FailParser<T> : IParser<T>
{
    /// <summary>
    /// The singleton instance of the parser.
    /// </summary>
    public static readonly FailParser<T> Instance = new();

    private FailParser()
    {
    }

    /// <inheritdoc />
    public Type ResultType => typeof(T);
}
