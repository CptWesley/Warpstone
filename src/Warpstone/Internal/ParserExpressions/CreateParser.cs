namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that always passes.
/// </summary>
/// <typeparam name="T">The type of the value that is always returned.</typeparam>
/// <param name="Value">The value that is always returned.</param>
internal sealed class CreateParser<T>(T Value) : IParser<T>
{
    /// <inheritdoc />
    public Type ResultType => typeof(T);
}
