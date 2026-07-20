# Tradeoffs

Every design choice trades something. This page makes the tradeoffs explicit so you can decide whether they fit your context.

## Full tree scan vs incremental

Every invocation scans the entire directory tree. An incremental scanner would track file checksums and only re-scan changed files. The full scan is simpler and avoids stale-cache bugs but costs more I/O on large trees.

**Choose incremental if:** your content directory has thousands of files and most don't change between scans. Maintain a checksum index and skip files whose hash hasn't changed.

## Word-boundary matching vs substring

Literal terms get word-boundary anchors, so "Henderson" doesn't match "Hendersonian." A substring match would catch more variants but also produce false positives.

**Choose substring matching if:** you want to catch all forms of a term regardless of word context. Remove the `\b` anchors in `BuildWordBoundaryPattern`.

## Fail-closed vs warn-only

Any single hit blocks the entire result. A warn-only mode would let content through with warnings, useful for advisory scanning.

**Choose warn-only if:** blocking is too disruptive and you trust authors to act on warnings. Add a `Severity` field to `ScrubHit` and a `Warnings` list to `ScrubResult`.

## Spec file vs database

Terms live in a plain-text file. A database would give you versioning, access control, audit history, and API access. The text file is simpler and works with any editor or CI pipeline.

**Choose a database if:** multiple teams manage terms, you need change history, or you want an API for term management.

## No binary inspection

Binary files are skipped for content scanning. A tool like `strings` could extract text from binaries, but it's noisy and slow.

**Choose binary inspection if:** you generate binary artifacts (PDFs, images with EXIF data) that might contain sensitive text. Run `strings` and scan the output as text.

## No redaction

The scanner reports hits but never modifies content. An auto-redaction feature would replace forbidden terms with `[REDACTED]` or similar. This is risky — automated redaction can break document structure and mask context.

**Choose redaction if:** you have high volume and manual remediation is impractical. Always preserve the original and generate the redacted version as a copy.
