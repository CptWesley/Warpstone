using DotNetProjectFile.Resx;

namespace Warpstone.Parsers;

/// <summary>
/// A parser which only parses the end of a file.
/// </summary>
public sealed class EndOfFileParser : ParserBase<string>, IEquatable<EndOfFileParser>
{
    /// <summary>
    /// The singleton instance of this parser.
    /// </summary>
    public static readonly EndOfFileParser Instance = new EndOfFileParser();

    private EndOfFileParser()
    {
    }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
    {
        if (position >= context.Input.Content.Length)
        {
            return Iterative.Done(this.Match(context, position, 0, string.Empty));
        }
        else
        {
            return Iterative.Done(this.Mismatch(context, position, new UnexpectedTokenError(context, this, position, 1, "EOF")));
        }
    }

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => "EOF";

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is EndOfFileParser other && Equals(other);

    /// <inheritdoc />
    public bool Equals(EndOfFileParser other)
        => other is not null;

    /// <inheritdoc />
    public override int GetHashCode()
        => typeof(CharacterParser).GetHashCode();
}
