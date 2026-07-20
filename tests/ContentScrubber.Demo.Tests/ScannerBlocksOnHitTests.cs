namespace ContentScrubber.Demo.Tests;

public class ScannerBlocksOnHitTests : IDisposable
{
    private readonly string _testDir;

    public ScannerBlocksOnHitTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"scrub-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    [Fact]
    public void SingleLiteralHit_BlocksEntireResult()
    {
        WriteFile("entry.md", "Visited the Henderson property today.");
        var spec = MakeSpec(literals: ["Henderson"]);
        var scanner = new ContentScanner(spec);

        var result = scanner.Scan(_testDir);

        Assert.False(result.Passed);
        Assert.Single(result.Hits);
    }

    [Fact]
    public void SingleRegexHit_BlocksEntireResult()
    {
        WriteFile("entry.md", "GPS: 42.3647N, 83.1723W");
        var spec = MakeSpec(patterns: [@"\b\d{1,3}\.\d{4,}[NS]\s*,?\s*\d{1,3}\.\d{4,}[EW]\b"]);
        var scanner = new ContentScanner(spec);

        var result = scanner.Scan(_testDir);

        Assert.False(result.Passed);
        Assert.Single(result.Hits);
    }

    [Fact]
    public void CleanContent_Passes()
    {
        WriteFile("entry.md", "Beautiful sunrise over the ridge this morning.");
        var spec = MakeSpec(literals: ["Henderson", "Blackwood"]);
        var scanner = new ContentScanner(spec);

        var result = scanner.Scan(_testDir);

        Assert.True(result.Passed);
        Assert.Empty(result.Hits);
    }

    private void WriteFile(string name, string content) =>
        File.WriteAllText(Path.Combine(_testDir, name), content);

    private static ScrubSpec MakeSpec(string[]? literals = null, string[]? patterns = null)
    {
        var spec = new ScrubSpec();
        foreach (var l in literals ?? [])
        {
            spec.Literals.Add(l);
        }

        foreach (var p in patterns ?? [])
        {
            spec.Patterns.Add(new System.Text.RegularExpressions.Regex(p, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }

        return spec;
    }
}
