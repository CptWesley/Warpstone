using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Warpstone.ParseState;

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
        : base(parsers)
    {
    }

    /// <inheritdoc/>
    public override IParseResult<T> Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken)
    {
        List<IParseResult> innerResults = new List<IParseResult>();

        foreach (IParser<T> parser in SubParsers)
        {
            IParseResult<T> result = recurse.Apply(parser, state, position, maxLength, cancellationToken);
            innerResults.Add(result);

            if (result.Success)
            {
                return new ParseResult<T>(this, result.Value, result.Position, result.Next);
            }
        }

        return new ParseResult<T>(this, new ErrorCollection(innerResults.Select(x => x.Error)!), position);
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => $"Or({string.Join(", ", SubParsers.Select(x => x.ToString(depth - 1)))})";
}
