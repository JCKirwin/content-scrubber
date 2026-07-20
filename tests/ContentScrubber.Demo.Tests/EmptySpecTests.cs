namespace ContentScrubber.Demo.Tests;

public class EmptySpecTests : IDisposable
{
    private readonly string _testDir;

    public EmptySpecTests()
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
    public void EmptySpec_AllContentPasses()
    {
        File.WriteAllText(Path.Combine(_testDir, "entry.md"), "Anything goes here.");
        var spec = new ScrubSpec();

        var scanner = new ContentScanner(spec);
        var result = scanner.Scan(_testDir);

        Assert.True(result.Passed);
        Assert.Empty(result.Hits);
    }

    [Fact]
    public void EmptySpec_TermCountsAreZero()
    {
        var spec = new ScrubSpec();
        var scanner = new ContentScanner(spec);
        var result = scanner.Scan(_testDir);

        Assert.Equal(0, result.LiteralCount);
        Assert.Equal(0, result.PatternCount);
        Assert.Equal(0, result.TotalTerms);
    }
}
