using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that always passes.
/// </summary>
/// <typeparam name="T">The type of the value that is always returned.</typeparam>
internal sealed class CreateParser<T> : ParserBase<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateParser{T}"/> class.
    /// </summary>
    /// <param name="value">The value that is always returned.</param>
    public CreateParser(T value)
    {
        Value = value;
    }

    /// <summary>
    /// The value that is always returned.
    /// </summary>
    public T Value { get; }

    /// <inheritdoc />
    public override IParserImplementation<T> CreateUninitializedImplementation()
        => new CreateParserImpl<T>(Value);

    /// <inheritdoc />
    protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
    {
        // Do nothing.
    }
}
