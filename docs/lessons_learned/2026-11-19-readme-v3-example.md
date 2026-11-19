# README V3 usage example

Replaced the V1/V2/V3 import snippets under `## Usage` with install instructions and a complete calculator console app (left-recursive `Term`/`Sum`, `Parse` and `TryParse`).

## Issues and fixes

- **Verify the example by running it.** Scratch console project in the scratchpad with `ImportDirectoryBuildProps=false`, `ImportDirectoryPackagesProps=false` and `<Compile Include=".../src/Warpstone/**/*.cs" Exclude="Properties/**;obj/**;bin/**" />` builds without packages. Paste the README code into its `Program.cs` and run it.
- **`ThenEnd()` inside a nested rule.** First draft used the entry point `Expression` (which ends in `ThenEnd()`) inside parentheses, so `(1 + 2) * 3` failed. Recurse into the rule without `ThenEnd()` (`Sum`).
- **Nullable warnings on self-referencing static fields.** `Lazy(() => Term)` gives CS8603 because the field isn't assigned yet. Use `Lazy(() => Term!)`, as `Warpstone.Sandbox` does.
- **Error messages are empty.** `UnexpectedTokenError` never passes a message to its base, so `ToString()`/`Message` give "Exception of type 'Warpstone.Errors.UnexpectedTokenError' was thrown." The private `GetMessage` that formats "Expected ... but found ..." is unused. The example prints `InputStartPosition`, `Expected` and `Found` itself. Fixing the library message would let the example print `error` directly.
- **Regex parsers show escaped patterns in `Expected`** (`\[0-9]\+`). `.WithName("number")` gives readable errors.
