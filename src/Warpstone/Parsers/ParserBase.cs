namespace Warpstone.Parsers;

/// <summary>
/// Provides a base implementation for the
/// <see cref="IParser{T}"/> interface.
/// </summary>
/// <typeparam name="T">The type of values found by the parser.</typeparam>
public abstract class ParserBase<T> : IParser<T>
{
    /// <inheritdoc />
    public Type ResultType => typeof(T);

    /// <inheritdoc />
    public abstract IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval);

    /// <inheritdoc cref="ToString(int)"/>
    protected abstract string InternalToString(int depth);

    /// <inheritdoc />
    [MethodImpl(InlinedOptimized)]
    public IParseResult<T> Fail(IReadOnlyParseContext context, int position)
        => ParseResult.CreateFail(context, this, position);

    /// <inheritdoc />
    [MethodImpl(InlinedOptimized)]
    IParseResult IParser.Fail(IReadOnlyParseContext context, int position)
        => Fail(context, position);

    /// <inheritdoc />
    [MethodImpl(InlinedOptimized)]
    public IParseResult<T> Mismatch(IReadOnlyParseContext context, int position, IEnumerable<IParseError> errors)
        => ParseResult.CreateMismatch(context, this, position, errors);

    /// <inheritdoc />
    [MethodImpl(InlinedOptimized)]
    public IParseResult<T> Match(IReadOnlyParseContext context, int position, int length, T value)
        => ParseResult.CreateMatch(context, this, position, length, value);

    /// <inheritdoc />
    [MethodImpl(InlinedOptimized)]
    IParseResult IParser.Mismatch(IReadOnlyParseContext context, int position, IEnumerable<IParseError> errors)
        => Mismatch(context, position, errors);

    /// <inheritdoc />
    [MethodImpl(InlinedOptimized)]
    IParseResult IParser.Match(IReadOnlyParseContext context, int position, int length, object value)
    {
        if (value is not T v)
        {
            throw new ArgumentException($"Argument is not of type '{typeof(T).FullName}'.", nameof(value));
        }

        return Match(context, position, length, v);
    }

    /// <inheritdoc />
    public string ToString(int depth)
    {
        if (depth < 0)
        {
            return "...";
        }

        return InternalToString(depth);
    }

    /// <inheritdoc />
    [MethodImpl(InlinedOptimized)]
    public sealed override string ToString()
        => ToString(10);
}
