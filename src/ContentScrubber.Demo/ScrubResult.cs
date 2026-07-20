namespace ContentScrubber.Demo;

public class ScrubResult
{
    public bool Passed => Hits.Count == 0;
    public List<ScrubHit> Hits { get; init; } = [];
    public int LiteralCount { get; init; }
    public int PatternCount { get; init; }
    public int TotalTerms => LiteralCount + PatternCount;
}
