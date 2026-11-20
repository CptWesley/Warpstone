# Failure tracker arrays, batched `Or` reports, 24-byte memo entries

Perf pass on `perf-exp` after `14dee89`. Not committed at the end of the session.

## Changes and measured effect

Stopwatch probe, MiniJava large, net11.0 Release, best of 5 × 0.4 s, **one process per mode**, base and new alternated 3×.

| Mode | before | after |
|---|---:|---:|
| Recursive (Auto) | 2.36 ms, 1 027 KB | 2.22 ms, 1 027 KB |
| Left-recursive grammar | 4.01 ms, 2 527 KB | 3.66 ms, 2 300 KB |
| Iterative | 4.74 ms, 1 175 KB | 4.53 ms, 1 174 KB |

- `FailureTracker` keeps entries in its own `Entry[]` + count and walks them with `ref readonly`, instead of a `List<Entry>` whose
  indexer copied the 24-byte struct twice per dedupe comparison. Entry fields are plain readonly fields.
- New internal `FailureTracker.AddExpected(int, (IParserImplementation, string)[])`: `Or` plans replay a skipped alternative's
  reports with one frontier check per batch. Plan report lists are arrays, not `IReadOnlyList`.
- `MemoTable.Entry` stores the result's fields flat (`object`, two `int`s, two `bool`s): 24 bytes instead of 32. Needed
  `UnsafeParseResult`'s 4-arg ctor to become `internal`.
- `MemoTable` remembers the last parser → slot lookup. Grow looks up the same `inner` 3+ times per call.

## Measured but not kept

- Stubbing `AddExpected` out entirely: recursive −24%, LR −17%, iterative −14%. That is the ceiling for failure reporting; the
  changes above recover about half of it. ~100K `AddExpected` calls per large parse, 93% at the frontier, ~6 entries scanned each.
- `RegexOptions.Compiled` on every regex: no measurable effect. Not the bottleneck.

## Ideas left (bigger changes)

- Report failures lazily: run without tracking, and on an overall failure re-run with tracking on. Removes the ~0.4 ms reporting
  cost from successful parses. Changes semantics: transform delegates run twice on failure, `IParseContext.Failures` is empty after
  a successful run, `TextReader` input must be re-readable.
- Memo chunk index is dense: 8 bytes per input position (576 KB of the LR 2 300 KB) for 4 150 memoized positions out of 73 K. Memo
  positions are spread evenly, so smaller chunks don't help (chunk bits 4/6/8/10 touch 66–100% of chunks). Int row ids (4 B) or a
  sparse index would; watch the LOH (see [2026-11-19-memo-and-sequences.md](2026-11-19-memo-and-sequences.md)).
- Iterative `Aggregate` still allocates a `Continuation` per run that matched an element (~10% of iterative bytes). Moving the state
  onto the result stack needs a place for accumulator, counts and the delimiter flag; a value-type accumulator would box.
- Identifier substrings from `Regex` are ~12% of recursive bytes. A per-regex span-keyed intern cache (`GetAlternateLookup`, net9+)
  would cut them; probably time-neutral.

## Pitfalls

- **Dynamic PGO makes in-process A/B order-dependent.** Running w3, w3lr, w3it in one process: whichever ran first was fastest
  (w3it 4.0 ms first vs 5.8 ms third). Shared generic impls (`SequenceParserImpl<__Canon,...>`) get profiled on the first
  workload. Run every mode in its own process. Scratch scripts: `snap.sh base|new` (build probe, copy output dir), `ab.sh N modes`
  (alternate base/new, one process each).
- `process_cpu` self time again blamed `BulkMoveWithWriteBarrier` (87%). Ignore it; `process_allocations` was useful (memo table
  = 63% of LR bytes).
- Python `str.replace` on files opened with `newline=''` silently missed patterns because the files are CRLF. Normalize to `\n`,
  `assert old in s`, write back as CRLF.
- `sed -i` on a CRLF file rewrites the whole file to LF (git warns). Revert hacks with `git checkout --`.
- An incremental `dotnet build` hid analyzer warnings in files that changed; `--no-incremental` showed SA1214 (field order), SA1600
  (doc on a now-internal ctor) and CS0419 (cref to a now-overloaded `AddExpected`).
- Verify received snapshots carry a TFM (`Snapshots.Generates.DotNet10_0#...received.cs`); rename to `Snapshots.Generates#...verified.cs`.
