# README "Where do the ideas come from?" section

Task: add a README section linking the work Warpstone's implementation is based on (Ford's packrat paper, Warth et al. left recursion, Guido van Rossum's pegen blog post, Umeda & Maeda).

## Issues hit

- **Medium blocks WebFetch** (HTTP 403). Confirm Guido van Rossum's post URLs through the "Blog series" list in the [pegen README](https://github.com/gvanrossum/pegen) instead.
- **The "original direct-left-recursion paper"** is Warth, Douglass & Millstein, PEPM 2008. It covers indirect left recursion too, so the README describes it by its technique ("seed growing"), not as direct-only.
- **README.md uses CRLF.** The Edit tool kept CRLF; check with `file README.md` after editing.

## Links used

- Ford, ICFP 2002: https://arxiv.org/abs/cs/0603077 (also https://pdos.csail.mit.edu/~baford/packrat/icfp02/).
- Warth et al., PEPM 2008: https://web.cs.ucla.edu/~todd/research/pepm08.pdf (DOI 10.1145/1328408.1328424).
- Van Rossum, "Left-recursive PEG Grammars": https://medium.com/@gvanrossum_83706/left-recursive-peg-grammars-65dab3c580e1
- pegen leader selection, `compute_left_recursives` (code lives in `pegen/`, not `src/pegen/`; permalink pinned to the last commit touching the file so the line range stays valid): https://github.com/gvanrossum/pegen/blob/bc78cbc9d048ce389cf4d693b875726f56473a8b/pegen/parser_generator.py#L173-L200
- Umeda & Maeda, JIP 29 (2021) pp. 174–187: https://doi.org/10.2197/ipsjjip.29.174

The README text ties each source to the code: `GrowParser` (seed growing), `LeftRecursionAnalysis` (one pegen-style leader per cycle) and `CycleGrowParser` (Umeda–Maeda fallback for cycles with no single leader). If those types are renamed, update the README text.
