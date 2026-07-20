# ADR 0001: Fail-Closed Gate (Any Hit Blocks)

## Context

The scanner needs a policy for what happens when forbidden terms are found. Options include warn-only (log and proceed), threshold-based (block above N hits), or fail-closed (any hit blocks).

## Decision

Any single hit makes the overall result "blocked." There is no warn-only mode and no threshold.

## Consequences

- Zero ambiguity: one hit = one block. Authors know exactly what to expect.
- No risk of "warning fatigue" where flagged terms are routinely ignored.
- False positives are more disruptive — a legitimate use of a flagged term requires either rephrasing or an allowlist extension.
- The allowlist escape hatch (see extending docs) exists for known false positives, but it must be maintained manually.
