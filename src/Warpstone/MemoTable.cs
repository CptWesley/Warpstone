using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Warpstone
{
    /// <summary>
    /// Represents a packrat memotable.
    /// </summary>
    /// <remarks>
    /// Every memoized parser gets a slot number, and every position a row with one entry per slot.
    /// Rows are found through chunks of a fixed number of positions. No array grows with the input,
    /// so none of them ends up on the large object heap, which is only cleaned up by gen 2 collections.
    /// </remarks>
    public sealed class MemoTable : IMemoTable
    {
        // 1024 positions per chunk: 8 KB of row references, far below the 85 KB at which arrays go to the large object heap.
        private const int ChunkBits = 10;
        private const int ChunkSize = 1 << ChunkBits;
        private const int ChunkMask = ChunkSize - 1;

        private readonly Dictionary<IParserImplementation, int> slots = new();
        private readonly List<IParserImplementation> parsers = new();
        private Entry[]?[]?[] chunks = new Entry[]?[]?[4];
        private int count;

        /// <inheritdoc />
        public UnsafeParseResult this[(int, IParserImplementation) key] => this[key.Item1, key.Item2];

        /// <inheritdoc />
        public UnsafeParseResult this[int position, IParserImplementation parser]
        {
            get => TryGetValue(position, parser, out var result)
                ? result
                : throw new KeyNotFoundException("Could not find entry in memo table.");
            set => Set(position, parser, value);
        }

        /// <inheritdoc />
        UnsafeParseResult IReadOnlyMemoTable.this[int position, IParserImplementation parser] => this[position, parser];

        /// <inheritdoc />
        public IEnumerable<(int, IParserImplementation)> Keys => this.Select(static x => x.Key);

        /// <inheritdoc />
        public IEnumerable<UnsafeParseResult> Values => this.Select(static x => x.Value);

        /// <inheritdoc />
        public int Count => count;

        /// <inheritdoc />
        public bool ContainsKey((int Position, IParserImplementation Parser) key)
            => TryGetValue(key.Position, key.Parser, out _);

        /// <inheritdoc />
        public bool TryGetValue((int Position, IParserImplementation Parser) key, [NotNullWhen(true)] out UnsafeParseResult value)
            => TryGetValue(key.Position, key.Parser, out value);

        /// <inheritdoc />
        public bool TryGetValue(int position, IParserImplementation parser, [NotNullWhen(true)] out UnsafeParseResult value)
        {
            var chunkIndex = position >> ChunkBits;
            if (position >= 0
                && chunkIndex < chunks.Length
                && chunks[chunkIndex] is { } chunk
                && chunk[position & ChunkMask] is { } row
                && slots.TryGetValue(parser, out var slot)
                && slot < row.Length
                && row[slot].IsSet)
            {
                value = row[slot].Result;
                return true;
            }

            value = default;
            return false;
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<(int Position, IParserImplementation Parser), UnsafeParseResult>> GetEnumerator()
        {
            for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
            {
                if (chunks[chunkIndex] is not { } chunk)
                {
                    continue;
                }

                for (var offset = 0; offset < ChunkSize; offset++)
                {
                    if (chunk[offset] is not { } row)
                    {
                        continue;
                    }

                    for (var slot = 0; slot < row.Length; slot++)
                    {
                        if (row[slot].IsSet)
                        {
                            yield return new(((chunkIndex << ChunkBits) | offset, parsers[slot]), row[slot].Result);
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private void Set(int position, IParserImplementation parser, in UnsafeParseResult value)
        {
            if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position), position, "Memo table positions can not be negative.");
            }

            if (!slots.TryGetValue(parser, out var slot))
            {
                slot = parsers.Count;
                slots.Add(parser, slot);
                parsers.Add(parser);
            }

            var chunkIndex = position >> ChunkBits;
            if (chunkIndex >= chunks.Length)
            {
                Array.Resize(ref chunks, Math.Max(chunkIndex + 1, chunks.Length * 2));
            }

            var chunk = chunks[chunkIndex] ??= new Entry[]?[ChunkSize];
            ref var row = ref chunk[position & ChunkMask];
            if (row is null)
            {
                row = new Entry[parsers.Count];
            }
            else if (slot >= row.Length)
            {
                Array.Resize(ref row, parsers.Count);
            }

            if (!row[slot].IsSet)
            {
                count++;
            }

            row[slot] = new(value);
        }

        private readonly struct Entry
        {
            public Entry(in UnsafeParseResult result)
            {
                Result = result;
                IsSet = true;
            }

            public UnsafeParseResult Result { get; }

            public bool IsSet { get; }
        }
    }
}
