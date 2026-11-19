# 2026-11-19 — Migrate test projects from xunit to NUnit

## What changed

- `src/Directory.Packages.props`: dropped `xunit`, `xunit.analyzers`, `xunit.runner.visualstudio` (all per-TFM groups), `Verify.Xunit`. Added `NUnit` 4.6.1, `NUnit.Analyzers` 4.15.0, `NUnit3TestAdapter` 6.3.1, `Verify.NUnit` 30.4.0 (kept equal to the `Verify` pin so transitive pinning doesn't downgrade-conflict).
- `src/tests.props`: package refs + global `<Using Include="NUnit.Framework" />`.
- Attribute mapping: `[Fact]`→`[Test]`, `[Theory]` removed, `[InlineData]`→`[TestCase]`, `[MemberData]`→`[TestCaseSource]`, `[Theory(Skip = "...")]`→`[Ignore("...")]`. Static classes work as NUnit fixtures; no `[TestFixture]` needed.
- `Warpstone.Sources.Tests`: `global using VerifyNUnit;`. Snapshot file names unchanged (`Snapshots.Generates#...verified.cs`).

## Pitfalls

- **Array covariance**: data sources declared `IEnumerable<object[]> X = new ParseOptions[][] { ... }` broke under NUnit — it passes the `ParseOptions[]` row as one argument ("Object of type 'Warpstone.ParseOptions[]' cannot be converted to type 'Warpstone.ParseOptions'"). xunit unpacked it. Fix: declare rows as `new object[][]`.
- Python heredoc with `'\obj\'` broke the whole conversion script silently-ish (SyntaxError, nothing written). Write scripts to the scratchpad and use `os.sep`.
- Preserve CRLF/BOM when rewriting files by script; several test files are LF, others CRLF.
- DotNetProjectFile `Proj0024` wants `PackageVersion` entries alphabetical (`Verify.NUnit` before `Verify.SourceGenerators`).

## Running

- `dotnet restore --force-evaluate` regenerated both lock files fine under SDK 11 this time (earlier NU1102 restore failure did not reproduce).
- Result: `Warpstone.Tests` 1402 passed, 12 ignored (`TransformationExceptionCorrect`); `Warpstone.Sources.Tests` 2 passed.
