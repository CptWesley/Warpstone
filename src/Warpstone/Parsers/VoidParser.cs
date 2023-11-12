namespace Warpstone.Parsers;

/// <summary>
/// Parser that doesn't take any arguments and always succeeds.
/// </summary>
/// <typeparam name="T">The result type of the parser.</typeparam>
public sealed class VoidParser<T> : ParserBase<T?>, IEquatable<VoidParser<T>>
{
    /// <summary>
    /// The singleton instance of this parser.
    /// </summary>
    public static readonly VoidParser<T> Instance = new();

    private VoidParser()
    {
    }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.Done(this.Match(context, position, 0, default));

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => "Void()";

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is VoidParser<T> other && Equals(other);

    /// <inheritdoc />
    public bool Equals(VoidParser<T> other)
        => other is not null;

    /// <inheritdoc />
    public override int GetHashCode()
        => typeof(VoidParser<T>).GetHashCode();
}
