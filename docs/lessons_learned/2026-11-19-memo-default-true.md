# 2026-11-19 — `EnableAutomaticMemoization` default to `true`

## Change

- `ParseOptions.EnableAutomaticMemoization` default `false` → `true` (`src/Warpstone/ParseOptions.cs`, doc comment too).
- All 1498 existing tests passed unchanged with memo on.

## Coverage audit

- Tests that set options explicitly (`Legacy/BasicParsersTests`, `Legacy/CommonParsersTests`, `Parsers/MemoParser`, `Parsers/LeftRecursiveMemoParser`, `Expressions/LeftRecursionGrammars`) are unaffected by the default.
- Tests on defaults silently flipped from memo-off to memo-on. Memo only applies when the grammar has a node with occurrence count > 1 (`ParserBase.GetSimplifiedOptions`), so only grammars with shared nodes matter:
  - `Expressions/LeftRecursive`, `Expressions/StackDepth`: now `[TestFixture(true)] [TestFixture(false)]` with a `bool memo` primary constructor.
  - Json `Complex.Simple_object`: `[TestCase(true/false)]`; `JsonParser.Parse` takes optional `ParseOptions?`.
  - Not changed: Json scalar tests, `AnalysisScaling`, and `AsResultParser` (no shared nodes, so memo gets simplified away); `BasicParsersTests` already covers both memo settings for basic parsers.
- Result: 1539 passed, 12 skipped (`TransformationExceptionCorrect`, skipped on purpose).

## Pitfalls

- Git Bash `sed -i` stripped CRLF from the test files, and patterns anchored on `\r$` then didn't match. Use the Edit tool on CRLF files, or restore line endings afterwards with Python (`replace(b'\n', b'\r\n')`).
- A failing Verify snapshot made DiffEngine open `vim.exe` and leave a `.…verified.cs.swp` in `Warpstone.Sources.Tests/Helpers/snapshots/`. Delete the swap file after accepting (rename `.received.cs` → `.verified.cs`).
- `dotnet test` restore works again in this environment (the restore failure from earlier lessons didn't happen).
