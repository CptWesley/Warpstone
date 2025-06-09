using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser that always passes.
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class CreateParserImpl<T> : ParserImplementationBase<CreateParser<T>, T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateParserImpl{T}"/> class.
    /// </summary>
    /// <param name="value">The value that is always returned.</param>
    public CreateParserImpl(T value)
    {
        Value = value;
    }

    /// <summary>
    /// The value that is always returned.
    /// </summary>
    public T Value { get; }

    /// <inheritdoc />
    protected override void InitializeInternal(CreateParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        // Do nothing.
    }

    /// <inheritdoc />
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        return new(position, 0, Value);
    }

    /// <inheritdoc />
    public override void Apply(IIterativeParseContext context, int position)
    {
        context.ResultStack.Push(new(position, 0, Value));
    }
}
