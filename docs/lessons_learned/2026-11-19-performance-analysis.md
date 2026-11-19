# Performance analysis: why Warpstone 3 trails Parlot and Pidgin

Analysis only, no library change. Numbers are for the MiniJava `large` input (73 KB), Release, net8.0, recursive (Auto) mode.

| Variant | ms/parse | KB/parse |
|---|---:|---:|
| Warpstone 3 as is | 30 | 56 400 |
| Errors stubbed out (no `UnexpectedTokenError`, no `JoinErrors`) | 11.4 | 7 700 |
| Errors stubbed + `EnableAutomaticMemoization = false` | 7.2 | 4 000 |
| Errors stubbed, `ExecutionMode = Iterative` | 27 | 19 600 |
| Pidgin | 13.5 | 2 560 |
| Parlot (runtime) | 1.4 | 700 |

## Findings

- About 88% of allocated bytes are error objects. Every failed leaf builds an `UnexpectedTokenError`, which derives from
  `Exception` and copies its `[expected]` and `[]` arguments into new lists. `UnsafeParseResult(int, IParseError)` wraps it in
  `new[] { error }`, and every failed `Or` allocates a lazy `JoinErrors` iterator. The parse succeeds, so none of this is ever read.
- The memo table is a `Dictionary<int, Dictionary<IParserImplementation, UnsafeParseResult>>`, and every entry first stores a
  freshly allocated `InfiniteRecursionError` placeholder. For this grammar, memoization costs about 4 ms and saves nothing.
  Leaves such as `Tok(Regex(id))` get memoized too.
- `Then` and `ThenSkip` are built as `ThenAdd` + `Transform`, so every `Tok(...)` boxes a `ValueTuple` and calls a delegate.
  N-ary `ThenAdd` chains box nested tuples and then flatten them with another map.
- `OptionalWhitespaces` allocates the substring it skips. `ParseInput` builds its line index eagerly with `content.Split('\n')`.
- `String(value)` defaults to `CultureInfo.CurrentCulture` with `CompareOptions.None`, which is a linguistic compare.
  While errors dominated, switching the grammar to Ordinal looked free. After Phase 1 the same switch saves about 25%
  (15 → 11.2 ms in the harness). Re-measure "no effect" findings once the dominant cost is gone.

## Pitfalls

- The roslynaut `process_cpu` *self* attribution was unreliable. Runs blamed `CompareInfo.Compare` (71%) or `List.CopyTo` (40%,
  from the benchmark's own `Seq` constructor), and neither changed anything when fixed. Allocation/GC time gets pinned on a nearby
  managed frame. Trust stopwatch A/B experiments and `process_allocations` more than CPU self percentages.
- Harness: a scratch console project that references `Warpstone.Benchmarks.csproj` and loops
  `Warpstone3MiniJava.Parser.Parse(MiniJavaGrammar.LoadInput("large"))` for N seconds, printing ms and
  `GC.GetAllocatedBytesForCurrentThread()` per parse. It needs `using Warpstone;` for the `Parse` extension.
  Pass the built `.dll` to `process_allocations`/`process_cpu` with `launch`.
- Measure-only hacks were done with `sed` in `src/Warpstone` and reverted with `git checkout`. Auto mode blocks that checkout until
  `git status`/`git diff` has been shown in the session.

## Phase 1 (done): failures report to a tracker instead of allocating errors

- `Warpstone.Errors.FailureTracker`, exposed as `IParseContext.Failures`, keeps only the failures at the furthest position
  (struct entries holding the parser and the expected string, deduplicated). `UnsafeParseResult` lost `Errors`. A failure is
  `UnsafeParseResult.Failure(pos)`, and errors are created in `AsSafe` only when the whole parse fails.
- `Or` lost `JoinErrors` and its second continuation. `Expected` (`WithName`) uses `BeginLabel`/`EndLabel` (a stack, so iterative
  mode needs no per-call state). It only replaces failures at its start position and keeps deeper ones. `Not` uses
  `BeginSuppress`/`EndSuppress`. `Memo` stores `UnsafeParseResult.InProgress(pos)` (a static marker, so no allocation) and reports
  `InfiniteRecursionError` only on a real in-progress hit, then returns a plain failure so the marker never reaches another memo
  entry. Grow and CycleGrow placeholders report nothing.
- BenchmarkDotNet ShortRun, MiniJava large: 27.7 → 10.7 ms, 58.6 → 6.1 MB. Pidgin 12.7 ms, Parlot 1.4 ms.
  All existing tests passed unchanged. `Warpstone.Tests/Parsers/FailureReporting.cs` pins the new semantics.
- Package validation (`dotnet pack Warpstone`) raised nothing for the public API changes.
- The scratch harness (15 ms) and BenchmarkDotNet (10.7 ms) disagree in absolute terms. Use the harness only for A/B comparisons.
