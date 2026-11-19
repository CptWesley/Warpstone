# Lessons Learned Previous Sessions

Index of past-session lessons, referenced from the **Lessons Learned Previous Sessions** section of [../../AGENTS.md](../../AGENTS.md). One line per entry — check here for a similar prior task before starting new work, then open the linked file for the full write-up.

Verify any file/type/command still exists before relying on an entry.

- [2026-11-19-legacy-test-port.md](2026-11-19-legacy-test-port.md): V1/V2 test locations in git (`b27fb56`, `7d7113b`), what got ported to V3, and a two-project scratch runner that works around the `Warpstone.Tests` restore failure.
- [2026-11-19-left-recursion-comparison.md](2026-11-19-left-recursion-comparison.md): Warpstone GrowParser vs pegen/Umeda–Maeda; test restore broke with `-f`, so probe via scratch project compiling `src/Warpstone` sources.
- [2026-11-19-xunit-to-nunit.md](2026-11-19-xunit-to-nunit.md): xunit→NUnit migration; attribute mapping, covariant `ParseOptions[][]` data-source pitfall, Verify.NUnit version pin.
- [2026-11-19-auto-execution-mode.md](2026-11-19-auto-execution-mode.md): Auto mode now runs recursively with stack guards on grammar cycles and restarts iteratively on `InsufficientExecutionStackException`; left recursion doesn't nest deep, Debug stack margin pitfalls.
- [2026-11-19-memo-default-true.md](2026-11-19-memo-default-true.md): memo default flipped to true; which default-option tests got memo on/off fixtures, plus sed CRLF and Verify vim `.swp` pitfalls.
- [2026-11-19-auto-memo-review.md](2026-11-19-auto-memo-review.md): auto memo used to memoize every shared node and is now limited to parsers retried at the same position (`FindRetried`); includes measurements, a scratch probe setup, and the `-f net9.0` pitfall.
- [2026-11-19-benchmark-package-update.md](2026-11-19-benchmark-package-update.md): Parlot 2 dropped `Compile()`; the benchmark now uses `Parlot.SourceGenerator` (`*.parlot.cs` + partial entry point), and the generated parser fails instead of overflowing at deep N.
- [2026-11-19-minijava-benchmark.md](2026-11-19-minijava-benchmark.md): MiniJava benchmark across V1/V2/V3/Pidgin/ParsecSharp/Parlot with a shared AST and a `verify-minijava` check; covers Farkle (LALR) setup, the Parlot generator error masking, ParsecSharp C# 14 extensions, Pidgin `Try`, and bogus roslynaut diagnostics.
- [2026-11-19-readme-comparison-table.md](2026-11-19-readme-comparison-table.md): sources for the README feature table (legacy copies, NuGet XML docs, ParsecSharp package README, benchmark overflow results); Farkle column least verified.
- [2026-11-19-stream-textreader-input.md](2026-11-19-stream-textreader-input.md): lazy `TextReader`/`Stream` input plus `ParseAsync` via `IParseInput` match methods returning `InputMatch` (Pending re-pushes in iterative mode); `char[]` buffer; pack/stash pitfalls.
- [2026-11-19-parseinput-factory-review.md](2026-11-19-parseinput-factory-review.md): `ParseInput` now a static factory; `CreateFromFile(path)` leaked a file handle via `File.OpenRead` (back to `ReadAllText`), factory tests, DiffEngine vim pitfall.
- [2026-11-19-ponytail-comments.md](2026-11-19-ponytail-comments.md): resolved `ponytail:` comments; `Or` now a balanced tree (analysis was quadratic / overflowed), reader `Content` drops buffer, `ReadAsync(Memory)` offset bug from `e7963b0`.
