using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Warpstone.Parsers;

/// <summary>
/// Provides a base for sequence parsers.
/// </summary>
/// <remarks>Might need to be replaced for performance reasons.</remarks>
/// <typeparam name="T">The result type of the sequence parser.</typeparam>
public abstract class BaseSeqParser<T> : Parser<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSeqParser{T}"/> class.
    /// </summary>
    /// <param name="parsers">The set of internal parsers.</param>
    public BaseSeqParser(IEnumerable<IParser> parsers)
    {
        Parsers = parsers.ToList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSeqParser{T}"/> class.
    /// </summary>
    /// <param name="parsers">The set of internal parsers.</param>
    public BaseSeqParser(params IParser[] parsers)
        : this(parsers as IEnumerable<IParser>)
    {
    }

    /// <summary>
    /// Gets the set of sequential parsers.
    /// </summary>
    public IReadOnlyList<IParser> Parsers { get; }

    /// <summary>
    /// Creates a final result from the internally found values.
    /// </summary>
    /// <param name="values">The internally found values.</param>
    /// <returns>The newly created value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract T CreateValue(object?[] values);

    /// <inheritdoc/>
    protected sealed override IParseResult<T> InternalTryMatch(string input, int position, int maxLength, IMemoTable memoTable, CancellationToken cancellationToken)
    {
        int curPos = position;
        int curMaxLength = maxLength;
        IParseResult[] innerResults = new IParseResult[Parsers.Count];
        object?[] values = new object?[Parsers.Count];

        for (int i = 0; i < Parsers.Count; i++)
        {
            IParser parser = Parsers[i];
            IParseResult result = parser.TryMatch(input, curPos, curMaxLength, memoTable, cancellationToken);
            innerResults[i] = result;

            if (!result.Success)
            {
                return new ParseResult<T>(this, result.Error, innerResults);
            }

            curPos += result.Length;
            curMaxLength -= result.Length;
            values[i] = result.Value;
        }

        return new ParseResult<T>(this, CreateValue(values), input, position, curPos - position, innerResults);
    }

    /// <inheritdoc/>
    protected sealed override string InternalToString(int depth)
        => $"Seq({string.Join(", ", Parsers.Select(x => x.ToString(depth - 1)))})";
}