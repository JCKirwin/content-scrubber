namespace ContentScrubber.Demo.Tests;

public class HitDetailTests : IDisposable
{
    private readonly string _testDir;

    public HitDetailTests()
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
    public void Hit_IncludesFilePathAndLineNumber()
    {
        File.WriteAllText(Path.Combine(_testDir, "notes.md"), "Line one\nVisited Henderson\nLine three");
        var spec = new ScrubSpec();
        spec.Literals.Add("Henderson");

        var scanner = new ContentScanner(spec);
        var result = scanner.Scan(_testDir);

        var hit = result.Hits.First(h => !h.IsFilenameHit);
        Assert.Contains("notes.md", hit.FilePath);
        Assert.Equal(2, hit.LineNumber);
        Assert.Contains("Henderson", hit.Context!);
    }

    [Fact]
    public void Hit_IncludesMatchedTerm()
    {
        File.WriteAllText(Path.Combine(_testDir, "entry.md"), "Near Blackwood Ranch gate.");
        var spec = new ScrubSpec();
        spec.Literals.Add("Blackwood Ranch");

        var scanner = new ContentScanner(spec);
        var result = scanner.Scan(_testDir);

        Assert.Contains(result.Hits, h => h.Term == "Blackwood Ranch");
    }
}
