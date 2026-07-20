namespace ContentScrubber.Demo.Tests;

public class CaseInsensitiveTests : IDisposable
{
    private readonly string _testDir;

    public CaseInsensitiveTests()
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

    [Theory]
    [InlineData("HENDERSON")]
    [InlineData("henderson")]
    [InlineData("Henderson")]
    [InlineData("hEnDeRsOn")]
    public void LiteralMatch_IsCaseInsensitive(string variant)
    {
        File.WriteAllText(Path.Combine(_testDir, "entry.md"), $"Near {variant} property.");
        var spec = new ScrubSpec();
        spec.Literals.Add("Henderson");

        var scanner = new ContentScanner(spec);
        var result = scanner.Scan(_testDir);

        Assert.False(result.Passed);
    }
}
