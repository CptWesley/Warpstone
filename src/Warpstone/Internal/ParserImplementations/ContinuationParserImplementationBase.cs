namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Provides a base implementation for continuations.
/// </summary>
internal abstract class ContinuationParserImplementationBase : IParserImplementation
{
    /// <inheritdoc />
    public IParser ParserExpression => throw new NotImplementedException();

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public abstract void Apply(IIterativeParseContext context, int position);

    /// <inheritdoc />
    public void Initialize(IParser parser)
    {
        throw new NotImplementedException();
    }
}
