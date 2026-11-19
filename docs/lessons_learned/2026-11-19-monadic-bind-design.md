# Monadic bind (`Bind`)

`Parsers.Bind(this IParser<T1>, Func<T1, IParser<T2>>)` was added on branch `monadic-binding`. The expression is `Internal/ParserExpressions/BindParser.cs`, the implementation `Internal/ParserImplementations/BindParserImpl.cs`, and the tests `Warpstone.Tests/Parsers/BindParser.cs` (all `TestOptions.All`). In the README table the V3 cell is 🟡, footnote 5.

## Why bind is hard in V3

`ParserBase<T>.GetImplementation` builds the whole implementation graph up front from static analysis: memo placement, grow wrappers and stack guards. A parser that `select` returns at parse time is invisible to all of it. A naive `next.GetImplementation(options)` call has these problems:

- **Stack overflow crash.** In `p = Char('a').Bind(_ => p)` the recursion runs through the closure, so no `StackGuardParser` is installed and the process dies.
- **Infinite left recursion.** When the source consumes nothing and the selected parser reaches the same bind again, there is no grow parser for that cycle.
- **Graph rebuild per call.** The implementation cache lives on the parser *instance*, so a lambda that allocates rebuilds the graph on every call.

## What the implementation does

- **Resolving the selected parser.** First try the outer grammar's `parserLookup`, which `Initialize` keeps; it is complete by then. A hit shares memo, grow and guards, and value-equal `String`/`Char`/`Regex`/`Create`/`End`/`Fail` also hit. Otherwise fall back to `next.GetImplementation(context.Options)`, whose cache lives on the parser, so nothing leaks.
- **Stack guard.** Recursive `Apply` always calls `RuntimeHelpers.EnsureSufficientExecutionStack()` before running the selected parser.
- **Re-entry check.** When the source consumed nothing, an `InProgress` entry under `BindParser.ReentryKey` marks the position, and finding it again reports `InfiniteRecursionError`. The key belongs to the *expression*, not the impl, because a fresh selected instance gets its own graph with a new `BindParserImpl` for the same expression. Re-entry through such a graph would be missed with an impl key (test `Detects_left_recursion_through_new_parser_instances`). The entry is reset to `Failure` afterwards, never to a real result, so it can't act as a stale memo inside a grow cycle.
- **Errors.** If `select` throws or returns null, the parse fails with a `TransformationError`.

## Pitfalls hit

- **Auto mode stack overflow.** `ParseContext.CreateAutomaticContext` only uses `AutoParseContext` (recursive, then an iterative restart) when `analysis.HasRecursiveParsers` is set. Grammars that recurse only through bind looked non-recursive, so they got a plain `RecursiveParseContext`, and the stack guard's exception escaped. Fix: `BindParser.PerformAnalysisStepInternal` sets `info.HasRecursiveParsers = true`. Side effects: growing stays enabled (the analysis runs once and finds nothing) and iterative impls are separate. Hot paths are unaffected.
- **Snapshot failure lists every file.** After the Sources snapshot run, `cmp` showed that only `Parsers` actually differed; the new Bind snapshots were created as `.verified.cs` automatically.
- **Leftover files.** DiffEngine opened vim and left `.*.swp` files in `snapshots/`; delete them. New files are written with LF while the rest of the repo uses CRLF; git normalizes this, and the diff stays small.
- **Unrelated warning.** `Parsers.cs` S8969 at the `Maybe(parser!, ...)` line was already there before this change.

## Known ceilings / upgrade paths

- Left recursion *through* a selected parser fails and is not grown. Left recursion through `Source` grows normally (test `Grows_left_recursive_source`).
- A lambda that allocates a parser wrapping a large part of the grammar rebuilds that part on every call. Upgrade path: a boundary-aware build that seeds the outer impls and stops analysis at known nodes (this needs a stop hook in `ParserBase.PerformAnalysisStep`).

## V2-style overloads

V2 had `Then`, `ThenSkip` and `ThenAdd` overloads that take a `Func<T1, IParser<T2>>` (`Legacy/Warpstone2/Parsers/BasicParsers.cs:538`). V3 now has them too, with `ThenAdd` covering tuple arities 2..8 to match the existing `ThenAdd(IParser)` overloads.

- **No wrapper parsers.** Don't build these as `Bind(x => second(x).Transform(...))`. That allocates a new parser on every call, so each call misses the outer lookup and rebuilds a graph. Instead, `BindParser<TIn, TSelected, TOut>` takes an optional `combine` function.
- **No combiner for `Bind` and `Then`.** They pass `null`, so the selected result is passed through without unboxing and boxing value types again.
- **Overload resolution.** With a lambda argument it picks the tuple-flattening `ThenAdd`, the same way it does for the `IParser` overloads (test `ThenAdd_flattens_tuples`).
- **Null-forgiving `!` in tests.** Sonar S8969 asks to drop `!` in `Lazy(() => Field!)`, but dropping it gives CS8603. The repo keeps the `!` (see `StackDepth.cs`).
- **Tuple equality with lists.** `Should().Be((2, list))` compares the list by reference; use `BeEquivalentTo`.
