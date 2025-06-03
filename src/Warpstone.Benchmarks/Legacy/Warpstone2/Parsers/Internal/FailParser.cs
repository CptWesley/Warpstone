using Legacy.Warpstone2.Errors;
using Legacy.Warpstone2.Internal;
using Legacy.Warpstone2.IterativeExecution;
using System.Collections.Immutable;

namespace Legacy.Warpstone2.Parsers.Internal;

/// <summary>
/// Parser that doesn't take any arguments and always fails.
/// </summary>
/// <typeparam name="T">The result type of the parser.</typeparam>
internal sealed class FailParser<T> : ParserBase<T>, IEquatable<FailParser<T>>
{
    /// <summary>
    /// The singleton instance of this parser.
    /// </summary>
    public static readonly FailParser<T> Instance = new();

    private FailParser()
    {
    }

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
        => Iterative.Done(this.Mismatch(context, position, new UnexpectedTokenError(context, this, position, 1, string.Empty)));

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => "Fail()";

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is FailParser<T> other && Equals(other);

    /// <inheritdoc />
    public bool Equals(FailParser<T>? other)
        => other is not null;

    /// <inheritdoc />
    public override int GetHashCode()
        => typeof(FailParser<T>).GetHashCode();
}
