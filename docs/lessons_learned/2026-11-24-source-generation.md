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
| Warpstone3 | 31.5 μs / 14.7 KB | 329 μs / 94 KB | 3.49 ms / 867 KB |
| Warpstone3_generated | 31.7 μs / 12.6 KB | 396 μs / 127 KB | 3.73 ms / 1,264 KB |
| Warpstone3_left_recursive | 45.0 μs / 22.1 KB | 571 μs / 179 KB | 5.60 ms / 1,697 KB |
| Warpstone3_generated_left_recursive | 42.4 μs / 29.6 KB | 498 μs / 314 KB | 5.77 ms / 3,371 KB (Gen2) |
| Parlot_generated | 9.3 μs / 6.5 KB | 112 μs / 67 KB | 1.17 ms / 673 KB |

The first version was **not faster** than the interpreter. Likely causes, none of them measured yet:
- `Or` tries every alternative. The interpreter skips alternatives with its first-char filter and replays their reports.
- `Expect` does a linear `List.Contains` for every failure.
- `Substring` for every regex and whitespace match. The interpreter interns common runs and regex matches.
- One `Dictionary<int, __Entry<T>>` per memo table grows onto the LOH (Gen2 in the left-recursive grammar), where the interpreter uses a chunked slot table.
- Transformations are delegate calls rather than inlined code.

## Pitfalls

- **Verify snapshots** are TFM-agnostic (`Snapshots.Generates#...verified.cs`), but the received files carry the TFM (`Snapshots.Generates.DotNet10_0#...`).
  Renaming received to verified in place creates the wrong names. Strip the `.DotNetXX_0` part.
- Run the Sources tests with `DiffEngine_Disabled=true`, or DiffEngine launches vim for every changed snapshot and leaves `.swp` files (and `vim.exe` processes) behind.
- `dotnet pack` rewrites `CompatibilitySuppressions.xml` (`ApiCompatGenerateSuppressionFile`) with a lot of unrelated churn.
  Diff the suppression *sets* first. This change added none, so the original file was restored.
- Bash heredocs that contain both `'''` Python strings and C# quotes broke (`unexpected EOF`). Write the script with the Write tool instead.
