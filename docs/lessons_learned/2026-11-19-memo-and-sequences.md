# Phases 2 and 3: cheaper memoization and fused sequences

Branch `perf/memo-and-sequences`, branched from `main` after #46. The plan is in
[2026-11-19-performance-analysis.md](2026-11-19-performance-analysis.md).

## Changes and measured effect

The harness numbers are for the MiniJava large input. Every step was timed on its own.

| Step | Harness ms | KB/parse |
|---|---:|---:|
| Start (after #46) | ~11 | 5 894 |
| `MemoTable`: one `Dictionary<Key, UnsafeParseResult>` (struct key, identity hash) instead of a dictionary per position | 9.8 | 4 760 |
| Don't auto-memoize cheap parsers (`LeftRecursionAnalysis.IsCheap`: at most 8 parsers, only leaves plus `And`/`Sequence`/`Map`/`Expected`) | 7.2 | 4 000 |
| `SequenceParser<TFirst, TSecond, TResult>`: `Then`, `ThenSkip` and 3–8-ary `ThenAdd` build it directly, and `Transform` of an `And` or a sequence fuses into it (`ISequenceParser<T>.Map`) | 6.2 | 2 089 |
| `Multiple` (`Many`) seeds with `Array.Empty`, so a repetition that matches nothing allocates no `List` | 6.2 | ~1 600 |
| `StringParseInput`: lazy line index, shared strings for whitespace runs that are spaces after at most one line break | 6.2 | 1 207 |

BenchmarkDotNet ShortRun, large: 9.2 → 5.3 ms, 5.9 → 1.2 MB. Pidgin 13.3 ms / 2.6 MB, Parlot 1.46 ms / 0.7 MB.
Iterative mode (harness): 20.7 → 12.2 ms.

## Pitfalls

- Every parser that MiniJava auto-memoized was cheap: with the cheap rule, memo on and memo off allocate the same. One harness run
  showed 8.3 vs 7.2 ms, which was noise. Run the A/B pair alternately (`w3nomemo w3 w3nomemo w3`) before believing a difference.
- `Transform` fusion gives a transformed `And` its own `SequenceParser` node. An `And` instance shared by two `Transform`s is no longer one
  shared node, but its children still are, so memoization still sees them.
- The flat memo dictionary made the left-recursive grammar do one Gen2 collection per parse: its resize arrays grew past 85 KB and landed
  on the large object heap. About 80% of the bytes were `Dictionary` resize arrays. Fix: `MemoTable` gives each parser a slot number
  (a small parser → slot dictionary). Each position gets a row with one entry per slot, created when first needed, and rows are found through
  chunks of 1024 positions (8 KB). An `Entry.IsSet` flag tells a stored failure apart from a missing entry. BenchmarkDotNet, large,
  left-recursive: 9.2 → 7.8 ms, 4.6 → 2.7 MB, Gen2 969 → 0 per 1000 parses. `MemoTables.cs` tests the chunk and slot logic.
- The scratch harness now prints `gen2/parse` (`GC.CollectionCount(2) / n`) and has a `w3lr` mode for the left-recursive grammar.
- Python heredocs that contain `'\n'` wrote real line breaks into C# char literals. Use the Edit tool for C# text with escapes.
- Set `DiffEngine_Disabled=true` when running `Warpstone.Sources.Tests`, or Verify opens vim and leaves `.swp` files behind.
- `verify-minijava | grep -v "same AST"` also counts the three header lines (`... AST dump N chars`). Count the verdict lines instead.
