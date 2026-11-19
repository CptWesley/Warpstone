# 2026-11-19 — Resolving the `ponytail:` comments

## What was resolved

- `TextReaderParseInput.Content` used to keep both the `char[]` buffer and the built string, so the input was in memory twice. Once `Content` is built, every match now delegates to a `StringParseInput` over that string, and the buffer is replaced by an empty array. `GetPosition` still uses the reader's own `lineStarts`, so positions point at the reader input. Below .NET 7, regex matching goes through the same delegate.
- The "Pending only from iterative parsers" note was an invariant, not a shortcut. It now lives in the `IsAsynchronous` XML doc.
- `FindRetried` was O(parsers × closure) on deep choice chains. The real cause was `Parsers.Or(params)`, which built a left-nested chain n deep. `PerformAnalysisStep` copies its trace at every step, so that chain made the analysis quadratic too, and the analysis overflowed the stack at about 4000 alternatives that share a prefix. `Or` now builds a balanced tree. Two and three alternatives keep their old shape (`Or(Or(a, b), c)`).

## Measured (net9.0 Release, scratch probe, `Or` of n `x.Then(String("a" + i))`)

| n | before, memo on | after, memo on |
|---|---|---|
| 2000 | 808 ms | 41 ms |
| 3000 | 1347 ms | — |
| 4000 | stack overflow in `Analyze()` | — |
| 32000 | — | 794 ms |

## Pitfalls

- Commit `e7963b0` switched `ReadPendingAsync` to `reader.ReadAsync(buffer, cancellationToken)`. A `char[]` converts to a `Memory<char>` over the whole array, so every read wrote over the start of the buffer. `Async_parse_reads_input_as_it_arrives` caught it. The fix is `buffer.AsMemory(length)`.
- A build rewrote `CompatibilitySuppressions.xml` with entries left over from the `InputMatch` work. This change reverted it to keep the diff scoped. Check it before committing.
- The snapshot test still opens vim through DiffEngine and leaves `.swp` files behind. Delete them, and close the `vim.exe` processes it leaves running.
- A probe that measures analysis cost must use a grammar with a shared node. Without one, `GetSimplifiedOptions` skips memoization, and `FindRetried` never runs.
