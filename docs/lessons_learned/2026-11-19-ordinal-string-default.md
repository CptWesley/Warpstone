# Ordinal string matching by default

Branch `perf/ordinal-string-default`, branched from `main` after Phase 1 (#45) was merged. It is the "quick win" from
[2026-11-19-performance-analysis.md](2026-11-19-performance-analysis.md).

## Change

- `Parsers.String(value)` and `String(value, ignoreCase)` now use `StringComparison.Ordinal` / `OrdinalIgnoreCase`. Before, they
  used `CultureInfo.CurrentCulture`, a linguistic comparison. The overloads that take a `CultureInfo`, `CompareOptions` or
  `StringComparison` are unchanged.
- `StringParseInput.Match` takes a `string.CompareOrdinal` fast path for `CompareOptions.Ordinal`. `TextReaderParseInput`
  already compared characters directly.
- BenchmarkDotNet ShortRun, MiniJava: large 10.7 → 9.2 ms, medium 865 → 745 µs, small 71 → 62 µs (about −14%). The scratch
  harness showed 15 → 11.8 ms (−21%).

## Pitfalls

- `Match` compares exactly `value.Length` input characters. A test with strings that are linguistically equal but have different
  lengths (precomposed `é` vs `e` + U+0301) can't tell ordinal from linguistic, because neither ever matched. Use the Turkish i
  instead: under `tr-TR`, ignore-case linguistic says `"i"` ≠ `"I"`, and ordinal says they are equal. NUnit's `[SetCulture("tr-TR")]` sets the
  current culture for one test. Check that a new test fails without the fix (`git stash push -- <file>`, run it, `git stash pop`).
- BenchmarkDotNet ShortRun drifted about 6% between runs for the unchanged libraries (Pidgin 12.7 → 13.5 ms, Parlot 1.37 → 1.46 ms).
  Compare ratios within one run rather than absolute numbers across runs.
- #45 committed five vim swap files (`src/Warpstone.Sources.Tests/Helpers/snapshots/.Snapshots...verified.cs.swp`). They are dotfiles,
  so `ls *.swp` doesn't show them. Use `git ls-files | grep '\.swp$'`.
