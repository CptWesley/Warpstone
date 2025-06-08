namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that parses a string.
/// </summary>
/// <param name="Value">The string to be parsed.</param>
/// <param name="Culture">The culture used for comparing.</param>
/// <param name="Options">The options used for comparing.</param>
internal sealed class StringParser(string Value, CultureInfo Culture, CompareOptions Options) : IParser<string>
{
    /// <inheritdoc />
    public Type ResultType => typeof(string);
}
