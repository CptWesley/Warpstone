using System;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers.Internal;

/// <summary>
/// Provides a recursive application function for simply applying the rule without growing.
/// </summary>
internal class RegularRecursion : IRecursionParser
{
    /// <summary>
    /// Gets the singleton instance of this recursion parser.
    /// </summary>
    public static readonly IRecursionParser Instance = new RegularRecursion();

    private RegularRecursion()
    {
    }

    /// <inheritdoc/>
    public IParseResult<T> Apply<T>(IParser<T> parser, IParseState state, int position, int maxLength, CancellationToken cancellationToken)
    {
        //Console.WriteLine($"At {position} applying {parser}");
        return Packrat.ApplyRule(parser, state, position, maxLength, cancellationToken);
    }
}
