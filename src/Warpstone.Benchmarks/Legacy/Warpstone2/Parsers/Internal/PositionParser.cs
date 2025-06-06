#nullable enable

using Legacy.Warpstone2.Errors;
using Legacy.Warpstone2.Internal;
using Legacy.Warpstone2.IterativeExecution;
using System.Collections.Immutable;

namespace Legacy.Warpstone2.Parsers.Internal;

/// <summary>
/// Parser that doesn't take any arguments and always succeeds
/// and return the current position in the input.
/// </summary>
internal sealed class PositionParser : ParserBase<ParseInputPosition>, IEquatable<PositionParser>
{
    /// <summary>
    /// The singleton instance of this parser.
    /// </summary>
    public static readonly PositionParser Instance = new();

    private PositionParser()
    {
    }

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
        => Iterative.Done(this.Match(context, position, 0, context.Input.GetPosition(position)));

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => "Position()";

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is PositionParser other && Equals(other);

    /// <inheritdoc />
    public bool Equals(PositionParser? other)
        => other is not null;

    /// <inheritdoc />
    public override int GetHashCode()
        => typeof(PositionParser).GetHashCode();
}
