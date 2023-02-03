using System.Text.RegularExpressions;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers;

/// <summary>
/// Represents a parser which sequentially applies a sequence of parsers and retains all results.
/// </summary>
/// <typeparam name="T1">The result type of the first parser.</typeparam>
/// <typeparam name="T2">The result type of the second parser.</typeparam>
/// <seealso cref="Parser{T}" />
public class SeqParser<T1, T2> : Parser<(T1 First, T2 Second)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqParser{T1, T2}"/> class.
    /// </summary>
    /// <param name="first">The first parser that's tried.</param>
    /// <param name="second">The second parser that's applied if the first one fails.</param>
    public SeqParser(IParser<T1> first, IParser<T2> second)
    {
        First = first;
        Second = second;
    }

    /// <summary>
    /// Gets the first parser.
    /// </summary>
    public IParser<T1> First { get; }

    /// <summary>
    /// Gets the second parser.
    /// </summary>
    public IParser<T2> Second { get; }

    /// <inheritdoc/>
    protected override IParseResult<(T1 First, T2 Second)> InternalTryMatch(IParseState state, int position, int maxLength, CancellationToken cancellationToken)
    {
        int curPosition = position;
        int curMaxLength = maxLength;

        IParseResult<T1> firstResult = First.TryMatch(state, curPosition, curMaxLength, cancellationToken);
        if (!firstResult.Success)
        {
            return new ParseResult<(T1, T2)>(this, firstResult.Error, new[] { firstResult });
        }

        curPosition += firstResult.Length;
        curMaxLength -= firstResult.Length;

        IParseResult<T2> secondResult = Second.TryMatch(state, curPosition, curMaxLength, cancellationToken);
        if (!secondResult.Success)
        {
            return new ParseResult<(T1, T2)>(this, secondResult.Error, new IParseResult[] { firstResult, secondResult });
        }

        curPosition += secondResult.Length;

        return new ParseResult<(T1, T2)>(this, (firstResult.Value, secondResult.Value), state.Unit.Input, position, curPosition - position, new IParseResult[] { firstResult, secondResult });
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => $"Seq({First.ToString(depth - 1)}, {Second.ToString(depth - 1)})";
}
