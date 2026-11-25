# Profiling round 3: interpreter and generated parser

The first part was analysis only; options 1–3 were implemented afterwards, see "Implemented" at the end. MiniJava `large`, net11.0 Release, stopwatch harness (best of 5 slices after 2 s warm-up), one process per variant, variants alternated 3–4×.
Noise on this machine was ±5%, so treat anything under 5% as unproven.

## Baseline

| | µs | KB |
|---|---:|---:|
| interpreter | 2,150 | 868 |
| interpreter left-recursive | 3,920 | 1,695 |
| interpreter iterative | 4,680 | 1,014 |
| generated | 640–690 | 902 |
| generated left-recursive | 1,240–1,320 | 1,924 |
| Parlot generated | 950–1,010 | 673 |

## Measured experiments (generated parser, edited copy of the `.g.cs`)

| Variant | time | bytes |
|---|---:|---:|
| Direct left recursion as a loop (LR grammar): seed = base alternative, then `while (op && operand) value = combine(...)`, no grow memo | **−42%** (1.32 → 0.76 ms) | same |
| same, but keep the grow memo read/write | −32% | same |
| `[AggressiveInlining]` on every method with ≤12 body lines and no `try` (200 methods) | **−7%** | same |
| Per-parser scratch stack for `Many` (one `T[]` + top index per element type, exact `T[]` result) | ~−4% | **−18%** |
| Interning regex matches (direct-mapped `string?[512]`, as the interpreter does on .NET 9+) | ±0 | **−18%** |
| Scratch stack + interning | ±0…−2% | **−36%** |
| `ArrayPool<T>.Shared` for `Many` | **+22%** (rejected) | −18% |
| One `if (track) { ... }` around each run of `Expect` calls | ≈ −1% | same |
| No stack guards | ≈ −2% | same |

- LR memory didn't move without the grow memo because generated memo rows are shared: the remaining `MemoParser` creates the rows, the grow tables only fill fields in them.
- `Many`'s `List<T>` costs: list object, grow-by-doubling garbage, and the grammar's `Seq` copies the list again anyway.

## Interpreter measurements

- Call counts per parse (`process_calls`, `per: ParserExtensions.Parse`), plain grammar: Sequence 61K, Combine 49K, Or 35K + Plan 33K, Aggregate 22K (only 4.2K `AddToList`: the 6 `Chain` levels are mostly empty), And 20K, leaves ~38K total. LR grammar: Grow 74K, StackGuard 84K, `MemoTable.TryGetSlot` 122K, `Set` 46K.
- Allocation profile: ~25% of bytes is `Many` list machinery, ~7% boxed `ValueTuple`s from `ThenAdd` chains and tuple elements of `Many`.
- Skipping `ReportAll`/`AddExpected` when the tracker is disabled (44K calls per parse): ≈0. Not worth it.

## Options not measured (bigger work)

- The interpreter is 3.3× slower than the generated parser on the same call graph. The gap is per-node dispatch (interface calls on shared `__Canon` generics, 24-byte `UnsafeParseResult` returns, boxing through `object`, `Cast`).
  Runtime compilation (expression trees or `DynamicMethod` that follow the `SourceEmitter` plan) is the only way to close most of it.
- N-ary sequence fusion (`a.ThenAdd(b).ThenAdd(c).Transform(f)` as one node calling `f` once) removes the intermediate tuple boxing and one virtual call per level.

## Setup and pitfalls

- Harness: a scratch console project referencing `Warpstone.Benchmarks.csproj` (`ManagePackageVersionsCentrally=false`), modes call the public `MiniJavaBenchmark` methods.
  A `copy` mode parses with `Harness.GenCopy`, a copy of the `.g.cs` with the class renamed, and first checks that the AST equals the interpreter's.
- Variants: a Python script rewrites the copy (regexes over the emitted method shapes), then `dotnet build -c Release -o out/<variant>`, so every variant keeps its own output dir and runs in its own process.
- **Building the harness or running roslynaut profiling rewrote `packages.lock.json` in `Warpstone.Tests`, `Warpstone.Sources` and `Warpstone.Sources.Tests`.** Check `git status` afterwards and revert them.
- `process_calls` counts calls to property getters that the JIT normally inlines (`StringParseInput.Content`: 209K per parse). Ignore those.

## Implemented (options 1–3)

Harness medians of 6 alternated runs, MiniJava large, before → after:

| | before | after |
|---|---:|---:|
| interpreter | 2,157 µs / 865 KB | 2,128 µs / 705 KB |
| interpreter left-recursive | 4,045 µs / 1,692 KB | 3,341 µs / 1,590 KB (−17%) |
| interpreter iterative | 5,072 µs / 1,013 KB | 4,929 µs / 871 KB |
| generated | 679 µs / 902 KB | 647 µs / 741 KB |
| generated left-recursive | 1,337 µs / 1,924 KB | 1,072 µs / 1,819 KB (−20%) |
| Parlot generated | 990 µs / 673 KB | |

