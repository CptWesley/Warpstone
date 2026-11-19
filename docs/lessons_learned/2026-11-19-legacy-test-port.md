# 2026-11-19 — Port V1/V2 unit tests to V3

## Where the legacy tests live

- **V1 final**: commit `b27fb56` (tag `1.0.8.1` is older, 42 commits behind). Matches `Warpstone.Benchmarks/Legacy/Warpstone1` (has `TryParser`). Tests: `src/Warpstone.Tests/{Parsers,Grammars}/*.cs`.
- **V2 final**: commit `7d7113b`, parent of `ac48cd7` "WIP V3". Matches `Legacy/Warpstone2`. Branch `origin/v2` is an *early* V2 (ParseState era), not the final one.
- Extract with `git show <ref>:<path>`; list with `git ls-tree -r --name-only <ref>`.

## State found

- V2 suite was already in V3 (`Legacy/`, JSON example, `Expressions/`, `AsResultParser`). Only `SelectParser` (monadic `Then(Func)`) was dropped.
- V2 had dropped V1's Maybe/IParsed/TransformationError/Position basic tests and all Common/Number/Expression/BNF tests. Now ported where V3 has the parser: 8 into `Legacy/BasicParsersTests.cs`, 3 whitespace tests into new `Legacy/CommonParsersTests.cs`.
- `TransformationExceptionCorrect` fails on purpose: V3 `TransformationError.Message` is generic, not inner message + `" At 1:1."`.

## Running tests

- `Warpstone.Tests` targets **net8.0 only** (`executable.props`), so `-f net9.0` is wrong.
- Restore of `Warpstone.Tests` fails in locked mode with `NU1102 System.Reflection.Metadata (>= 8.0.0)` under SDK 11, and `NU1101` under SDK 9 via temp `global.json`. Not a cache problem (fresh `NUGET_PACKAGES` gave the same). Not fixed.
- Workaround that ran the full suite: scratch dir with two projects, both `ImportDirectoryBuildProps=false`, `ImportDirectoryPackagesProps=false`, `EnableDefaultCompileItems=false`:
  - `lib/Warpstone.csproj` compiling `src/Warpstone/**/*.cs` (include `Properties/`, needs `AllowUnsafeBlocks` for `SkipLocalsInit`).
  - `runner/Runner.csproj` compiling `src/Warpstone.Tests/**/*.cs`, referencing lib, with xunit/AssertNet/AwesomeAssertions/Test.Sdk and the `default_imports.props` usings (incl. `System.Collections.Immutable`).
  - Must be separate projects: the tests' `global using static Warpstone.Parsers` breaks `RegexParserImpl` (`Regex` resolves to the method) if compiled together.
- Roslynaut `diagnostics` can't see test files while restore fails ("not part of the loaded project").
