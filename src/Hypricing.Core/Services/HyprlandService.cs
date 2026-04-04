using Hypricing.Core.Infrastructure;
using Hypricing.HyprlangParser;
using Hypricing.HyprlangParser.Nodes;

namespace Hypricing.Core.Services;

/// <summary>
/// Owns the hyprland.conf lifecycle: load, modify, save, reload.
/// For v0.2 this operates on a single file (no source= resolution yet).
/// </summary>
public class HyprlandService
{
    private readonly CliRunner _cli;
    private ConfigNode? _config;
    private string? _filePath;

    public HyprlandService(CliRunner cli)
    {
        _cli = cli;
    }

    /// <summary>
    /// Loads and parses hyprland.conf.
    /// If <paramref name="path"/> is null, resolves via <see cref="ConfigFileLocator"/>.
    /// </summary>
    public async Task LoadAsync(string? path = null, CancellationToken ct = default)
    {
        _filePath = path ?? ConfigFileLocator.Resolve()
            ?? throw new FileNotFoundException("Could not locate hyprland.conf");

        var text = await File.ReadAllTextAsync(_filePath, ct);
        _config = HyprlangParser.HyprlangParser.Parse(text);
    }

    /// <summary>
    /// Returns all top-level $var declarations from the loaded config.
    /// </summary>
    public IReadOnlyList<DeclarationNode> GetDeclarations()
    {
        EnsureLoaded();
        return _config!.Children.OfType<DeclarationNode>().ToList();
    }

    /// <summary>
    /// Returns all top-level <c>env = KEY,VALUE</c> keyword nodes.
    /// </summary>
    public IReadOnlyList<KeywordNode> GetEnvironmentVariables()
    {
        EnsureLoaded();
        return _config!.Children.OfType<KeywordNode>()
            .Where(k => k.Keyword == "env")
            .ToList();
    }

    /// <summary>
    /// Writes the modified config back to disk and invokes hyprctl reload.
    /// Re-parses afterward to refresh Range references.
    /// </summary>
    public async Task SaveAsync(CancellationToken ct = default)
    {
        EnsureLoaded();

        var text = HyprlangWriter.Write(_config!);
        await File.WriteAllTextAsync(_filePath!, text, ct);

        // Re-parse to refresh Range references after write
        _config = HyprlangParser.HyprlangParser.Parse(text);

        await _cli.RunAsync("hyprctl", "reload", ct);
    }

    private void EnsureLoaded()
    {
        if (_config is null || _filePath is null)
            throw new InvalidOperationException("Config not loaded. Call LoadAsync first.");
    }
}
