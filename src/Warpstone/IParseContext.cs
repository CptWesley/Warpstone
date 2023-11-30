namespace Warpstone;

/// <summary>
/// Provides write access to the parsing context.
/// </summary>
public interface IParseContext : IReadOnlyParseContext
{
    /// <summary>
    /// Progress the parser through a single step of the parsing process.
    /// </summary>
    /// <returns><c>true</c> if progress was made, <c>false</c> if the parsing is finished.</returns>
    /// <remarks>
    /// The amount of work done in a single step is an implementation detail and
    /// should not be relied upon for other logic.
    /// </remarks>
    public bool Step();

    /// <summary>
    /// Runs the parser to completion.
    /// </summary>
    /// <param name="cancellationToken">The token used for cancelling the process.</param>
    /// <returns>The result of parsing.</returns>
    /// <remarks>
    /// Cancelling the operation does not invalidate the <see cref="IParseContext"/>.
    /// The parsing process can be resumed after cancelling.
    /// </remarks>
    public IParseResult RunToEnd(CancellationToken cancellationToken);
}

/// <summary>
/// Provides write access to the parsing context.
/// </summary>
/// <typeparam name="T">The output type.</typeparam>
public interface IParseContext<out T> : IParseContext, IReadOnlyParseContext<T>
{
    /// <summary>
    /// Runs the parser to completion.
    /// </summary>
    /// <param name="cancellationToken">The token used for cancelling the process.</param>
    /// <returns>The result of parsing.</returns>
    /// <remarks>
    /// Cancelling the operation does not invalidate the <see cref="IParseContext"/>.
    /// The parsing process can be resumed after cancelling.
    /// </remarks>
    public new IParseResult<T> RunToEnd(CancellationToken cancellationToken);
}
