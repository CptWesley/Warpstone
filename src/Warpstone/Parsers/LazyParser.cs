using System;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers;

/// <summary>
/// A parser that is lazily instantiated.
/// </summary>
/// <typeparam name="T">Result type of the given parser.</typeparam>
/// <seealso cref="Parser{T}" />
public class LazyParser<T> : Parser<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyParser{T}"/> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    public LazyParser(Func<IParser<T>> parser)
        => Parser = new Lazy<IParser<T>>(parser);

    /// <summary>
    /// Gets the parser.
    /// </summary>
    public Lazy<IParser<T>> Parser { get; }

    /// <inheritdoc/>
    protected override IParseResult<T> InternalTryMatch(IParseState state, int position, int maxLength, CancellationToken cancellationToken)
    {
        IParseResult<T> result = Parser.Value.TryMatch(state, position, maxLength, cancellationToken);
        if (result.Success)
        {
            return new ParseResult<T>(this, result.Value, state.Unit.Input, result.Start, result.Length, new[] { result });
        }

        return new ParseResult<T>(this, result.Error, new[] { result });
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => $"Lazy({Parser.Value.ToString(depth - 1)})";
}