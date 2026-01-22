namespace Hyprland.Configuration.Ast;

public class HyprBlock(string name) : HyprStatement
{
    public string Name { get; set; } = name;
    public List<HyprStatement> Statements { get; } = [];
    
    public string CloseBraceTrailingComment { get; set; } = string.Empty;
    
}