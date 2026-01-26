namespace Hyprland.Configuration.IO;

public sealed class HyprFile
{
    public string Path { get; }
    public string RawText { get; }

    private HyprFile(string path, string rawText)
    {
        Path = path;
        RawText = rawText;
    }

    public static async Task<HyprFile> LoadAsync(string path)
    {
        string full = System.IO.Path.GetFullPath(path);
        string text = await File.ReadAllTextAsync(full);
        return new HyprFile(full, text);
    }
}