1. **Grow rounds skip what can't change** (`GrowParser.RecursiveAlternatives`, `LeftRecursionAnalysis.CountLeftRecursiveAlternatives`).
   A leader that is an `Or` whose first k alternatives reach the leader through `Seq`/`And`/`Map` first children only, and whose other alternatives are outside the leader's own component:
   round 1 runs only the other alternatives (the k fail without a seed), later rounds run only the k (the others would return the round-1 seed, which never improves).
   The grow memo stays, so results are identical; the no-memo loop of the prototype (−42%) would need each sequence impl to accept a given left result.
   - Pitfall: checking the bases against `LeftRecursionInfo.Members` (all cycles) kept 5 of MiniJava's 6 levels on the old path, because the next precedence level is a member of its own cycle.
     The analysis now records each leader's component (`Components`).
   - Pitfall: the emitter emitted wrapper methods inside `ParserPlan.Execute`, with a one-entry lookup, so a wrapper couldn't call the wrapped choice's alternatives. Wrappers are now emitted after `Execute`, with the full lookup plus their own parser mapped to the inner node.
   - Skipping round 1's recursive alternatives was worth as much as skipping later rounds' bases (−12% → −20% on LR).
   - Iterative mode: one `AlternativeContinuation` per alternative, which knows the last index of its range.
   - Test `Growing_runs_the_base_alternative_once` counts base transformations (4, was 5); forcing k = 0 makes it fail.
2. **Inlining** (`SourceEmitter.IsSmall`): at most 32 lines without a `try`, not counting `Expect(` lines and braces.
   The hot ones are first-character `Or`s between operators, mostly dead `Expect` lines. 12 lines (the plan) gained nothing: the prototype's −7% came from a regex that stopped at the first nested `}`, so it measured a different set. Allowing `try` (.NET 10 can inline it) added nothing.
3. **`Many` on scratch stacks** (`ScratchStacks`, `AggregateParser.CollectsList`, set only by `Parsers.Multiple`). Results are now exact-size `T[]`, not `List<T>`.
   - Interpreter: the contexts implement the internal `ScratchStacks.IOwner` explicitly. One stack per element type (`Slot<T>.Index`), so nested repetitions of a type share it LIFO; a repetition that fails after collecting truncates back.
   - **Looking the stack up on every call made the interpreter 9% slower**: 22K `Many` calls per parse, most of them empty (the `Chain` levels). Recursive mode now looks it up on the first element; iterative mode already only builds its continuation then.
   - Generator: a generic `__Stack<T>` class ran as shared generic code and cost about 7% compared with typed `T[]` fields plus a count field, with the push and pop written inline. Uses `Array.Copy`, not `AsSpan`, since generated files must compile on `netstandard2.0`.
   - Tests: `Nested_repetitions_of_the_same_type_keep_their_own_elements`, `Failed_repetition_drops_its_elements` (fails without truncation, all modes) and the generated `NestedLists` grammar (fails without the generated reset).
4. **Interning matched text in the generated parser** (`__Parser.Intern`, used by all three regex value paths in `RegexParser.EmitSource`).
   Direct-mapped `string?[]` like the interpreter's, but portable: the hash is an `unchecked` `h * 31 + c` loop and the compare is `string.CompareOrdinal`, so it needs no span APIs on `netstandard2.0`, and an overflow-checked consumer build can't throw.
   Matches over 32 characters skip it. Large: −22% bytes (−9% left-recursive), about +2% time, the same trade-off as in the interpreter.
   - A fixed 512-slot table (4 KB) made the small input +36% bytes and +8% time. The table now has a slot per 16 input characters, between 16 and 512, created on first use: small went to −2% bytes.
   - Tests: `Repeated_matches_share_one_string` (`BeSameAs`) and `Interned_matches_that_collide_keep_their_text` (4,000 words in 512 slots plus a 40-character word; fails if the text compare is dropped).

### Pitfalls this round

- Python on Windows can't open `/c/...` paths; pass `C:/...`. Inline `python -c` with nested quotes in bash silently broke the patterns, so write variant scripts with the Write tool.
- A `cat > file` with no heredoc waits on stdin and hangs the Bash call until its timeout.
- Measurement noise rose to ±15% while other apps were busy. `ab.py` (alternate variants, one process each, print min and median) is more trustworthy than single rounds.
- `dotnet pack` rewrote `CompatibilitySuppressions.xml` again with unrelated churn. Nothing referenced the new types, so it was restored with `git checkout`.
