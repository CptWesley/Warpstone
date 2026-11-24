# Source generation (`GenerateSource`)

`IParser<T>.GenerateSource(namespace, className, params usings)` writes a C# file with a parser that doesn't depend on Warpstone.
This file covers the design choices and the pitfalls hit while building it.

## Design

- **The plan is shared.** The memo, grow, cycle-grow, memo-bypass and stack-guard decisions moved out of `ParserBase.GetImplementation`
  into `ParserPlan` (an ordered list of wrap and alias steps, replayed by `Execute`). The interpreter replays it with implementations,
  the emitter with method names, so both memoize and grow the same nodes. Phase 0 was a refactor with no behaviour change, and the existing 2047 tests guarded it.
- **One method per node:** `bool P{n}(int pos, out int end, out T value)` in a nested `__Parser` class. Wrappers get their own method,
  and their memo table is keyed by the *inner* method name. That mirrors `MemoTable[position, inner]` in the implementations.
- **Delegates become `CodeTemplate`s** (`(emitter, args) => expression`). The arguments are variables or member accesses, so a template may use them twice.
  `Code.Compose` binds the inner result to a temp first. `Transform` and `Aggregate` capture user lambda text with `[CallerArgumentExpression]`,
  which PolySharp polyfills for `netstandard2.0`. Library combiners (`Then`, `ThenSkip`, `ThenAdd`, `Many`) pass hand-written templates.
  Internal calls must not go through the public `Transform`, or they capture the wrong text (`x => transformation(...)`). Use the private `Parsers.Map` helper.
- **User code is wrapped in try/catch**, like `MapRefParserImpl` and `SequenceParserImpl.Combine`. The emitter evaluates the template into a side buffer,
  so it knows whether a user delegate was involved before it decides on the `try`.
- **Closure check:** reject when `Delegate.Target` has instance fields, which means a compiler display class.
  Non-capturing lambdas target the `<>c` singleton, which has none.
- The user lambdas live as `static readonly` fields of the *outer* user class, not of `__Parser`, so their names can't clash with the parser's members.
  The file always imports `System`, `System.Collections.Generic` and `System.Linq`. `usings` entries can be aliases (`"G = Full.Name"`),
  but an alias target must be fully qualified, because aliases can't see the other usings.
- Not supported (they throw `NotSupportedException` naming the node): `Bind` (and the `Then`/`ThenAdd`/`ThenSkip(Func)` overloads),
  `AsResult`, `Create` with a value that has no literal, and lambdas that capture variables. There is no iterative mode: deep input throws `InsufficientExecutionStackException`.

## Testing

- `Warpstone.Tests/SourceGeneration` compiles the output in memory with Roslyn (`Microsoft.CodeAnalysis.CSharp`, added to `Warpstone.Tests`)
  and compares the success, value, furthest error position, expected set and exception count with `TryParse`.
  Grammar transformations must be public or static-only, because the dynamic assembly can't see internals.
- Pass the `NETx_0_OR_GREATER` preprocessor symbols to `CSharpParseOptions`, or the `#if NET7_0_OR_GREATER` regex branch is never compiled in the tests.
- MiniJava: `dotnet run -c Release --project Warpstone.Benchmarks -- generate-minijava Warpstone.Benchmarks/MiniJava` regenerates
  `Warpstone3Generated*.g.cs`. `verify-minijava` checks that they give the same AST. Regenerate after any change to the emitter or the grammar.

## Performance (MiniJava, ShortRun, `--inProcess`, .NET 11)

| | small | medium | large |
|---|---:|---:|---:|
| Warpstone3 (interpreted) | 31.6 μs / 14.7 KB | 369 μs / 94 KB | 3.56 ms / 865 KB |
| generated, first version | 31.7 μs / 12.6 KB | 396 μs / 127 KB | 3.73 ms / 1,264 KB |
| generated, optimized | 15.0 μs / 9.5 KB | 180 μs / 95 KB | 1.78 ms / 948 KB |
| Warpstone3 left-recursive (interpreted) | 50.5 μs / 22.1 KB | 583 μs / 179 KB | 6.01 ms / 1,695 KB |
| generated left-recursive, first version | 42.4 μs / 29.6 KB | 498 μs / 314 KB | 5.77 ms / 3,371 KB (Gen2) |
| generated left-recursive, optimized | 20.1 μs / 19.4 KB | 245 μs / 198 KB | 2.43 ms / 1,970 KB |

