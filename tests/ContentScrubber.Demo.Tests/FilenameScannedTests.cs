namespace ContentScrubber.Demo.Tests;

public class FilenameScannedTests : IDisposable
{
    private readonly string _testDir;

    public FilenameScannedTests()
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
    public void ForbiddenTermInFilename_IsDetected()
    {
        File.WriteAllText(Path.Combine(_testDir, "henderson-trail.md"), "Clean content here.");
        var spec = new ScrubSpec();
        spec.Literals.Add("henderson");

        var scanner = new ContentScanner(spec);
        var result = scanner.Scan(_testDir);

        Assert.False(result.Passed);
        var filenameHit = Assert.Single(result.Hits, h => h.IsFilenameHit);
        Assert.Contains("henderson-trail.md", filenameHit.Context!);
    }

    [Fact]
    public void RegexInFilename_IsDetected()
    {
        File.WriteAllText(Path.Combine(_testDir, "maintenance-code-report.md"), "Clean.");
        var spec = new ScrubSpec();
        spec.Patterns.Add(new System.Text.RegularExpressions.Regex("maintenance-code",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase));

        var scanner = new ContentScanner(spec);
        var result = scanner.Scan(_testDir);

        Assert.False(result.Passed);
        Assert.Contains(result.Hits, h => h.IsFilenameHit);
    }
}
