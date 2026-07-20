# Content Scrubber

A spec-driven forbidden-term scanner in C# that gates publishing by checking files and filenames — any single hit blocks the output. Fork it, read it, adapt it.

## What you'll learn

- How to build a fail-closed content gate with zero false-negative tolerance
- How word-boundary matching reduces false positives for literal terms
- How to mix literal strings and regex patterns in a single spec file
- How to scan both filenames and file contents in one pass
- How to test scanner behavior with deterministic file fixtures

## Quick Start

```bash
git clone https://github.com/JCKirwin/content-scrubber.git
cd content-scrubber
dotnet run --project src/ContentScrubber.Demo
```

The demo simulates a trail journal platform. Three journal entries are scanned against a spec that forbids landowner names, GPS coordinates, and maintenance codes. Two entries are blocked with detailed hit reports; one passes clean.

```
=== Content Scrubber Demo: Trail Journal ===

Spec loaded:
  Literals: 3
  Patterns: 1

--- Scanning journal entries ---
  Result: BLOCKED
  Hits: 5

  [Henderson] hidden-falls.md -> line 2: Parked near the Henderson property gate...
  [42.3647N, 83.1723W] hidden-falls.md -> line 4: GPS waypoint for the falls...
  [maintenance-code] maintenance-code-trail-report.md -> filename
  [Blackwood Ranch] maintenance-code-trail-report.md -> line 3: Blackwood Ranch caretaker...
```

## Run the tests

```bash
dotnet test tests/ContentScrubber.Demo.Tests
```

## Project structure

```
content-scrubber/
├── src/ContentScrubber.Demo/         Core scanner + trail-journal demo
│   ├── ScrubSpec.cs                  Spec file loader (literals + regex)
│   ├── ContentScanner.cs             Directory-tree scanner engine
│   ├── ScrubHit.cs                   Single finding data record
│   ├── ScrubResult.cs                Aggregate pass/fail result
│   └── Program.cs                    Demo entry point
├── tests/ContentScrubber.Demo.Tests/ 18 xUnit v3 tests
├── docs/                            Pattern docs, architecture, ADRs
├── samples/demo-data.json           Journal + spec configuration
└── .github/workflows/ci.yml        Build + test on push/PR
```

## Documentation

- [01 — The Pattern](docs/01-the-pattern.md): What a content scrubber is and why you'd use one
- [02 — Architecture](docs/02-architecture.md): Components, scan flow, and how they connect
- [03 — Walkthrough](docs/03-walkthrough.md): File-by-file tour of the source code
- [04 — Tradeoffs](docs/04-tradeoffs.md): What this design trades and when to choose differently
- [05 — Extending](docs/05-extending.md): Common extensions and how to add them

## License

[MIT](LICENSE) — Copyright (c) JCKirwin
