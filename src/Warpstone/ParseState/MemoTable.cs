using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Warpstone.ParseState;

/// <summary>
/// Represents a memo table in the packrat algorithm.
/// </summary>
public class MemoTable : IMemoTable
{
    private static readonly IReadOnlyDictionary<IParser, IParseResult> EmptyMatches = new Dictionary<IParser, IParseResult>();
    private readonly object lck = new object();
    private readonly Dictionary<int, Dictionary<IParser, IParseResult>> table = new Dictionary<int, Dictionary<IParser, IParseResult>>();

    /// <inheritdoc/>
    public bool Set(int position, IParser parser, IParseResult result)
    {
        if (parser.OutputType != result.OutputType)
        {
            throw new ArgumentException($"Output type of result '{result.OutputType.FullName}' does not match with output type of parser '{parser.OutputType.FullName}'.", nameof(result));
        }

        lock (lck)
        {
            if (!table.TryGetValue(position, out Dictionary<IParser, IParseResult>? innerTable))
            {
                innerTable = new Dictionary<IParser, IParseResult>();
                table.Add(position, innerTable);
            }

            if (!innerTable.TryGetValue(parser, out IParseResult? oldResult))
            {
                innerTable[parser] = result;
                return false;
            }

            if (oldResult == result)
            {
                return false;
            }

            innerTable[parser] = result;
            return true;
        }
    }

    /// <inheritdoc/>
    public bool TryGet(int position, IParser parser, [NotNullWhen(true)] out IParseResult? parseResult)
    {
        if (table.TryGetValue(position, out Dictionary<IParser, IParseResult>? innerTable)
            && innerTable.TryGetValue(parser, out parseResult))
        {
            return true;
        }

        parseResult = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGet<TOutput>(int position, IParser<TOutput> parser, [NotNullWhen(true)] out IParseResult<TOutput>? parseResult)
    {
        if (!TryGet(position, parser, out IParseResult? untypedParseResult))
        {
            parseResult = default;
            return false;
        }

        parseResult = (IParseResult<TOutput>)untypedParseResult;
        return true;
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<IParser, IParseResult> GetAtPosition(int position)
    {
        if (!table.TryGetValue(position, out Dictionary<IParser, IParseResult> innerTable))
        {
            return EmptyMatches;
        }

        return innerTable;
    }

    /// <inheritdoc/>
    public IReadOnlyList<int> GetPositions()
        => table.Keys.ToList();
}
