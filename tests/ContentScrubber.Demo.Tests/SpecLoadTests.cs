namespace ContentScrubber.Demo.Tests;

public class SpecLoadTests : IDisposable
{
    private readonly string _testDir;

    public SpecLoadTests()
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
    public void Load_ParsesLiteralsAndPatterns()
    {
        var specPath = Path.Combine(_testDir, "spec.txt");
        File.WriteAllText(specPath, "Henderson\n# comment\nregex:\\d+\nBlackwood\n");

        var spec = ScrubSpec.Load(specPath);

        Assert.Equal(2, spec.Literals.Count);
        Assert.Single(spec.Patterns);
        Assert.Contains("Henderson", spec.Literals);
        Assert.Contains("Blackwood", spec.Literals);
    }

    [Fact]
    public void Load_MissingFile_ReturnsEmptySpec()
    {
        var spec = ScrubSpec.Load(Path.Combine(_testDir, "nonexistent.txt"));

        Assert.Empty(spec.Literals);
        Assert.Empty(spec.Patterns);
    }

    [Fact]
    public void Load_SkipsCommentsAndBlankLines()
    {
        var specPath = Path.Combine(_testDir, "spec.txt");
        File.WriteAllText(specPath, "# header\n\n  \nHenderson\n# another comment\n");

        var spec = ScrubSpec.Load(specPath);

        Assert.Single(spec.Literals);
    }
}
