# Stream and TextReader input, async parsing

## What was added
- `ParseInput.Create(IParseInputSource, TextReader | Stream)`, `ParseInput.CreateFromMemory(TextReader | Stream)`, `ParseInput.CreateFromFile(path, TextReader | Stream)` and `ParseInput.CreateFromStream(Stream)`. They return the internal `TextReaderParseInput` (`src/Warpstone/Internal/Input/TextReaderParseInput.cs`; string input is `Internal/Input/StringParseInput.cs`, `ParseInput` is now a static factory class), which reads on demand.
- `IterativeParseContext<T>.RunToEndAsync` and `ParserExtensions.TryParseAsync<T>`/`ParseAsync<T>`. These always run iteratively.
- Tests: `src/Warpstone.Tests/ParseInputSources.cs`. They use a channel-fed `TextReader` whose sync `Read` throws, so a blocking read fails the test. Its `Delivered` count shows progress, because reading `Content` would force a full read.

## Design (second version; the first used a `RequestMore` hook inside the impls)
- `IParseInput` has match methods: `Match(index, char)`, `Match(index, string, culture, options, out match)`, `Match(index, Regex, out match)`, `MatchWhitespaces`, `MatchEnd`, plus `Substring` and `TryGetChar`. They return `InputMatch { Failure, Success, Pending }`.
- `Content` always means the whole input; a reader input reads to the end, blocking. No library code calls it during parsing, except Regex on targets below .NET 7.
- `Substring`/`TryGetChar` read synchronously up to the requested index, even under `ParseAsync`. `UnexpectedTokenError.Found` uses `TryGetChar`.
- Impls: recursive mode checks `== InputMatch.Success`; iterative mode uses a switch where `Pending` pushes `(position, this)` back onto `ExecutionStack`. `Pending` only occurs while `RunToEndAsync` has set `TextReaderParseInput.IsAsynchronous`.
- `TextReaderParseInput` buffers in a doubling `char[]`, not a `StringBuilder`. The `StringBuilder` indexer walks its chunk list, which is slow for backtracking. Appends are amortized O(1), so each read takes only what the reader has ready, and no read waits for more than the parser needs.
- Regex needs the whole input. On .NET 7+ it matches a span of the buffer (`EnumerateMatches`); below that it builds `Content`. Once `Content` exists, the input delegates to a `StringParseInput` over it and drops the buffer (see [2026-11-19-ponytail-comments.md](2026-11-19-ponytail-comments.md)).
- `ParseInput` (string) on .NET 7+ also uses `EnumerateMatches`, which skips the `Match` allocation; MiniJava allocates ~2.4% less.

## Measured (ShortRun, string input)
MiniJava and RightRecursiveSimpleString: no time regression (differences within noise); allocations equal or lower.

## Pitfalls
- Sonar S4136: all `Create` overloads must be adjacent. Sonar S1751: `foreach` that returns on its first iteration; use `MoveNext()` on the `EnumerateMatches` enumerator.
- Leaf impl files use CRLF. Patch them with a script that normalizes line endings.
- Roslynaut `diagnostics` reported bogus CS0518 errors for new files; `dotnet build` was clean.
- `dotnet build -f net9.0` rewrote `Warpstone.Sources/packages.lock.json`; restore it with `git checkout`.
- `dotnet pack` rewrites `CompatibilitySuppressions.xml` (`ApiCompatGenerateSuppressionFile=true`) with unrelated nullable-attribute churn. Revert it unless the change really needs a suppression. The new `IParseInput` members needed none, because baseline 1.0.8.1 predates `IParseInput`.
- Verify leaves a vim `.swp` file in `Warpstone.Sources.Tests/Helpers/snapshots/`; delete it after accepting snapshots.
- Benchmarking before/after in place: `git worktree add` into the scratchpad fails ("Filename too long"). `git stash push -u -- <path>` followed by `git stash pop --index` refused to pop because the user had an unstaged, unrelated `README.md` edit, and the stash was kept. Recovery: `git checkout stash@{0} -- <path>`, `git restore --staged -- <path>`, `git restore --source='stash@{0}^2' --staged -- <staged files>`, compare untracked files against `stash@{0}^3`, then drop the stash. Next time: back up first, and don't benchmark while the user is editing files.
