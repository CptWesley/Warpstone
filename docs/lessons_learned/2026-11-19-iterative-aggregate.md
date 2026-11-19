# Phase 6: one continuation per `Aggregate` run in iterative mode

Plan: [2026-11-19-performance-analysis.md](2026-11-19-performance-analysis.md), phase 6. Branch `perf/iterative-aggregate`.

## Change

`AggregateParserImpl` (behind `Many`, `OneOrMore`, `Multiple`) used to allocate a new immutable `ContinuationElement` or
`ContinuationDelimiter` for every element and every delimiter in iterative mode. Now one mutable `Continuation` per run holds
the accumulator, length and remaining counts, plus an `afterDelimiter` flag, and pushes itself again. This is safe because a run's
continuation is on the execution stack at most once. Restarts (Grow, Auto falling back to iterative) call `Apply` again and get a
new one.

Every other impl already reused continuations: they are stored in fields or are a static `Instance`. Aggregate was the only one
that allocated per element.

## Measured (net8.0 Release, stopwatch probe, best of 5 × 2 s)

| Case, iterative | before | after |
|---|---:|---:|
| `Many(Char('x'))`, 100k chars | 2.56 ms, 9 107 KB | 2.08–2.24 ms, 2 857 KB |
| `Many(Char('x'), Char(','))`, 50k elements | 2.16 ms, 8 851 KB | 1.83 ms, 2 601 KB |
| MiniJava large | 9.8 ms, 2 841 KB | 9.6–10.0 ms, 2 406 KB |

Iterative `Many` now allocates the same as recursive. Recursive mode and the BenchmarkDotNet MiniJava job (Auto, so recursive)
are not affected. MiniJava iterative still allocates about 1.2 MB more than recursive. That extra does not come from continuations.

## Bug found

`Multiple(p, 0)` (maxCount 0) parsed one element in iterative mode, while recursive mode returned an empty list. The old code had this
bug too. `Apply(IIterativeParseContext)` now pushes the empty result when `maxCount <= 0`. Covered by `Parsers/AggregateParser.cs`,
which runs over `TestOptions.All`.

## Pitfalls

- The probe lives in the session scratchpad: a net8.0 console project with a `ProjectReference` to `Warpstone.Benchmarks`, plus
  `ManagePackageVersionsCentrally=false` and `RestorePackagesWithLockFile=false` so it builds outside `src/`.
- `Warpstone.Sources.Tests` snapshot for `AggregateParserImpl` changes. Accept it by renaming `*.received.cs` to `*.verified.cs`,
  and run with `DiffEngine_Disabled=true`.
- `dotnet build Warpstone -c Release` rewrote `CompatibilitySuppressions.xml` even though no public API changed. Revert it with
  `git checkout` unless the change really touches the API.
