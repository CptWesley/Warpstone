using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers;

/// <summary>
/// Provides an interface for wrapping recursive applications of parsers.
/// </summary>
public interface IRecursionParser
{
    /// <summary>
    /// Applies a given <paramref name="parser"/> recursively.
    /// </summary>
    /// <typeparam name="T">The result type of the given <paramref name="parser"/>.</typeparam>
    /// <param name="parser">The parser to apply.</param>
    /// <param name="state">The current state of the parsing.</param>
    /// <param name="position">The position to apply the parser to.</param>
    /// <param name="maxLength">The maximum number of tokens to consume.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling this task.</param>
    /// <returns>The found result.</returns>
    IParseResult<T> Apply<T>(IParser<T> parser, IParseState state, int position, int maxLength, CancellationToken cancellationToken);
}
