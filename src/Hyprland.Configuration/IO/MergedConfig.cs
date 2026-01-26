namespace Hyprland.Configuration.IO;

public sealed record MergedConfig(string Text, SourceLocation[] LineMap);