namespace Warpstone.Parsers.Internal;

/// <summary>
/// A parser which only parses the end of a file.
/// </summary>
internal sealed class EndOfFileParser : ParserBase<string>, IEquatable<EndOfFileParser>
{
    /// <summary>
    /// The singleton instance of this parser.
    /// </summary>
    public static readonly EndOfFileParser Instance = new();

    private EndOfFileParser()
    {
    }

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
    {
        if (position >= context.Input.Content.Length)
        {
            return Iterative.Done(this.Match(context, position, 0, string.Empty));
        }
        else
        {
            return Iterative.Done(this.Mismatch(context, position, 1, "EOF"));
        }
    }

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => "EOF";

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is EndOfFileParser other && Equals(other);

    /// <inheritdoc />
    public bool Equals(EndOfFileParser? other)
        => other is not null;

    /// <inheritdoc />
    public override int GetHashCode()
        => typeof(CharacterParser).GetHashCode();
}
