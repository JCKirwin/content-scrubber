using System.Text.RegularExpressions;

namespace ContentScrubber.Demo;

public class ContentScanner
{
    private static readonly HashSet<string> BinaryExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".dll", ".bin", ".obj", ".png", ".jpg", ".gif", ".ico", ".zip", ".gz", ".tar",
    };

    private readonly ScrubSpec _spec;

    public ContentScanner(ScrubSpec spec)
    {
        _spec = spec;
    }

    public ScrubResult Scan(string directoryPath)
    {
        var hits = new List<ScrubHit>();

        if (_spec.Literals.Count == 0 && _spec.Patterns.Count == 0)
        {
            return new ScrubResult
            {
                Hits = hits,
                LiteralCount = 0,
                PatternCount = 0,
            };
        }

        foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileName(filePath);
            ScanFilename(fileName, filePath, hits);

            var ext = Path.GetExtension(filePath);
            if (BinaryExtensions.Contains(ext))
            {
                continue;
            }

            ScanFileContents(filePath, hits);
        }

        return new ScrubResult
        {
            Hits = hits,
            LiteralCount = _spec.Literals.Count,
            PatternCount = _spec.Patterns.Count,
        };
    }

    private void ScanFilename(string fileName, string filePath, List<ScrubHit> hits)
    {
        foreach (var literal in _spec.Literals)
        {
            if (fileName.Contains(literal, StringComparison.OrdinalIgnoreCase))
            {
                hits.Add(new ScrubHit
                {
                    Term = literal,
                    FilePath = filePath,
                    IsFilenameHit = true,
                    Context = fileName,
                });
            }
        }

        foreach (var pattern in _spec.Patterns)
        {
            if (pattern.IsMatch(fileName))
            {
                hits.Add(new ScrubHit
                {
                    Term = pattern.ToString(),
                    FilePath = filePath,
                    IsFilenameHit = true,
                    Context = fileName,
                });
            }
        }
    }

    private void ScanFileContents(string filePath, List<ScrubHit> hits)
    {
        var lines = File.ReadAllLines(filePath);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            foreach (var literal in _spec.Literals)
            {
                var wordBoundaryPattern = BuildWordBoundaryPattern(literal);
                if (Regex.IsMatch(line, wordBoundaryPattern, RegexOptions.IgnoreCase))
                {
                    hits.Add(new ScrubHit
                    {
                        Term = literal,
                        FilePath = filePath,
                        LineNumber = i + 1,
                        Context = line.Trim(),
                    });
                }
            }

            foreach (var pattern in _spec.Patterns)
            {
                if (pattern.IsMatch(line))
                {
                    hits.Add(new ScrubHit
                    {
                        Term = pattern.ToString(),
                        FilePath = filePath,
                        LineNumber = i + 1,
                        Context = line.Trim(),
                    });
                }
            }
        }
    }

    private static string BuildWordBoundaryPattern(string literal)
    {
        var escaped = Regex.Escape(literal);
        var prefix = char.IsLetterOrDigit(literal[0]) ? @"\b" : "";
        var suffix = char.IsLetterOrDigit(literal[^1]) ? @"\b" : "";
        return $"{prefix}{escaped}{suffix}";
    }
}
