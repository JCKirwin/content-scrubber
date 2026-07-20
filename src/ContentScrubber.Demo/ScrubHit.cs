namespace ContentScrubber.Demo;

public class ScrubHit
{
    public required string Term { get; init; }
    public required string FilePath { get; init; }
    public int? LineNumber { get; init; }
    public string? Context { get; init; }
    public bool IsFilenameHit { get; init; }
}
