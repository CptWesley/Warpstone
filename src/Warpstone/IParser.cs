using System;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone;

/// <summary>
/// Parser interface for parsing textual input.
/// </summary>
/// <typeparam name="TOutput">The type of the output.</typeparam>
public interface IParser<out TOutput> : IParser
{
    /// <summary>
    /// Attempts to match the current parser at the given <paramref name="position"/>.
    /// </summary>
    /// <param name="state">The current parse state.</param>
    /// <param name="position">The position to match.</param>
    /// <param name="maxLength">The maximum length of the match.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the parsing task.</param>
    /// <returns>The found parse result.</returns>
    new IParseResult<TOutput> TryMatch(IParseState state, int position, int maxLength, CancellationToken cancellationToken);
}

/// <summary>
/// Parser interface for parsing textual input.
/// </summary>
public interface IParser : IBoundedToString
{
    /// <summary>
    /// Gets the output type of the parser.
    /// </summary>
    Type OutputType { get; }

    /// <summary>
    /// Attempts to match the current parser at the given <paramref name="position"/>.
    /// </summary>
    /// <param name="state">The current parse state.</param>
    /// <param name="position">The position to match.</param>
    /// <param name="maxLength">The maximum length of the match.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the parsing task.</param>
    /// <returns>The found parse result.</returns>
    IParseResult TryMatch(IParseState state, int position, int maxLength, CancellationToken cancellationToken);
}
