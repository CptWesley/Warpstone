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
- ~~First-char prefilter on `Or`~~: done, see below.
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

## Follow-up: first-character filter on `Or`

The part above was committed as `2837858`. Then `Or` learned to skip alternatives that can't start with the next character.

| Mode (probe, MiniJava large) | before | after |
|---|---:|---:|
| Recursive (Auto) | 3.31 ms | 2.38 ms (−28%) |
| Left-recursive grammar | 4.93 ms | 3.99 ms (−19%) |
| Iterative | 8.2 ms | 4.87 ms (−41%) |

Allocations did not change.

### How it works

- `FirstChars` (public, `src/Warpstone/FirstChars.cs`) holds the ASCII characters a match can start with, plus the exact
  `AddExpected` calls the parser makes when it fails on any other character. Only parsers that never match empty input have one.
  Impls override `IParserImplementation.GetFirstChars` (the base returns null): the Char, String (ordinal; ignore-case only when the first char is not a letter) and Regex
  leaves, pass-throughs (Sequence, the four And impls, both Map impls, Memo, StackGuard, Aggregate with `minCount > 0`), and Expected
  (its own name replaces the inner reports). `Or` takes the union. Everything else counts as unknown, so it always runs.
- `RegexFirstChars` reads the first atom of simple patterns: a literal, an escaped punctuation character, or a class like `[A-Za-z_]`,
  after any leading `(?=..)`/`(?!..)` lookaheads. It gives up on a top-level `|`, `*`/`?`/`{` after the atom, `\w`/`\d` (not ASCII), negated
  classes, subtraction, `IgnoreCase` and so on.
- `OrParserImpl` lazily builds one `Plan` per ASCII character plus one for any other character and the end of input, and shares equal plans.
  A plan runs the possible alternatives and replays the skipped alternatives' reports where they would have run, in order. The
  errors are therefore the same, in the same order. This only happens on `StringParseInput`, because a reader may not have the character yet.
- Known difference: a skipped Memo-wrapped alternative reports its failure even where a memo hit would have reported nothing (the
  ponytail note in `FailureTracker.CreateErrors`).

### Pitfalls

- `AnalysisScaling.Shared_parsers_are_analyzed_once` (64 levels of `Or(p, p)`) took 4 minutes and failed. Walking shared parsers is
  exponential, and so is concatenating their reports. Fixes: cache `FirstChars` per impl in a `ConditionalWeakTable` (a null cached
  because of a cycle is safe, since that parser always runs), and dedupe reports by reference. A repeated `(parser, expected)` in one
  run of reports is a no-op in `FailureTracker`.
- A heredoc in the Bash tool input collapsed the doubled backslash of a C# char literal for backslash into a single one (CS1010). Fix C# backslash literals with the Edit tool.
- Differential test `Parsers/FirstCharFilter.cs`: string input (filter on) against `StringReader` input (filter off), with the same results
  and the same ordered error descriptions. A mutation that drops the skipped reports made 45 tests fail.
- New files in `src/Warpstone` add new `*.verified.cs` snapshots. Rename the per-TFM received file just as you would for edits.

## Follow-up: interface folded, README tables rerun

- `IFirstCharsSource` was folded into the public `IParserImplementation` as `GetFirstChars(ISet<IParserImplementation>)`, so `FirstChars`
  became public in the `Warpstone` namespace. `Chars` is an `IReadOnlyList<bool>`; the helpers the impls use (`Leaf(bool[]?)`,
  `AddDistinct`, `CreateSeen`, `Count`) stay internal. `ParserImplementationBase` returns null by default, and so do continuations.
- `sed -i` over `Internal/ParserImplementations/*.cs` rewrote CRLF to LF in files it did not change. Restore those with
  `git checkout --` after checking `git diff --ignore-all-space` is empty.
- `dotnet pack Warpstone -c Release` regenerates `CompatibilitySuppressions.xml` (revert it). With
  `-p:ApiCompatGenerateSuppressionFile=false`, HEAD already fails with 14 stale CP0001/CP0002 errors (`FailureTracker`,
  `UnsafeParseResult.Errors`, `IParseContext.Failures`, ...) from the earlier failure-tracker work. The new interface member added none.
  Regenerate the suppressions on purpose before a release.
- Moving a file in `src/Warpstone` renames its snapshot: delete the old `*.verified.cs`.
- README tables rerun with the full MiniJava suite (`--filter "*MiniJavaBenchmark*" --inProcess`, default job): 27 benchmarks took 9.5 min,
  not the 25 min the older note says. BenchmarkDotNet is now 0.16.0-preview.2. V3 large went from 5.31 to 3.01 ms (1,139 to 1,027 KB),
  and the left-recursive variant from 7.98 to 5.42 ms. BenchmarkDotNet's absolute numbers are about 25% above the stopwatch probe's.
  Don't run other CPU-heavy work (tests, builds) while it runs.
