# 2026-11-19 — README feature comparison table

Task: add feature table (Warpstone V1/V2/V3 vs Pidgin, Parlot, ParsecSharp, Farkle) to `README.md`.

## Where the facts came from

- **V1/V2**: frozen copies in `src/Warpstone.Benchmarks/Legacy/Warpstone1|2`. V1 has `ExpressionParsers` (operator precedence), no memo, recursive only. V2 has `MemoTable`, `IterativeExecutor`, `Step()`/`RunToEnd(CancellationToken)`, `SelectParser` (bind), and `InfiniteRecursionError` (left recursion detected, not supported).
- **Warpstone.Sources** first appears in `d0ac32c`, which is not an ancestor of V1 (`b27fb56`) or V2 (`7d7113b`), so it is V3 only.
- **Pidgin / Parlot**: method names and summaries read from `~/.nuget/packages/<id>/<ver>/lib/<tfm>/*.xml`. Parlot 2.3 has `CheckCancellation`, `EnsureSufficientStack` (fails instead of overflowing), `Parse(TextReader)`, no regex.
- **ParsecSharp** ships no XML docs. Its `README.md` in the package folder lists features explicitly ("No left-recursion support", "No packrat parsing support", full backtracking, any token type). API names via `grep -a -o` on the DLL.
- **Deep-input row**: `docs/lessons_learned/2026-11-19-benchmark-package-update.md` lists which benchmarks hit stack overflow at N=100000 (Warpstone1, Pidgin, non-generated Parlot); ParsecSharp did not.
- **Farkle** is not in the NuGet cache. Its column is from general knowledge (LALR(1) builder, regex terminals, operator scopes, precompiler) and is the least verified one.
