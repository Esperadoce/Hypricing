using System.Text;
using System.Text.RegularExpressions;

namespace Hyprland.Configuration.IO;

public sealed class HyprConfigLoader
{
    public int MaxDepth { get; init; } = 128;
    public int MaxFiles { get; init; } = 10_000;
    public int MaxOutputLines { get; init; } = 1_000_000;
    public int MaxOutputChars { get; init; } = 100_000_000; // ~100MB

    private int _filesSeen;
    private int _linesEmitted;

    public async Task<MergedConfig> LoadMergedAsync(string rootPath)
    {
        _filesSeen = 0;
        _linesEmitted = 0;

        var visited = new HashSet<string>(StringComparer.Ordinal);
        var text = new StringBuilder();
        var map = new List<SourceLocation>();

        await ExpandAsync(Path.GetFullPath(rootPath), visited, text, map, depth: 0, includingFile: null, includingLine: null);

        return new MergedConfig(text.ToString(), map.ToArray());
    }

    private async Task ExpandAsync(
        string filePath,
        HashSet<string> visited,
        StringBuilder output,
        List<SourceLocation> lineMap,
        int depth,
        string? includingFile,
        int? includingLine)
    {
        if (depth > MaxDepth)
        {
            AppendError(output, lineMap, includingFile, includingLine,
                $"# loader-error: max include depth exceeded ({MaxDepth}) at {filePath}");
            return;
        }

        filePath = Path.GetFullPath(filePath);

        if (++_filesSeen > MaxFiles)
        {
            AppendError(output, lineMap, includingFile, includingLine,
                $"# loader-error: max included files exceeded ({MaxFiles})");
            return;
        }

        if (!visited.Add(filePath))
        {
            AppendError(output, lineMap, includingFile, includingLine,
                $"# loader-error: recursive source {filePath} (from {includingFile}:{includingLine})");
            return;
        }

        if (!File.Exists(filePath))
        {
            AppendError(output, lineMap, includingFile, includingLine,
                $"# loader-error: missing source {filePath} (from {includingFile}:{includingLine})");
            visited.Remove(filePath);
            return;
        }

        var baseDir = Path.GetDirectoryName(filePath)!;

        using var fs = File.OpenRead(filePath);
        using var sr = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

        int lineNo = 0;
        while (await sr.ReadLineAsync() is { } line)
        {
            lineNo++;

            var trimmed = line.TrimStart();

            if (!trimmed.StartsWith('#') &&
                TryParseSource(line, out var include))
            {
                var resolved = NormalizePath(include, baseDir);

                var matches = ExpandGlob(resolved);
                if (matches.Count == 0)
                {
                    await ExpandAsync(resolved, visited, output, lineMap, depth + 1, filePath, lineNo);
                }
                else
                {
                    foreach (var m in matches)
                        await ExpandAsync(m, visited, output, lineMap, depth + 1, filePath, lineNo);
                }

                continue;
            }

            EmitLine(output, lineMap, filePath, lineNo, line);
        }

        visited.Remove(filePath);
    }

    private void EmitLine(StringBuilder output, List<SourceLocation> map, string file, int lineNo, string line)
    {
        if (++_linesEmitted > MaxOutputLines)
            throw new InvalidOperationException($"MaxOutputLines exceeded ({MaxOutputLines}).");

        output.AppendLine(line);
        map.Add(new SourceLocation(file, lineNo));

        if (output.Length > MaxOutputChars)
            throw new InvalidOperationException($"MaxOutputChars exceeded ({MaxOutputChars}).");
    }

    private static void AppendError(StringBuilder output, List<SourceLocation> map, string? file, int? line, string msg)
    {
        output.AppendLine(msg);
        map.Add(new SourceLocation(file ?? "<loader>", line ?? 0));
    }

    private static bool TryParseSource(string line, out string include)
    {
        include = "";

        int hash = line.IndexOf('#');
        string noComment = hash >= 0 ? line[..hash] : line;

        int eq = noComment.IndexOf('=');
        if (eq < 0) return false;

        string lhs = noComment[..eq].Trim();
        if (!lhs.Equals("source", StringComparison.OrdinalIgnoreCase))
            return false;

        string rhs = noComment[(eq + 1)..].Trim();
        rhs = rhs.Trim().Trim('"', '\'');

        if (rhs.Length == 0) return false;

        include = rhs;
        return true;
    }

    private static string NormalizePath(string include, string baseDir)
    {
        include = ExpandEnvVars(include);
        include = ResolveTilde(include);

        if (Path.IsPathRooted(include))
            return Path.GetFullPath(include);

        return Path.GetFullPath(Path.Combine(baseDir, include));
    }

    private static string ResolveTilde(string path)
    {
        if (!path.StartsWith('~'))
            return path;

        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, path[1..].TrimStart('/', '\\'));
    }

    private static string ExpandEnvVars(string s)
    {
        return Regex.Replace(
            s,
            @"\$(\{(?<v>[A-Za-z_][A-Za-z0-9_]*)\}|(?<v>[A-Za-z_][A-Za-z0-9_]*))",
            m =>
            {
                var name = m.Groups["v"].Value;
                var val = Environment.GetEnvironmentVariable(name);
                return string.IsNullOrEmpty(val) ? m.Value : val;
            });
    }

    private static List<string> ExpandGlob(string path)
    {
        if (!path.Contains('*') && !path.Contains('?'))
            return new List<string> { path };

        var dir = Path.GetDirectoryName(path);
        var pattern = Path.GetFileName(path);

        if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(pattern) || !Directory.Exists(dir))
            return new List<string>();

        var results = Directory.GetFiles(dir, pattern, SearchOption.TopDirectoryOnly);
        Array.Sort(results, StringComparer.Ordinal);
        return results.ToList();
    }
}
