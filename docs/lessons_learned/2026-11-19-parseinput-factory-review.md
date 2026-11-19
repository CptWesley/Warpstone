# Review of the ParseInput refactor (static factory + Internal/Input)

## What changed (by the user)
- `ParseInput` became a static factory class. String input moved to `Internal/Input/StringParseInput.cs`, reader input was renamed to `Internal/Input/TextReaderParseInput.cs`.
- `CreateFromReader(TextReader)` was replaced by `CreateFromMemory(TextReader)`. New overloads: `CreateFromMemory(Stream)`, `CreateFromFile(path, TextReader)`, `CreateFromFile(path, Stream)`.

## Issues found and fixed
- `CreateFromFile(path)` had become `CreateFromFile(path, File.OpenRead(path))`. Reader inputs never dispose the stream (`leaveOpen: true`), so the file handle leaked and the file stayed locked on Windows until GC. Reverted to `File.ReadAllText(path)`, which is also faster (string input). Regression test: `CreateFromFile_does_not_keep_the_file_open` writes to the file right after creating the input; it fails with `IOException` on the old code.
- `StringParseInput` constructor was undocumented (SA1600). Unused usings removed.
- The new TextReader/Stream overloads now carry the "not disposed" and encoding remarks that `CreateFromReader` had.
- Tests: `CreateFromReader` → `CreateFromMemory`. New tests cover every factory overload (source kept, from-memory source, file source, stream left open).

## Pitfalls
- Verify's DiffEngine opens `vim -d` for each changed snapshot and leaves the processes running, holding `.swp` files. Set `DiffEngine_Disabled=true` before `dotnet test`. Kill only vim processes whose command line points at `*.received.cs` and whose start time matches your test run.
- Verify auto-creates `.verified.cs` for new embedded files. Renamed files leave a stale `.verified.cs` behind (here `ReaderParseInput`); delete it with `git rm`.
