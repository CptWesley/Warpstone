using System;
using System.Threading;

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
    /// <param name="input">The input string.</param>
    /// <param name="position">The position to match.</param>
    /// <param name="maxLength">The maximum length of the match.</param>
    /// <param name="parseUnit">The parse unit.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the parsing task.</param>
    /// <returns>The found parse result.</returns>
    new IParseResult<TOutput> TryMatch(string input, int position, int maxLength, IParseUnit parseUnit, CancellationToken cancellationToken);
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
    /// <param name="input">The input string.</param>
    /// <param name="position">The position to match.</param>
    /// <param name="maxLength">The maximum length of the match.</param>
    /// <param name="parseUnit">The parse unit.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the parsing task.</param>
    /// <returns>The found parse result.</returns>
    IParseResult TryMatch(string input, int position, int maxLength, IParseUnit parseUnit, CancellationToken cancellationToken);
}
