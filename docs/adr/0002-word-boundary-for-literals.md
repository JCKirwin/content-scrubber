# ADR 0002: Word-Boundary Anchors for Literal Terms

## Context

Literal term matching can use substring search (catches "Hendersonian" for "Henderson"), exact-word search (word boundaries), or configurable per-term matching. The choice affects false-positive and false-negative rates.

## Decision

Wrap literal terms in `\b` word-boundary anchors at scan time when the term starts or ends with a word character. Terms starting or ending with non-word characters get partial anchoring.

## Consequences

- "Henderson" matches "Henderson" and "Henderson's" but not "Hendersonian" — reduced false positives.
- Multi-word terms like "Blackwood Ranch" match the exact phrase with boundaries at the outer edges.
- A term embedded in a URL or path (e.g., "henderson" in "henderson-trail.md") is caught by filename scanning (substring match) even if word-boundary matching would miss it in content.
- The anchoring is automatic — spec authors don't need to write regex for literal terms.
