using System.Collections.Generic;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers.Internal;

/// <summary>
/// Provides a recursive application function for growing the left recursive solutions.
/// </summary>
internal class GrowthRecursion : IRecursionParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GrowthRecursion"/> class.
    /// </summary>
    /// <param name="growthPosition">The growth position.</param>
    /// <param name="limits">The parsers that have been previously applied.</param>
    internal GrowthRecursion(int growthPosition, ISet<IParser> limits)
    {
        GrowthPosition = growthPosition;
        Limits = limits;
    }

    /// <summary>
    /// Gets the position where the growth is occuring.
    /// </summary>
    public int GrowthPosition { get; }

    /// <summary>
    /// Gets the parsers that have been applied previously.
    /// </summary>
    public ISet<IParser> Limits { get; }

    /// <inheritdoc/>
    public IParseResult<T> Apply<T>(IParser<T> parser, IParseState state, int position, int maxLength, CancellationToken cancellationToken)
    {
        if (GrowthPosition == position && !Limits.Contains(parser))
        {
            return Packrat.ApplyRuleGrow(parser, this, state, position, maxLength, cancellationToken);
        }

        return Packrat.ApplyRule(parser, state, position, maxLength, cancellationToken);
    }
}
