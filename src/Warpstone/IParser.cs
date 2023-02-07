using System;
using System.Collections.Generic;
using System.Threading;
using Warpstone.Parsers;
using Warpstone.ParseState;

namespace Warpstone;

/// <summary>
/// Parser interface for parsing textual input.
/// </summary>
/// <typeparam name="TOutput">The type of the output.</typeparam>
public interface IParser<out TOutput> : IParser
{
    /// <summary>
    /// Attempts to match the current parser at the given <paramref name="position"/> without any checks.
    /// </summary>
    /// <param name="state">The current parse state.</param>
    /// <param name="position">The position to match.</param>
    /// <param name="maxLength">The maximum length that the match can be.</param>
    /// <param name="recurse">The recursion function which should be used for obtaining result of inner parsers.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the parsing task.</param>
    /// <returns>The found parse result.</returns>
    public new IParseResult<TOutput> Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken);
}

/// <summary>
/// Parser interface for parsing textual input.
/// </summary>
public interface IParser : IBoundedToString
{
    /// <summary>
    /// Gets the output type of the parser.
    /// </summary>
    public Type OutputType { get; }

    /// <summary>
    /// Gets a list of all parsers that can be invoked by this parser.
    /// </summary>
    public IReadOnlyList<IParser> SubParsers { get; }

    /// <summary>
    /// Gets a value indicating whether or not this parser is terminal.
    /// </summary>
    public bool IsTerminal { get; }

    /// <summary>
    /// Attempts to match the current parser at the given <paramref name="position"/> without any checks.
    /// </summary>
    /// <param name="state">The current parse state.</param>
    /// <param name="position">The position to match.</param>
    /// <param name="maxLength">The maximum length that the match can be.</param>
    /// <param name="recurse">The recursion function which should be used for obtaining result of inner parsers.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the parsing task.</param>
    /// <returns>The found parse result.</returns>
    public IParseResult Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a graph representation of this <see cref="IParser"/> instance in the DOT language.
    /// The DOT language is used by the Graphviz graph rendering engine.
    /// More information can be found here: <see href="https://graphviz.org/"/>.
    /// </summary>
    /// <returns>A graph representation of this instance in the DOT language.</returns>
    public string ToDotGraph();
}
