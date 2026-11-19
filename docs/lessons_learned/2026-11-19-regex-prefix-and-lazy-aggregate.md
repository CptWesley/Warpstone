# Regex literal prefix, lazy iterative `Aggregate`, Create box once

Follow-up perf pass after #53. Not committed at the end of the session.

## Changes and measured effect

Stopwatch probe, MiniJava large, net11.0 Release, best of 5 × 2 s, baseline and change alternated twice (`git stash` of `src/Warpstone`).

| Mode | before | after |
|---|---:|---:|
| Recursive (Auto) | 3.78 ms, 1 139 KB | 3.39 ms, 1 027 KB |
| Left-recursive grammar | 5.62 ms, 2 639 KB | 5.13 ms, 2 527 KB |
| Iterative | 8.9–9.5 ms, 2 341 KB | 8.2–8.75 ms, 1 177 KB |

- `RegexParserImpl` finds the literal letters/digits/underscores a pattern starts with (`if` in `if\b`) and rejects a position with
  `string.CompareOrdinal` before running the regex (`StringParseInput` only). Most of the time win: MiniJava tries many keyword
  alternatives (`Kw(...)`) per statement and the regex call overhead is paid even when it fails at the first char. No prefix when the
  pattern has `|` anywhere, or `IgnoreCase`/`IgnorePatternWhitespace`/`RightToLeft`; a quantifier after the run drops its last char.
- Same overload reuses the regex's first matched string when a later match has the same text (write-once field, so no cross-thread
  write contention). Keyword regexes stop allocating a substring per match.
- `CreateParserImpl` stores the value as `object` once, so `Maybe(p, false)` no longer boxes per call.
- Iterative `Aggregate` pushes a shared `FirstContinuation`; the per-run mutable `Continuation` is only allocated after the first
  element succeeded. Most `Many` runs match nothing (every binary operator level of `Chain`). Halved iterative allocations.

Tests: `Warpstone.Tests/Parsers/RegexParser.cs` pins that the prefix check never rejects a match (quantifiers, alternation, inline
`(?i)`, `IgnoreCase`) and that matches return their own text.

## Remaining ideas (not done, bigger changes)

- Iterative mode is still ~2.4× recursive. Every step goes through `IIterativeParseContext` interface properties and two BCL `Stack<T>`s.
  A concrete context type or array-backed stacks exposed to impls would cut dispatch; touches every impl.
- First-char (FIRST set) prefilter on `Or` alternatives, like Parlot. Needs FIRST-set analysis on expressions; regex leaves have none
  except the literal prefix above.
- `MemoTable` does a slot-dictionary lookup per get/set; Memo/Grow do three per call. A per-impl slot assigned at graph build would
  remove them, but impls such as `RegexParserImpl` are shared between graphs.
- Fused `Transform` on a sequence composes two delegates (`map(combine(a, b))`).

## Pitfalls

- `process_cpu` again blamed `Buffer.BulkMoveWithWriteBarrier` for ~90% (the leaf path ends in the benchmark's `Seq` constructor).
  Ignore CPU self attribution; use the probe.
- Python heredoc wrote `\b` inside a C# doc comment as a backspace char (CS1570 + QW0004). Escape as `chr(92) + 'b'` or use Edit.
- Verify wrote received snapshots per TFM (`Snapshots.Generates.DotNet8_0#...received.cs`) while the verified file has no TFM
  suffix. Diff with `--strip-trailing-cr`, then rename one received file to the `Snapshots.Generates#...verified.cs` name and delete the rest.
- `dotnet test Warpstone.Tests -f net481` fails with NETSDK1005 (assets file lacks net481); `-f net11.0` works.
