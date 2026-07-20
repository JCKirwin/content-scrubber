# Extending the Content Scrubber

This implementation is deliberately minimal. Here are the most common extensions and how to add them.

## Allowlist (false-positive suppression)

Add an allowlist section to the spec file that exempts specific file+line combinations:

```
# Allowlist (suppress known false positives)
allow:docs/history.md:42
allow:README.md:*
```

In `ContentScanner`, after collecting hits, filter out any whose file path and line number match an allowlist entry.

## Severity levels

Add a severity prefix to spec entries:

```
error:Henderson
warn:maintenance
```

Extend `ScrubHit` with a `Severity` property. Change `ScrubResult.Passed` to only consider `error`-level hits as blocking.

## Multi-spec composition

Load multiple spec files and merge them:

```csharp
var combined = new ScrubSpec();
foreach (var path in specPaths)
{
    var partial = ScrubSpec.Load(path);
    combined.Literals.AddRange(partial.Literals);
    combined.Patterns.AddRange(partial.Patterns);
}
```

This lets you maintain a global spec (organization-wide) alongside project-specific specs.

## Incremental scanning with checksums

Cache file checksums between runs and skip unchanged files:

```csharp
var currentHash = ComputeHash(filePath);
if (previousHashes.TryGetValue(filePath, out var cached) && cached == currentHash)
    continue; // skip unchanged file
```

Store the hash index as a JSON file alongside the content directory.

## CI integration

Run the scanner as a build step. Exit with a non-zero code when hits are found:

```csharp
var result = scanner.Scan(directory);
if (!result.Passed)
{
    foreach (var hit in result.Hits)
        Console.Error.WriteLine($"BLOCKED: [{hit.Term}] {hit.FilePath}:{hit.LineNumber}");
    Environment.Exit(1);
}
```

This turns the scrubber into a CI gate that fails the build when forbidden terms are detected.

## Custom term matchers

Replace the literal/regex split with a `ITermMatcher` interface:

```csharp
public interface ITermMatcher
{
    string Label { get; }
    bool IsMatch(string text);
}
```

Implement matchers for literals, regex, glob patterns, or semantic checks (e.g., "looks like a phone number"). The scanner becomes agnostic to how terms are defined.
