namespace Hyprland.Configuration.Ast;

public class HyprComment(string text) : HyprStatement
{
    public string Text { get; set; } = text;
}