# The Pattern

Content that goes public can leak sensitive terms — internal URLs, personal names, proprietary jargon, GPS coordinates. This document describes a content scrubber: a spec-driven scanner that gates publishing by checking a directory tree for forbidden terms. Any single hit blocks the entire output.

## What is a content scrubber?

A content scrubber is a read-only gate. You give it a directory of content and a spec file listing forbidden terms. It scans every file — names and contents — and returns a pass/fail result with detailed hit reports. It never modifies content; it only tells you whether the content is safe to publish.

## The problem it solves

Consider a trail journal platform where hikers share journal entries publicly. A hiker writes about a trail that crosses private land and mentions the landowner's name. Another entry includes raw GPS coordinates for a sensitive location. A third references an internal trail-maintenance code that means nothing to the public but reveals organizational structure.

You need a gate before the "publish" button that catches these leaks. The content scrubber fills that role. Define the forbidden terms once in a spec file, and every piece of content is checked before it goes live.

## How it works

### Spec file

A plain-text file where each line is either a literal term or a regex pattern. Comments start with `#`. Blank lines are ignored.

```
# Literal terms (case-insensitive, word-boundary matched)
Henderson
Blackwood Ranch
maintenance-code

# Regex patterns
regex:\b\d{1,3}\.\d{4,}[NS]\s*,?\s*\d{1,3}\.\d{4,}[EW]\b
```

Literal terms are matched case-insensitive with word-boundary anchors. Regex patterns are prefixed with `regex:` and compiled with `IgnoreCase`.

### Scanner

The scanner walks the directory tree. For each file, it checks:

1. **Filename** — does the filename contain any forbidden term?
2. **Contents** — does any line contain a forbidden term?

Binary files (`.exe`, `.dll`, `.png`, etc.) are skipped for content scanning but their filenames are still checked.

### Result

The scanner returns a structured result:

- `Passed` — `true` if zero hits, `false` otherwise
- `Hits` — a list of `ScrubHit` objects, each with the matched term, file path, line number (for content hits), context snippet, and whether it was a filename hit
- `LiteralCount` / `PatternCount` — how many terms were in the spec

### Fail-closed

Any single hit makes the result `Passed = false`. There is no warn-only mode, no severity levels, no allowlist. The scanner blocks or it doesn't.

## What it does not do

- No content modification or redaction. The scanner reports, it does not fix.
- No binary file inspection. Only text files are scanned for content.
- No incremental scanning. Every invocation scans the full tree.
- No severity levels. Every hit is equally blocking.

These constraints make the scanner predictable. You know exactly what it checks, and you know that one hit means one block.
