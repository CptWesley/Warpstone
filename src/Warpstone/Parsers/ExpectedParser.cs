using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers;

/// <summary>
/// Wraps a parser and changes the expected output.
/// </summary>
/// <typeparam name="T">The result type of the wrapped parser.</typeparam>
public class ExpectedParser<T> : Parser<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpectedParser{T}"/> class.
    /// </summary>
    /// <param name="parser">The wrapped parser.</param>
    /// <param name="expected">The overriden expectation string.</param>
    public ExpectedParser(IParser<T> parser, string expected)
        : base(parser)
    {
        Parser = parser;
        Expected = expected;
    }

    /// <summary>
    /// Gets the wrapped parser.
    /// </summary>
    public IParser<T> Parser { get; }

    /// <summary>
    /// Gets the expected input.
    /// </summary>
    public string Expected { get; }

    /// <inheritdoc/>
    public override IParseResult<T> Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken)
    {
        IParseResult<T> result = recurse.Apply(Parser, state, position, maxLength, cancellationToken);

        if (result.Success)
        {
            return new ParseResult<T>(this, result.Value, result.Position, result.Next);
        }

        if (result.Error is UnexpectedTokenError ute)
        {
            return new ParseResult<T>(this, new UnexpectedTokenError(result.Error!.Position, new[] { Expected }, GetFound(state.Unit.Input, position, 1)), position);
        }

        return new ParseResult<T>(this, result.Error, result.Next);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is ExpectedParser<T> other && other.Expected == Expected;

    /// <inheritdoc/>
    public override int GetHashCode()
        => (820175, Expected).GetHashCode();

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => Expected;
}
