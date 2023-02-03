using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Warpstone.Parsers;

/// <summary>
/// Parser that returns the result of the first succesful parser.
/// </summary>
/// <typeparam name="T">The parse result type of the parsers.</typeparam>
/// <seealso cref="Parser{T}" />
public class FirstParser<T> : Parser<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirstParser{T}"/> class.
    /// </summary>
    /// <param name="parsers">The parsers to try.</param>
    public FirstParser(IEnumerable<IParser<T>> parsers)
        => Parsers = parsers.ToList();

    /// <summary>
    /// Gets the parsers that will be applied.
    /// </summary>
    public IReadOnlyList<IParser<T>> Parsers { get; }

    /// <inheritdoc/>
    protected override IParseResult<T> InternalTryMatch(IParseUnit parseUnit, int position, int maxLength, CancellationToken cancellationToken)
    {
        List<IParseResult> innerResults = new List<IParseResult>();

        foreach (IParser<T> parser in Parsers)
        {
            IParseResult<T> result = parser.TryMatch(parseUnit, position, maxLength, cancellationToken);
            innerResults.Add(result);

            if (result.Success)
            {
                return new ParseResult<T>(this, result.Value, parseUnit.Input, position, result.Length, innerResults);
            }
        }

        return new ParseResult<T>(this, new ErrorCollection(innerResults.Select(x => x.Error)!), innerResults);
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => $"Or({string.Join(", ", Parsers.Select(x => x.ToString(depth - 1)))})";
}
