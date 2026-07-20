# Code Walkthrough

This walkthrough tours the source files in order, explaining what each piece does and how it connects to the content scrubber pattern.

## ScrubHit.cs

A data record for one finding. Five properties: `Term` (what matched), `FilePath` (where), `LineNumber` (which line, null for filename hits), `Context` (the matched line or filename), and `IsFilenameHit` (distinguishes filename from content matches).

All properties use `required init` — a hit is immutable after construction.

## ScrubResult.cs

The aggregate result. `Passed` is a computed property: `Hits.Count == 0`. This means you can't construct a "passed" result that has hits — the type enforces the invariant.

`LiteralCount` and `PatternCount` echo how many terms were in the spec, useful for logging ("scanned against 5 literals and 2 patterns").

## ScrubSpec.cs

The spec loader. `Load` reads a file line by line, trims whitespace, skips blanks and `#` comments, and routes each line to either `Literals` or `Patterns` based on the `regex:` prefix.

```csharp
if (line.StartsWith("regex:"))
    spec.Patterns.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled));
else
    spec.Literals.Add(line);
```

Patterns are compiled at load time (`RegexOptions.Compiled`) so the per-line scan is fast. Loading from a missing file returns an empty spec — the scanner defaults to open, not closed.

## ContentScanner.cs

The engine. Three private methods divide the work:

**ScanFilename** checks the filename string against every literal (`Contains`, case-insensitive) and every regex. Filename hits get `IsFilenameHit = true`.

**ScanFileContents** reads all lines and checks each against every literal and pattern. Literals are converted to word-boundary regex at scan time via `BuildWordBoundaryPattern`:

```csharp
var escaped = Regex.Escape(literal);
var prefix = char.IsLetterOrDigit(literal[0]) ? @"\b" : "";
var suffix = char.IsLetterOrDigit(literal[^1]) ? @"\b" : "";
```

This ensures "Henderson" matches the word but not "Hendersonian," while a term like `@internal` still gets appropriate anchoring.

**Scan** orchestrates: check term count (empty spec = immediate pass), enumerate files, call `ScanFilename` and `ScanFileContents` for each, skip binary extensions for content scanning.

The `BinaryExtensions` set is a static list of extensions known to be non-text. Files with these extensions still get filename scanning but skip content scanning.

## Program.cs

The demo runs a four-step scenario:

1. **Create a spec** with three literals and one GPS-coordinate regex.
2. **Create three journal entries** — one clean, one with a landowner name and GPS coordinate, one with a maintenance code in the filename.
3. **Scan and report** — the scrubber blocks with detailed hit output.
4. **Add a new term and re-scan** — demonstrates the spec-driven workflow where adding a term catches previously-passing content.
