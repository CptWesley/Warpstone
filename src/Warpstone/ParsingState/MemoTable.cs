using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Warpstone.Internal;

namespace Warpstone.ParsingState;

/// <summary>
/// Represents a memo table in the packrat algorithm.
/// </summary>
public class MemoTable : IMemoTable
{
    private readonly object lck = new object();
    private readonly Dictionary<int, Dictionary<IParser, ILrStack>> table = new Dictionary<int, Dictionary<IParser, ILrStack>>();
    private readonly IReadOnlyDictionary<int, IReadOnlyHead> readOnlyHeads;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoTable"/> class.
    /// </summary>
    public MemoTable()
    {
        Dictionary<int, IHead> heads = new Dictionary<int, IHead>();
        Heads = heads;
        readOnlyHeads = new ShallowReadOnlyDictionary<int, int, IReadOnlyHead, IHead>(heads);
    }

    /// <inheritdoc/>
    public ILrStack? LrStack { get; set; }

    /// <inheritdoc/>
    IReadOnlyLrStack? IReadOnlyMemoTable.LrStack => LrStack;

    /// <inheritdoc/>
    public IDictionary<int, IHead> Heads { get; }

    /// <inheritdoc/>
    IReadOnlyDictionary<int, IReadOnlyHead> IReadOnlyMemoTable.Heads => readOnlyHeads;

    /// <inheritdoc/>
    public bool Set(int position, ILrStack lr)
    {
        lock (lck)
        {
            if (!table.TryGetValue(position, out Dictionary<IParser, ILrStack>? innerTable))
            {
                innerTable = new Dictionary<IParser, ILrStack>();
                table.Add(position, innerTable);
            }

            if (!innerTable.TryGetValue(lr.Parser, out ILrStack? oldResult))
            {
                innerTable[lr.Parser] = lr;
                return false;
            }

            if (oldResult == lr)
            {
                return false;
            }

            innerTable[lr.Parser] = lr;
            return true;
        }
    }

    /// <inheritdoc/>
    public bool TryGet(int position, IParser parser, [NotNullWhen(true)] out ILrStack? parseResult)
    {
        if (table.TryGetValue(position, out Dictionary<IParser, ILrStack>? innerTable)
            && innerTable.TryGetValue(parser, out parseResult))
        {
            return true;
        }

        parseResult = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGet(int position, IParser parser, [NotNullWhen(true)] out IReadOnlyLrStack? parseResult)
    {
        if (TryGet(position, parser, out ILrStack? writeableResult))
        {
            parseResult = writeableResult;
            return true;
        }

        parseResult = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGet<TOut>(int position, IParser<TOut> parser, [NotNullWhen(true)] out ILrStack<TOut>? parseResult)
    {
        if (!TryGet(position, parser, out ILrStack? untypedParseResult))
        {
            parseResult = default;
            return false;
        }

        parseResult = (ILrStack<TOut>)untypedParseResult;
        return true;
    }

    /// <inheritdoc/>
    public bool TryGet<TOut>(int position, IParser<TOut> parser, [NotNullWhen(true)] out IReadOnlyLrStack<TOut>? parseResult)
    {
        if (TryGet(position, parser, out ILrStack<TOut>? writeableResult))
        {
            parseResult = writeableResult;
            return true;
        }

        parseResult = default;
        return false;
    }
}
