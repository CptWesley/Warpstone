using System;
using System.Collections.Generic;
using System.Threading;
using Warpstone.Parsers;
using Warpstone.ParseState;

namespace Warpstone;

/// <summary>
/// Parser class for parsing textual input.
/// </summary>
/// <typeparam name="TOutput">The type of the output.</typeparam>
public abstract class Parser<TOutput> : IParser<TOutput>
{
    /// <summary>
    /// An empty set of results.
    /// </summary>
    protected static readonly IEnumerable<IParseResult> EmptyResults = Array.Empty<IParseResult>();

    /// <inheritdoc/>
    public Type OutputType => typeof(TOutput);

    /// <inheritdoc/>
    public string ToString(int depth)
    {
        if (depth < 0)
        {
            return "...";
        }

        return InternalToString(depth);
    }

    /// <inheritdoc/>
    public sealed override string ToString()
        => ToString(4);

    /// <inheritdoc/>
    public abstract IParseResult<TOutput> Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken);

    /// <inheritdoc/>
    IParseResult IParser.Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken)
        => Eval(state, position, maxLength, recurse, cancellationToken);

    /// <summary>
    /// Gets the found characters.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="position">The position.</param>
    /// <param name="length">The length to retrieve.</param>
    /// <returns>The found characters.</returns>
    protected string GetFound(string input, int position, int length)
    {
        if (length == 1)
        {
            return position < input?.Length ? $"'{input[position]}'" : "EOF";
        }

        int remainingLength = Math.Min(input.Length - position, length);
        string found = input.Substring(position, remainingLength);

        return $"\"{found}\"";
    }

    /// <summary>
    /// Provides a stringified version of the parser without depth checks.
    /// </summary>
    /// <param name="depth">The maximum depth to explore.</param>
    /// <returns>The stringified version of the parser.</returns>
    protected abstract string InternalToString(int depth);
}
