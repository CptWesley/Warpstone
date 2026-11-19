# 2026-11-19 — Review of `EnableAutomaticMemoization`

## Question

Does automatic memoization insert memo points where the parser can backtrack to?

## Findings

- The heuristic in `ParserBase.GetImplementation` memoizes every node with `OccurrenceCount > 1`, plus lazy targets whose lazy parser occurs more than once. That means "reachable through two or more paths in the grammar graph", not "can be re-invoked at the same position after a backtrack".
  - Shared tokens such as a whitespace parser get memoized at every position they run.
  - Every recursive rule gets memoized too, because the analysis counts the recursive re-entry as an occurrence.
  - Re-invocation through distinct-but-equal instances is never detected.
- The mechanics are sound. Memoization is keyed by (position, inner impl), failures are stored, an `InfiniteRecursionError` placeholder is used, left-recursion members are excluded the way pegen does it, explicit memos inside cycles are bypassed, and Auto drops the memo table on the iterative restart.
- Minor issue: when an explicit `Memo(x)` wraps an `x` that is shared elsewhere, `x` gets memoized twice.

## Measurement (net9.0, Release, scratch console app)

| grammar | memo off | memo on | memo entries |
|---|---|---|---|
| JSON-like, 20k objects (~600 KB) | 150–210 ms | 840–900 ms | 940 003 |
| `Or(term '+' expr, term '-' expr, term)`, 14 nested parens | 4.4–7.1 s | 0 ms | 30 |

The memo default (`true`) slows down grammars without shared-prefix backtracking by 4–6×.

## Fix applied

- `LeftRecursionAnalysis.FindRetried` returns the parsers reachable without consuming input through two different left-edge children of one parser, which means two `Or` alternatives, a nullable `And` first part and its second part, or a lookahead and what follows it. It stops at the first meeting point, because memoizing that node covers everything below it.
- `ParserBase.GetImplementation` memoizes only those parsers. Lazy parsers are resolved to their targets, and left-recursion members, explicit memos and grow parsers are still excluded. The rules based on `OccurrenceCount > 1` are gone. `GetSimplifiedOptions` still uses the occurrence count as a cheap precondition, since a meeting point always has two or more parents.
- Probe afterwards: the JSON-like grammar has 0 memo entries and runs as fast as with memo off. The expr grammar still takes 0 ms with 15 entries, because only `term` is memoized now.
- Known gap: `Or(Seq(a1, B), Seq(a2, B))` where `a1` and `a2` are different instances matching the same text. `B` is retried after consumption, so it isn't detected. Use explicit `Memo` there.
- Tests are in `Warpstone.Tests/Expressions/AutomaticMemoization.cs`.

## Pitfalls

- `dotnet test -f net9.0` fails with NETSDK1005, because `Warpstone.Tests` doesn't target net9.0 and runs on net8.0. Run `dotnet test` without `-f`.
- The Verify failure message lists unchanged snapshot files too. Check which `*.received.cs` files actually differ with `cmp` before you accept them.

## How to probe quickly

- A scratch console project with `<ProjectReference Include=".../src/Warpstone/Warpstone.csproj" SetTargetFramework="TargetFramework=net9.0" />` builds and runs fine from the scratchpad. It needs no lock-file or central-package setup, and analyzer warnings are just noise.
- `ParseContext.Create(ParseInput.CreateFromMemory(s), parser, options).RunToEnd(default)` followed by `ctx.MemoTable.Count` shows how many memo entries were created.
