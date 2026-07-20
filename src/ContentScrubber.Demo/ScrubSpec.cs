using System.Text.RegularExpressions;

namespace ContentScrubber.Demo;

public class ScrubSpec
{
    public List<string> Literals { get; } = [];
    public List<Regex> Patterns { get; } = [];

    public static ScrubSpec Load(string specFilePath)
    {
        var spec = new ScrubSpec();

        if (!File.Exists(specFilePath))
        {
            return spec;
        }

        foreach (var rawLine in File.ReadAllLines(specFilePath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith('#'))
            {
                continue;
            }

            if (line.StartsWith("regex:"))
            {
                var pattern = line["regex:".Length..].Trim();
                spec.Patterns.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled));
            }
            else
            {
                spec.Literals.Add(line);
            }
        }

        return spec;
    }
}
