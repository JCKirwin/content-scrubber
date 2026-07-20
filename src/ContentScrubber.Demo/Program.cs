namespace ContentScrubber.Demo;

public static class Program
{
    public static void Main()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "content-scrubber-demo");
        if (Directory.Exists(baseDir))
        {
            Directory.Delete(baseDir, true);
        }

        var journalDir = Path.Combine(baseDir, "journals");
        var specPath = Path.Combine(baseDir, "scrub-spec.txt");
        Directory.CreateDirectory(journalDir);

        Console.WriteLine("=== Content Scrubber Demo: Trail Journal ===");
        Console.WriteLine();

        // Create the scrub spec.
        File.WriteAllText(specPath, """
            # Forbidden terms for trail journal publishing
            # Literal terms (case-insensitive, word-boundary matched)
            Henderson
            Blackwood Ranch
            maintenance-code

            # Regex patterns (prefixed with regex:)
            regex:\b\d{1,3}\.\d{4,}[NS]\s*,?\s*\d{1,3}\.\d{4,}[EW]\b
            """);

        Console.WriteLine("Spec loaded:");
        var spec = ScrubSpec.Load(specPath);
        Console.WriteLine($"  Literals: {spec.Literals.Count}");
        Console.WriteLine($"  Patterns: {spec.Patterns.Count}");
        Console.WriteLine();

        // Create sample journal entries.
        File.WriteAllText(Path.Combine(journalDir, "sunrise-ridge.md"), """
            # Sunrise Ridge Trail
            Started at the east trailhead around 6:30 AM. The ridge offers panoramic views
            of the valley below. Spotted three deer near the creek crossing. Wildflowers
            are blooming along the south face. Total distance: 4.2 miles round trip.
            """);

        File.WriteAllText(Path.Combine(journalDir, "hidden-falls.md"), """
            # Hidden Falls Loop
            Parked near the Henderson property gate — the owner lets hikers use the lot
            on weekends. Trail follows the creek upstream for about a mile before the
            falls come into view. GPS waypoint for the falls: 42.3647N, 83.1723W.
            """);

        File.WriteAllText(Path.Combine(journalDir, "maintenance-code-trail-report.md"), """
            # Trail Maintenance Report
            Section 4B needs regrading after the spring runoff. Contacted the
            Blackwood Ranch caretaker about the fallen tree blocking the north fork.
            """);

        var scanner = new ContentScanner(spec);

        // Scan all journals.
        Console.WriteLine("--- Scanning journal entries ---");
        var result = scanner.Scan(journalDir);
        Console.WriteLine($"  Result: {(result.Passed ? "PASSED" : "BLOCKED")}");
        Console.WriteLine($"  Hits: {result.Hits.Count}");
        Console.WriteLine();

        foreach (var hit in result.Hits)
        {
            var location = hit.IsFilenameHit
                ? $"filename: {hit.Context}"
                : $"line {hit.LineNumber}: {hit.Context}";
            Console.WriteLine($"  [{hit.Term}] {Path.GetFileName(hit.FilePath)} -> {location}");
        }

        Console.WriteLine();

        // Demonstrate adding a new term and re-scanning.
        Console.WriteLine("--- Adding 'caretaker' to spec and re-scanning ---");
        File.AppendAllText(specPath, "\ncaretaker\n");
        var updatedSpec = ScrubSpec.Load(specPath);
        var updatedScanner = new ContentScanner(updatedSpec);
        var updatedResult = updatedScanner.Scan(journalDir);
        Console.WriteLine($"  Result: {(updatedResult.Passed ? "PASSED" : "BLOCKED")}");
        Console.WriteLine($"  Hits: {updatedResult.Hits.Count}");
        Console.WriteLine();
        Console.WriteLine("=== Demo complete ===");
    }
}