The first version was no faster than the interpreter. What fixed it:
1. **First-char `Or`**: the emitter builds the implementations too (`ParserPlan.CreateImplementations`) and reads `FirstChars` from them,
   then emits a `byte[129]` char-to-plan table and a `switch`. Skipped alternatives `Expect` their reports, so errors stay identical.
2. **Failures in an `object[]`**, deduplicated with `ReferenceEquals`: expected texts are literals, so they are interned. Return early behind `failPos`.
3. **Match-only methods `P{n}M(pos, out end)`** for nodes whose value is unused (`Then`, `ThenSkip`, delimiters, `Not`, the inside of match-only nodes).
   `Analyze(template)` evaluates a template with sentinel arguments to find which arguments it uses and whether it runs user code.
   User code still runs in match-only mode (into `_ = ...`), because it may throw. Wrappers (memo, grow) always run in full.
4. **Shared memo rows**: one generated `__Row` class with a typed field per memo table, chunked 256 positions per array, plus a last-row cache.
   A `Dictionary` per table, even chunked, cost 2× the interpreter's memory, because the 7 grow levels memoize the same positions.
5. **Lambdas as static methods** (`Lambda.TryParse` splits the parameters and body; discards get unique names). Method groups like `int.Parse` are called directly.
   Delegate fields remain only as a fallback.

## Regexes become loops (generator only)

`SimpleRegex.TryEmit` turns a regex into a static `int __H{n}(string input, int pos)` scanner when it can; otherwise `RegexParser.EmitSource`
compares the literal prefix (`RegexParserImpl.FindLiteralPrefix`, now `internal`) before calling the `Regex`. Small leaf parsers get `[MethodImpl(AggressiveInlining)]`.
- **Exact sets by brute force:** every atom (`[...]`, `\w`, `\p{L}`, a literal) becomes a `bool[65536]` by asking .NET's `Regex` `^(?:atom)\z` about each char.
  Word chars for `\b` likewise (`^a\B` against `"a" + c`). They are emitted as compares (up to 4 ranges), or as an ASCII bit mask plus a Unicode category mask
  plus an XOR list of exceptions. `\w` needs the exceptions: U+200C/U+200D are word chars, but the rest of their category (Cf) is not.
- **No backtracking:** convert only when greedy equals backtracking. A repeated set must be disjoint from the sets that can follow it,
  up to and including the first required one. A repeated set before `\b` needs `min >= 1` and all-word or all-non-word members.
  A lookahead must not follow a repeated set. Alternation is only allowed inside a negative lookahead, where it is expanded.
  `[^a-c]{2,3}y` is correctly rejected, because `y` is in `[^a-c]`.
- Tests compare ~310 random and fixed inputs per pattern with the interpreter, and assert which patterns convert.
- MiniJava: every regex converts, including the keyword-excluding identifier `(?!(?:class|...)\b)[A-Za-z_]...`, which becomes a `switch` on the first character.
  Generated/interpreted went from 0.49 to 0.40 (large) and 0.47 to 0.39 (small). That is now 1.14-1.18x Parlot's generated parser.

## Pitfalls

- **Verify snapshots** are TFM-agnostic (`Snapshots.Generates#...verified.cs`), but the received files carry the TFM (`Snapshots.Generates.DotNet10_0#...`).
  Renaming received to verified in place creates the wrong names. Strip the `.DotNetXX_0` part.
- Run the Sources tests with `DiffEngine_Disabled=true`, or DiffEngine launches vim for every changed snapshot and leaves `.swp` files (and `vim.exe` processes) behind.
- `dotnet pack` rewrites `CompatibilitySuppressions.xml` (`ApiCompatGenerateSuppressionFile`) with a lot of unrelated churn.
  Diff the suppression *sets* first. This change added none, so the original file was restored.
- A full BenchmarkDotNet run prints the version and runtime lines once per benchmark, so `| head -60` cut off the summary.
  The tables are in `src/BenchmarkDotNet.Artifacts/results/*-report-github.md` anyway.
- The generated file needs `#pragma warning disable`: `// <auto-generated/>` silences analyzers but not compiler warnings like CS0649 on unused helpers.
- Python `'\\n'` inside a bash heredoc came out as a real newline in C# string literals. Write scripts with the Write tool.
- Bash heredocs that contain both `'''` Python strings and C# quotes broke (`unexpected EOF`). Write the script with the Write tool instead.
