# System.IO.Hashing and Proj**** warnings

## System.IO.Hashing
- Warning `System.IO.Hashing 11.0.0 doesn't support net9.0 ...` came from `Microsoft.SourceLink.GitHub` 11.0.100 (via `Microsoft.Build.Tasks.Git`). Its `buildTransitive` targets warn on every TFM below net10.0.
- Fix: removed the `Microsoft.SourceLink.GitHub` package. The .NET 8+ SDK bundles SourceLink, so `EnableSourceLink` stays `true` (check: `dotnet msbuild Warpstone/Warpstone.csproj -getProperty:EnableSourceLink -p:TargetFramework=net9.0`).
- Avoid `SuppressTfmSupportBuildWarnings`: it hides the warning for every package, not just this one.

## DotNetProjectFile.Analyzers (Proj rules)
- **Stale results:** the analyzer caches parsed project files inside the compiler server. After editing `.props` files, run `dotnet build-server shutdown` before rebuilding, otherwise old warnings (pointing at lines that no longer exist) remain.
- Proj6010/6013/6014: `global.json` at repo root has to pin the SDK exactly (`rollForward: disable`, `allowPrerelease: false`) because lock files are on. CI (`setup-dotnet`) must install that exact SDK.
- Proj0807: `DisableImplicitNuGetFallbackFolder` moved from `Directory.Packages.props` to `common.props`.
- Proj3003: the trailing space was inside `Include="AwesomeAssertions "`, which had been paired with a separate `Update` item. Merged them into a single `PackageVersion` entry.
- Proj0006: every imported `.props` file must be listed in `AdditionalFiles` in `common.props` (`default_imports.props` was missing).
- Proj1703: a blank line is required after `<Project>` and before `</Project>`. Proj3000: no BOM.
- Proj0012: package metadata was duplicated in `Warpstone.csproj` and `package.props`, so the csproj copy was removed.
- Turning on `EnforceCodeStyleInBuild` (Proj0050) added no new IDE warnings.

## CI pipeline
- `setup-dotnet` now uses `global-json-file: global.json` to install the pinned SDK, plus the 8/9/10 runtimes the tests run on (11 comes with the SDK). It no longer installs 3.0–7.0: nothing runs on those, and the SDK gets their reference packs from NuGet.
- Restore uses `--locked-mode`, which resolves the old TODO. It passes locally with the committed lock files.
