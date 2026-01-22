using Hyprland.Configuration.Ast;
using Hyprland.Configuration.Formatting;
using Hyprland.Configuration.Parsing;

namespace Hyprland.Configuration.IO;

public sealed class HyprFile(string path, HyprDocument document)
{
    public string Path { get; } = path;
    public HyprDocument Document { get; } = document;

    public static HyprFile Load(string path)
    {
        var text = File.ReadAllText(path);
        var parser = new HyprParser();
        var document = parser.Parse(text);
        return new HyprFile(path, document);
    }

    public void Save(string path)
    {
        var writer = new HyprWriter();
        File.WriteAllText(path, writer.Write(Document));
    }
}