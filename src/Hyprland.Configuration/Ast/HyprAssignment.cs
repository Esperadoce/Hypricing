namespace Hyprland.Configuration.Ast;

public class HyprAssignment(string key, string rawValue) : HyprStatement
{
    public string Key { get; set; } = key;
    public string RawValue { get; set; } = rawValue;
}
