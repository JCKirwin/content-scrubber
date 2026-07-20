# ADR 0003: Empty Spec Passes All Content

## Context

When the spec file is empty or missing, the scanner could either block everything (fail-closed default) or pass everything (fail-open default). Both have valid security arguments.

## Decision

An empty spec results in all content passing. The scanner does not block by default.

## Consequences

- A missing or empty spec file does not break the pipeline — the scanner is a no-op and returns `Passed = true`.
- This means a misconfigured spec (e.g., wrong file path) silently passes all content. Callers should verify the spec loaded expected term counts before relying on the result.
- The rationale: the spec is the authority. If no terms are defined, there is nothing to scan for. Blocking on an empty spec would make the scanner unusable without a spec file, which is a worse default for adoption.
