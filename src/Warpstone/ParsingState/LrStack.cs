using System;

namespace Warpstone.ParsingState;

/// <summary>
/// Represents an LR instance in the left-recursive packrat algorithm.
/// </summary>
/// <typeparam name="TOut">The output type of the parser.</typeparam>
public class LrStack<TOut> : ILrStack<TOut>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LrStack{TOut}"/> class.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="seed">The parse seed.</param>
    private LrStack(IParser<TOut> parser, IParseResult<TOut> seed)
    {
        Parser = parser;
        Seed = seed;
        Finished = false;
    }

    /// <inheritdoc/>
    public IParser<TOut> Parser { get; }

    /// <inheritdoc/>
    IParser IReadOnlyLrStack.Parser => Parser;

    /// <inheritdoc/>
    public IParseResult<TOut> Seed { get; set; }

    /// <inheritdoc/>
    IParseResult ILrStack.Seed
    {
        get => Seed;
        set
        {
            if (value is not IParseResult<TOut> typedValue)
            {
                throw new InvalidOperationException("Incorrect result type.");
            }

            Seed = typedValue;
        }
    }

    /// <inheritdoc/>
    IParseResult IReadOnlyLrStack.Seed => Seed;

    /// <inheritdoc/>
    public IHead<TOut>? Head { get; set; }

    /// <inheritdoc/>
    IReadOnlyHead? IReadOnlyLrStack.Head => Head;

    /// <inheritdoc/>
    IHead? ILrStack.Head
    {
        get => Head;
        set
        {
            if (value is not IHead<TOut> typedValue)
            {
                throw new InvalidOperationException("Incorrect result type.");
            }

            Head = typedValue;
        }
    }

    /// <inheritdoc/>
    IReadOnlyHead<TOut>? IReadOnlyLrStack<TOut>.Head => Head;

    /// <inheritdoc/>
    public ILrStack? Next { get; set; }

    /// <inheritdoc/>
    IReadOnlyLrStack? IReadOnlyLrStack.Next => Next;

    /// <inheritdoc/>
    public bool Finished { get; set; }

    /// <inheritdoc/>
    bool IReadOnlyLrStack.Finished => Finished;


    /// <summary>
    /// Creates a new LR stack instance.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="input">The input string.</param>
    /// <param name="position">The position in the input.</param>
    /// <returns>The newly created <see cref="ILrStack{TOut}"/> instance.</returns>
    public static ILrStack<TOut> Create(IParser<TOut> parser, string input, int position)
        => new LrStack<TOut>(parser, new ParseResult<TOut>(parser, new UnboundedRecursionError(new SourcePosition(input, position, 1)), Array.Empty<IParseResult>()));
}
