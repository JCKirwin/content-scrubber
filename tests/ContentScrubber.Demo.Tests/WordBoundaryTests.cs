namespace ContentScrubber.Demo.Tests;

public class WordBoundaryTests : IDisposable
{
    private readonly string _testDir;

    public WordBoundaryTests()
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
    public void LiteralWithWordBoundary_DoesNotMatchSubstring()
    {
        File.WriteAllText(Path.Combine(_testDir, "entry.md"), "The Hendersonian society met today.");
        var spec = new ScrubSpec();
        spec.Literals.Add("Henderson");

        var scanner = new ContentScanner(spec);
        var result = scanner.Scan(_testDir);

        Assert.True(result.Passed);
    }

    [Fact]
    public void LiteralWithWordBoundary_MatchesExactWord()
    {
        File.WriteAllText(Path.Combine(_testDir, "entry.md"), "Near Henderson today.");
        var spec = new ScrubSpec();
        spec.Literals.Add("Henderson");

        var scanner = new ContentScanner(spec);
        var result = scanner.Scan(_testDir);

        Assert.False(result.Passed);
    }
}
