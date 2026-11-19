# Flat n-ary `OrParser`

Follow-up to [2026-11-19-memo-and-sequences.md](2026-11-19-memo-and-sequences.md) (phase 3, fused `SequenceParser`).

## Change

`OrParser<T>` holds `IReadOnlyList<IParser<T>> Alternatives` instead of `First`/`Second`. `Parsers.Or(params)` / `Or(IEnumerable)` builds one
node (one alternative → that parser, none → `Fail`) instead of the balanced binary tree from
[2026-11-19-ponytail-comments.md](2026-11-19-ponytail-comments.md). `OrParserImpl` loops in recursive mode; in iterative mode it keeps one
`Continuation` per alternative (chained through `next`), so trying alternative i costs one continuation frame instead of log n nested Or frames.

## Measured (net8.0 Release, stopwatch probe referencing `Warpstone.Benchmarks`, best of 5)

| Case | before | after |
|---|---:|---:|
| `Many(Or(64 strings))`, 6000 tokens, iterative | 9.2 ms | 6.8 ms |
| same, recursive | 6.1 ms | 5.1 ms |
| MiniJava large, iterative | 9.8 ms | 9.5 ms |
| MiniJava large, recursive | 5.1 ms | 5.0 ms |

BenchmarkDotNet ShortRun MiniJava: no change beyond noise, same allocations. MiniJava choices are only 2–3 wide.

## Pitfalls

- Don't flatten a nested `Or(Or(a, b), c)` at construction. `AnalysisScaling.Shared_parsers_are_analyzed_once` builds 64 levels of
  `Or(p, p)`; flattening copies shared children and gives 2^64 alternatives. Only the `params`/`IEnumerable` factory is flat.
- `Warpstone.Benchmarks` targets net8.0 only (`benchmarks.props`): `dotnet run -f net9.0` fails with NETSDK1005.
- Roslynaut `diagnostics` on `Parsers.cs` reports `StringSyntaxAttribute` CS0246 errors. They come from the netstandard view and are not real.
- `git stash` for an A/B run leaves untracked files (the new test) in place; fine here, the probe doesn't compile tests.
