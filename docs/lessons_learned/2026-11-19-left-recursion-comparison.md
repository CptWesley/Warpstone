# Left-recursion comparison (Warpstone vs. Guido/pegen vs. Umeda–Maeda)

Task: compare findings from an earlier claude.ai chat (Umeda & Maeda, JIP 29, 2021 vs. Guido van Rossum's left-recursive PEG article / pegen) against Warpstone's `GrowParser` implementation.

## Issues hit

- **claude.ai chat and share links cannot be fetched.** `claude.ai/chat/...` returns 403; `claude.ai/share/...` returns only the JS app shell. Ask the user to paste the content straight away.
- **`dotnet test -f net9.0` / `-f net8.0` failed restore** with `NU1102: Unable to find package System.Reflection.Metadata with version (>= 8.0.0)` (SDK 11.0.100 installed). `--no-restore` didn't help: `obj/project.assets.json` had no `net9.0` target.
- **That failed restore rewrote `src/Warpstone.Sources/packages.lock.json`** (netstandard2.0 → net9.0). Run `git status` after any `-f` restore and revert with `git checkout -- <lockfile>`.
- **Workaround for running probe code:** a scratch console project outside `src/` with `ImportDirectoryBuildProps=false`, `ImportDirectoryPackagesProps=false`, and `<Compile Include=".../src/Warpstone/**/*.cs" Exclude="Properties/**;obj/**;bin/**" />`. It builds without any packages. The DLLs in `bin/` were stale, so don't reference them.
- **Manual `Grow` placement (pegen style):** the leader must lie on *every* cycle, including direct self-loops. In `S ← A 'b' / 'b'; A ← A 'a' / S 'a'` the leader is `A`, not `S`. Picking `S` caused a hard `Stack overflow` that killed the process (Recursive mode), with no catchable exception.

## Results worth remembering

- Auto mode (default options) fails on indirect left recursion (`S/A`, `S/A/B`, interlocking cycles) because the analysis grows *every* cycle entry point it finds, and the inner growers' memo entries go stale.
- Auto grow and memo off, with one `Grow` on a pegen-style leader: `S/A` and `S/A/B` pass.
