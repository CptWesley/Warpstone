# Stream and TextReader input, async parsing

## What was added
- `ParseInput.Create(IParseInputSource, TextReader | Stream)`, `ParseInput.CreateFromReader(TextReader)` and `ParseInput.CreateFromStream(Stream)`. They return the internal `ReaderParseInput` (`src/Warpstone/ReaderParseInput.cs`), which reads on demand.
- `IterativeParseContext<T>.RunToEndAsync` and `ParserExtensions.TryParseAsync<T>`/`ParseAsync<T>`. These always run iteratively.
- Tests: `src/Warpstone.Tests/ParseInputSources.cs`. They use a channel-fed `TextReader` whose sync `Read` throws, so a blocking read fails the test.

## Design
- `IParseInput.Content` stays a `string`. For a reader input it holds the text read so far, and all of that stays buffered, so backtracking simply re-reads it.
- The 6 leaf impls (Character, String, End, Whitespaces, OptionalWhitespaces, Regex) call `ReaderParseInput.RequestMore(input, end)`, and only on the branch where they would otherwise fail at the end of `Content`. String inputs therefore pay nothing on the success path. Regex asks for `int.MaxValue`, which means the whole input.
- Retry when `RequestMore` returns true: recursive mode does `return Apply(context, position)`, iterative mode pushes `(position, this)` back onto `ExecutionStack` and returns.
- Async: `RunToEndAsync` sets `IsAsynchronous`. `Request` then only records `pendingEnd`, the step loop stops on `IsPending`, it awaits `ReadPendingAsync`, and then it resumes. Sync contexts block in `reader.Read`.
- Each read asks for at least as much as is already buffered (minimum 4096 chars). Rebuilding `Content` with `string.Concat` stays linear that way. The ceiling: a read can wait for more input than the parser needs right now, which is fine for sources that end and not for request/response protocols. The upgrade path is a `char[]` buffer with impls that read through it.

## Measured (ShortRun, before vs after, string input)
MiniJava small/medium/large and RightRecursiveSimpleString: allocations identical, times within noise.

## Pitfalls
- Sonar S4136: all `Create` overloads must be adjacent.
- Leaf impl files use CRLF. Patch them with a script that normalizes line endings, or with Edit.
- `StringParserImpl`: don't call `Matches` twice on failure (`Or` chains fail often); checking the length alone is enough to decide whether to request more.
- Roslynaut `diagnostics` reported bogus CS0518 errors for the new file; `dotnet build` was clean.
- `dotnet build -f net9.0` rewrote `Warpstone.Sources/packages.lock.json`; restore it with `git checkout`.
- Verify left a vim `.swp` file in `Warpstone.Sources.Tests/Helpers/snapshots/`. Delete it after accepting snapshots, and check `git status` for staged leftovers after `git stash pop`.
- `git worktree add` into the scratchpad failed with "Filename too long" (snapshot names). Benchmark before/after in place with `git stash push -u` instead.
