using Warpstone;

namespace ClausewitzLsp.Core.Parsing;

/// <summary>
/// Provides logic for tolerant parsing.
/// </summary>
/// <typeparam name="TIn">The input type of the object being parsed.</typeparam>
/// <typeparam name="TOut">The output type of the object being parsed.</typeparam>
internal class TryParser<TIn, TOut> : Parser<TOut>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TryParser{TIn, TOut}"/> class.
    /// </summary>
    /// <param name="parser">The wrapped inner parser.</param>
    /// <param name="recoveryParser">The parser used to reach a recovery point in case of failure.</param>
    /// <param name="resultTransformation">The result transformation function.</param>
    public TryParser(IParser<TIn> parser, IParser<string> recoveryParser, Func<IParseResult<TIn>, TOut> resultTransformation)
        => (Parser, RecoveryParser, ResultTransformation) = (parser, recoveryParser, resultTransformation);

    /// <summary>
    /// Gets the wrapped parser.
    /// </summary>
    public IParser<TIn> Parser { get; }

    /// <summary>
    /// Gets the parser used for reaching a recovery point.
    /// </summary>
    public IParser<string> RecoveryParser { get; }

    /// <summary>
    /// Gets the result transformation.
    /// </summary>
    public Func<IParseResult<TIn>, TOut> ResultTransformation { get; }

    /// <inheritdoc/>
    public override IParseResult<TOut> TryParse(string input, int position)
    {
        IParseResult<TIn> inner = Parser.TryParse(input, position);
        string name = Parser.ToString(10);
        TOut outValue = ResultTransformation(inner);

        int endPosition = inner.Position.End;
        if (!inner.Success)
        {
            IParseResult<string> recovery = RecoveryParser.TryParse(input, position);
            if (recovery.Success)
            {
                endPosition = recovery.Position.End;
            }

            return new ParseResult<TOut>(this, outValue, input, position, endPosition, new IParseResult[] { inner, recovery });
        }

        return new ParseResult<TOut>(this, outValue, input, position, endPosition, new IParseResult[] { inner });
    }

    /// <inheritdoc/>
    public override string ToString(int depth)
    {
        if (depth < 0)
        {
            return "...";
        }

        return $"Try({Parser.ToString(depth - 1)}, {ResultTransformation})";
    }
}
