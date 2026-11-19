# 2026-11-19 — `ParserExecutionMode.Auto` with recursive-to-iterative fallback

## Problem

`ParseContext.CreateAutomaticContext` chose iterative whenever the grammar was recursive. Almost every real grammar (parentheses, nested JSON) is recursive, so Auto was nearly always iterative and the faster recursive mode was never used. Whether recursion overflows depends on how deep the *input* nests, not on the grammar.

## Change

- `StackGuardParser<T>` + `StackGuardParserImpl<T>`: recursive `Apply` calls `RuntimeHelpers.EnsureSufficientExecutionStack()` (available on `netstandard2.0`) then delegates; iterative `Apply` just delegates.
- `ParserBase.GetImplementation` wraps every key of `analysis.RecursiveOccurrenceCounts` (lazy keys resolved through `lazyTargets`) in a guard, after the memo/grow wrapping and before the lazy removal step. Every grammar cycle contains a DFS back edge, and the analysis records each back-edge target there, so every cycle gets a guard.
- Guards are skipped for `Iterative`: `GetSimplifiedOptions` now keeps `ExecutionMode = Iterative` (for recursive grammars only), so iterative gets its own cached implementation without guards. Auto and Recursive share the guarded one.
- `AutoParseContext<T>` (internal): runs a `RecursiveParseContext`, and on `InsufficientExecutionStackException` throws that context away (its memo table may hold half-grown left-recursion seeds) and starts over with an `IterativeParseContext` using `Options with { ExecutionMode = Iterative }`.
- Explicit `Recursive` mode now throws `InsufficientExecutionStackException` instead of crashing the process with a stack overflow.

## Pitfalls

- Left recursion does *not* nest per character (`GrowParser` loops), so a "deep input throws" test must use right recursion.
- `EnsureSufficientExecutionStack` keeps a safety margin (~64–128 KB). In Debug, 1 000 right-recursive levels with memo on already tripped it on the NUnit thread, so `StackDepth` caps Recursive at 100 now.
- New `src/Warpstone` files get a `.verified.cs` snapshot written automatically. Only *changed* files leave `.received.cs` files that need renaming.
- New files written with the Write tool are LF; the repo uses CRLF. Convert them with Python afterwards.
- To benchmark the old library against the new one: `git stash push -u -- Warpstone/`, run the benchmarks, then `git stash pop`. Don't edit `src/Warpstone` while it's stashed.
