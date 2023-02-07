using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers;

public class ExpectedParser<T> : Parser<T>
{
    public ExpectedParser(IParser<T> parser, string expected)
    {
        Parser = parser;
        Expected = expected;
    }

    public IParser<T> Parser { get; }

    public string Expected { get; }

    public override IParseResult<T> Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken)
    {
        IParseResult<T> result = recurse.Apply(Parser, state, position, maxLength, cancellationToken);

        if (result.Success)
        {
            return new ParseResult<T>(this, result.Value, state.Unit.Input, result.Start, result.Length, new[] { result });
        }

        if (result.Error is UnexpectedTokenError ute)
        {
            return new ParseResult<T>(this, new UnexpectedTokenError(result.Error!.Position, new[] { Expected }, GetFound(state.Unit.Input, position)), new[] { result });
        }

        return new ParseResult<T>(this, result.Error, new[] { result });
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => Expected;
}
